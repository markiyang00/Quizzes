using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzes.Data;
using Quizzes.Data.Model;
using Quizzes.ViewModels;

namespace Quizzes.Controllers
{
	public class UserTestController : Controller
	{
		private readonly AppDBContext context;

		public UserTestController(AppDBContext context)
		{
			this.context = context;
		}


		public IActionResult Mes(UrlViewModel urlView)
		{
			return View(urlView);
		}

		//[Route("UserTest/Start/{url}")]
		public IActionResult Start(string url)
		{
			var urlTest = context.UrlTests.FirstOrDefault(a => a.Url == url);
			if (urlTest != null)
			{
				var data = DateTime.Now;
				if (DateTime.Compare(data, urlTest.Time) == -1)
				{
					if (urlTest.NumberOfRuns != null)
						if (urlTest.NumberOfRuns == 0)
						{
							var urlModel = new UrlViewModel {Mes = "NumberOfRuns = 0", Url = url};
							return RedirectToAction("Mes", urlModel);
						}
						else
						{
							urlTest.NumberOfRuns--;
							context.Update(urlTest);
							context.SaveChanges();
						}

					var questionPage = CreateQuestionPage(urlTest);
					return View(questionPage);
					
				}

				var urlModel1 = new UrlViewModel {Mes = "Time is overdue ", Url = url};
				return RedirectToAction("Mes", urlModel1);
			}

			var urlModel2 = new UrlViewModel {Mes = "Not Gul ", Url = url};
			return RedirectToAction("Mes", urlModel2);
		}

		[Route("UserTest/Start/{id}")]
		[HttpPost]
		public IActionResult Start(int id,QuestionPageViewModel userQuestionPage)
		{
			userQuestionPage.QuestionId = id;
			var urlTestBase = context.UrlTests.AsNoTracking().First(a => a.Url == userQuestionPage.Url);
			if (!string.IsNullOrEmpty(userQuestionPage.UrlTestName))
			{
				if (urlTestBase.Name != userQuestionPage.UrlTestName)
				{
					urlTestBase.Name = userQuestionPage.UrlTestName;
					context.Update(urlTestBase);
					context.SaveChanges();
				}
			}

			if (string.IsNullOrEmpty(urlTestBase.Name))
			{
				userQuestionPage.TestName = context.Tests.First(a => a.Id == urlTestBase.TestId&!a.IsDel).Name;
				userQuestionPage.Mes = "Write Name";
				return View(userQuestionPage);
			}
			var urlAttends = context.UrlTestAttends.AsNoTracking().Where(a => a.UrlTestUrl == userQuestionPage.Url)
				.OrderByDescending(a => a.NumberOfRun).ToList();
			var urlAttend = new UrlTestAttend() { UrlTestUrl = userQuestionPage.Url };
			if (urlAttends.Count == 0)
				urlAttend.NumberOfRun = 1;
			else
				urlAttend.NumberOfRun = urlAttends[0].NumberOfRun + 1;
			urlAttend.StartTimeTest = DateTime.Now;
			context.UrlTestAttends.Add(urlAttend);
			context.SaveChanges();
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == userQuestionPage.Url & a.NumberOfRun == urlAttend.NumberOfRun);

			return RedirectToAction("QuestionPage",new{ testAttemptId=urlAttendBase.Id,id});
		}

		[Route("UserTest/QuestionPage/{testAttemptId}/{id}")]
		public IActionResult QuestionPage(int testAttemptId, int id)
		{
			var questionPage = new QuestionPageViewModel();
			var urlAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == testAttemptId);
			if (urlAttend.IsEnd)
			{
				return RedirectToAction("CheckTest","Admin");
			}
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == urlAttend.UrlTestUrl);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == urlTest.Url & a.NumberOfRun == urlAttend.NumberOfRun);
			questionPage.Answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == id).ToList();
			var resultsBase = context.Results.AsNoTracking()
				.Where(a => a.UrlTestAttendId == testAttemptId).ToList();
			if (questionPage.Answers != null)
			{
				var answersUser = AnswersUser(id, resultsBase);
				if (answersUser.Count != 0)
					foreach (var answer in answersUser)
					{
						for (var i = 0; i < questionPage.Answers.Count; i++)
						{
							if (answer.Id == questionPage.Answers[i].Id)
							{
								questionPage.Answers[i].Selected = true;
							}
						}
					}
			}

			var questions = NewQuestion(id, questionPage, test, urlTest, urlAttendBase, resultsBase);
			FilledNextPrev(id, questionPage, questions);
			questionPage.QuestionId = questionPage.Question.Id;
			return View(questionPage);
		}

		[Route("UserTest/QuestionPage/{testAttemptId}/{id}")]
		[HttpPost]
		public IActionResult QuestionPage(int testAttemptId, int id, QuestionPageViewModel userQuestionPage)
		{
			var urlAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == testAttemptId);
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == urlAttend.UrlTestUrl);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == urlTest.Url & a.NumberOfRun == urlAttend.NumberOfRun);
			var resultsBase = context.Results.AsNoTracking()
				.Where(a => a.UrlTestAttendId == urlAttendBase.Id).ToList();
			if (userQuestionPage.Answers != null)
			{
				var answersUser = AnswersUser(userQuestionPage.QuestionId, resultsBase);
				AddResult(userQuestionPage, answersUser, urlAttend, urlAttendBase);
			}

			var dateNow = DateTime.Now;
			var data = test.TestTime;
			urlAttend.TestTime = dateNow.Subtract(urlAttendBase.StartTimeTest);
			if (id == -2| (TimeSpan.Compare(urlAttend.TestTime, data) == 1))
			{
				if (string.IsNullOrEmpty(urlTest.Name))
				{
					userQuestionPage.Question=NewQuestion(userQuestionPage.Question.Id, userQuestionPage, test, urlTest, urlAttendBase, resultsBase).Find(a=>a.Id==userQuestionPage.Question.Id);
					userQuestionPage.QuestionId = userQuestionPage.Question.Id;
					userQuestionPage.Mes = "Not write Username!";
					return View(userQuestionPage);
				}

				urlAttend.IsEnd = true;
				context.Update(urlAttend);
				context.SaveChanges();
				return RedirectToAction("CheckTest","Admin");
			}

			var questions = NewQuestion(id, userQuestionPage, test, urlTest, urlAttendBase, resultsBase);
			FilledNextPrev(id, userQuestionPage, questions);
			userQuestionPage.QuestionId = userQuestionPage.Question.Id;
			return RedirectToAction("QuestionPage","UserTest");
		}

		private QuestionPageViewModel CreateQuestionPage(UrlTest urlTest)
		{
			var questionPage = new QuestionPageViewModel();
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId&!a.IsDel);
			questionPage.TestName = test.Name;
			questionPage.UrlTestName = urlTest.Name;
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).OrderBy(a => a.Id)
				.ToList();
			if (questions.Count != 0)
				questionPage.QuestionId = questions[0].Id;
			return questionPage;
		}

		private List<Answer> AnswersUser(int id, List<Result> resultsBase)
		{
			var answersUser = new List<Answer>();
			foreach (var result in resultsBase)
			{
				var answer = context.Answers.FirstOrDefault(a =>
					a.Id == result.AnswerId & a.QuestionId == id);
				if (answer != null)
					answersUser.Add(answer);
			}

			return answersUser;
		}

		private List<Question> NewQuestion(int id, QuestionPageViewModel userQuestionPage, Test test, UrlTest urlTest,
			UrlTestAttend urlAttendBase, List<Result> resultsBase)
		{
			userQuestionPage.TestName = test.Name;
			userQuestionPage.UrlTestName = urlTest.Name;
			userQuestionPage.UrlTestAttendId = urlAttendBase.Id;
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel)
				.OrderBy(a => a.Id)
				.ToList();


			userQuestionPage.Answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == id).ToList();
			var answersPage = new List<Answer>();
			foreach (var result in resultsBase)
			{
				var answer = context.Answers.FirstOrDefault(a => a.Id == result.AnswerId & a.QuestionId == id);
				if (answer != null)
					answersPage.Add(answer);
			}

			if (answersPage.Count != 0)
			{
				foreach (var answer in answersPage)
				{
					userQuestionPage.Answers.Find(a => a.Id == answer.Id).Selected = true;
				}
			}

			return questions;
		}

		private void AddResult(QuestionPageViewModel userQuestionPage, List<Answer> answersUser, UrlTestAttend urlAttend,
			UrlTestAttend urlAttendBase)
		{
			foreach (var answer in userQuestionPage.Answers)
			{
				if (answersUser.Count == 0)
				{
					if (answer.Selected)
					{
						var result = new Result() { AnswerId = answer.Id, UrlTestAttendId = urlAttend.Id };
						context.Results.Add(result);
					}
				}
				else
				{
					if (answersUser.FirstOrDefault(a => a.Id == answer.Id) == null)
					{
						if (answer.Selected)
						{
							var result = new Result() { AnswerId = answer.Id, UrlTestAttendId = urlAttend.Id };
							context.Results.Add(result);
						}
					}
					else
					{
						if (!answer.Selected)
						{
							var resultBase = context.Results.AsNoTracking()
								.First(a => a.UrlTestAttendId == urlAttendBase.Id & a.AnswerId == answer.Id);
							context.Remove(resultBase);
						}
					}
				}
			}


			context.SaveChanges();
		}

		private static void FilledNextPrev(int id, QuestionPageViewModel userQuestionPage, List<Question> questions)
		{
			var i = questions.FindIndex(q => q.Id == id);

			userQuestionPage.Question = questions[i];
			if (i != 0)
				userQuestionPage.PrevId = questions[i - 1].Id;
			else
				userQuestionPage.PrevId = null;
			if (questions.Count - 1 == i)
				userQuestionPage.NextId = null;
			else
				userQuestionPage.NextId = questions[i + 1].Id;

		}
	}
}
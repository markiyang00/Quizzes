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

		[Route("UserTest/Start/{testAttemptId}/{id}")]
		[HttpPost]
		public IActionResult Start(int testAttemptId, int id,QuestionPageViewModel userQuestionPage)
		{
			var urlAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == testAttemptId);
			var urlTestBase = context.UrlTests.AsNoTracking().First(a => a.Url == urlAttend.UrlTestUrl);
			if (!string.IsNullOrEmpty(userQuestionPage.UrlTestName))
			{
				if (urlTestBase.Name != userQuestionPage.UrlTestName)
				{
					urlTestBase.Name = userQuestionPage.UrlTestName;
					context.Update(urlTestBase);
					context.SaveChanges();
				}
			}

			userQuestionPage.QuestionId = id;
			return RedirectToAction("QuestionPage");
		}

		[Route("UserTest/QuestionPage/{testAttemptId}/{id}")]
		public IActionResult QuestionPage(int testAttemptId, int id)
		{
			var questionPage = new QuestionPageViewModel();
			var urlAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == testAttemptId);
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
		public IActionResult QuestionPagePost(int testAttemptId, int id, QuestionPageViewModel userQuestionPage)
		{
			var urlAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == testAttemptId);
			var urlTest = UrlTestCheck(userQuestionPage, urlAttend);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == urlTest.Url & a.NumberOfRun == urlAttend.NumberOfRun);
			var resultsBase = context.Results.AsNoTracking()
				.Where(a => a.UrlTestAttendId == urlAttendBase.Id).ToList();
			if (userQuestionPage.Answers != null)
			{
				var answersUser = AnswersUser(id, resultsBase);
				AddResult(userQuestionPage, answersUser, urlAttend, urlAttendBase);
			}

			if (id == -2)
			{
				return RedirectToAction("CheckTest", urlAttend);
			}

			var questions = NewQuestion(id, userQuestionPage, test, urlTest, urlAttendBase, resultsBase);
			FilledNextPrev(id, userQuestionPage, questions);
			userQuestionPage.QuestionId = userQuestionPage.Question.Id;
			return RedirectToAction("QuestionPage","UserTest");
		}

		public IActionResult CheckTest(UrlTestAttend urlTestAttend)
		{
			var point = 0;
			var results = context.Results.AsNoTracking().Where(a => a.UrlTestAttendId == urlTestAttend.Id).ToList();
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == urlTestAttend.UrlTestUrl);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel)
				.OrderBy(a => a.Id).ToList();
			var answers = new List<Answer>();

			foreach (var result in results)
			{
				answers.Add(context.Answers.AsNoTracking().First(a => a.Id == result.AnswerId));
			}

			foreach (var question in questions)
			{
				var answersBase = context.Answers.AsNoTracking().Where(a => a.QuestionId == question.Id).ToList();
				var answerQuestion = answers.Where(a => a.QuestionId == question.Id).ToList();
				var trueAnswer = true;
				var trueAnswerCount = 0;
				foreach (var answer in answersBase)
					if (answer.True)
						trueAnswerCount++;

				if (trueAnswerCount == answerQuestion.Count)
				{
					foreach (var answer in answerQuestion)
						if (!answer.True)
						{
							trueAnswer = false;
							break;
						}
				}
				else
					trueAnswer = false;

				if (trueAnswer)
				{
					point++;
				}
			}


			urlTestAttend.Point = point;
			context.Update(urlTestAttend);
			context.SaveChanges();
			var urlResult = new UrlResultViewModel()
			{
				Name = urlTest.Name,
				Point = urlTestAttend.Point,
				MaxPoint = questions.Count,
				NumberOfRun = urlTestAttend.NumberOfRun
			};
			return View(urlResult);
		}

		private QuestionPageViewModel CreateQuestionPage(UrlTest urlTest)
		{
			var questionPage = new QuestionPageViewModel();
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var urlAttends = context.UrlTestAttends.AsNoTracking().Where(a => a.UrlTestUrl == urlTest.Url)
				.OrderByDescending(a => a.NumberOfRun).ToList();
			var urlAttend = new UrlTestAttend() {UrlTestUrl = urlTest.Url};
			if (urlAttends.Count == 0)
				urlAttend.NumberOfRun = 1;
			else
				urlAttend.NumberOfRun = urlAttends[0].NumberOfRun + 1;
			context.UrlTestAttends.Add(urlAttend);
			context.SaveChanges();
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == urlTest.Url & a.NumberOfRun == urlAttend.NumberOfRun);
			questionPage.UrlTestAttendId = urlAttendBase.Id;
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
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id)
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

		private UrlTest UrlTestCheck(QuestionPageViewModel userQuestionPage, UrlTestAttend urlAttend)
		{
			var urlTestBase = context.UrlTests.AsNoTracking().First(a => a.Url == urlAttend.UrlTestUrl);
			var urlTest = urlTestBase;
			if (!string.IsNullOrEmpty(userQuestionPage.UrlTestName))
			{
				if (urlTestBase.Name != userQuestionPage.UrlTestName)
				{
					urlTestBase.Name = userQuestionPage.UrlTestName;
					context.Update(urlTestBase);
					context.SaveChanges();
					urlTest = urlTestBase;
				}
			}

			return urlTest;
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
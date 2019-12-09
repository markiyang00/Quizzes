﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzes.Data;
using Quizzes.Data.Model;
using Quizzes.ViewModels;

namespace Quizzes.Controllers
{
	public class UrlController : Controller
	{
		private readonly AppDBContext context;

		public UrlController(AppDBContext context)
		{
			this.context = context;
		}

		public IActionResult CheckAnswer(string url)
		{
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == url);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel);
			var urlTestAttends = context.UrlTestAttends.AsNoTracking().Where(a => a.UrlTestUrl == url)
				.OrderBy(a => a.NumberOfRun).ToList();
			var obj = new UrlAttendsViewModel()
				{Name = urlTest.Name, MaxPoint = questions.Count(), UrlTestAttends = urlTestAttends};
			return View(obj);
		}

		[Route("Url/Exit")]
		[Route("Url/Exit/{url}")]
		public IActionResult Exit(string url)
		{
			if (url != null)
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
								return View(urlModel);
							}
							else
							{
								urlTest.NumberOfRuns--;
								context.Update(urlTest);
								context.SaveChanges();
							}

						return RedirectToAction("QuestionPage", urlTest);
					}

					var urlModel1 = new UrlViewModel {Mes = "Time is overdue ", Url = url};
					return View(urlModel1);
				}
			}

			return View();
		}

		[HttpPost]
		public IActionResult Exit(UrlViewModel urlModel)
		{
			var urlTest = context.UrlTests.FirstOrDefault(a => a.Url == urlModel.Url);
			if (urlTest != null)
			{
				var data = DateTime.Now;
				if (DateTime.Compare(data,urlModel.Time)==-1)
				{
					if (urlTest.NumberOfRuns != null)
						if (urlTest.NumberOfRuns == 0)
						{
							urlModel.Mes = "NumberOfRuns = 0";
							return View(urlModel);
						}
						else
						{
							urlTest.NumberOfRuns--;
							context.Update(urlTest);
							context.SaveChanges();
						}

					return RedirectToAction("QuestionPage", urlTest);
				}

				urlModel.Mes = "Time is overdue ";
				return View(urlModel);
			}

			urlModel.Mes = "Url not Equals";
			return View(urlModel);
		}

		public IActionResult TestUser(UrlTest urlTest)
		{
			var testPage = new TestUserViewModel();
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).ToList();
			var urlAttends = context.UrlTestAttends.AsNoTracking().Where(a => a.UrlTestUrl == urlTest.Url)
				.OrderBy(a => a.NumberOfRun).ToList();
			var urlAttend = new UrlTestAttend() {UrlTestUrl = urlTest.Url};
			if (urlAttends.Count == 0)
				urlAttend.NumberOfRun = 1;
			else
				urlAttend.NumberOfRun = urlAttends[0].NumberOfRun + 1;
			context.UrlTestAttends.Add(urlAttend);
			context.SaveChanges();
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == urlTest.Url & a.NumberOfRun == urlAttend.NumberOfRun);
			var i = 0;
			var answers = new List<Answer>[questions.Count()];
			foreach (var question in questions)
			{
				answers[i] = context.Answers.AsNoTracking().Where(a => a.QuestionId == question.Id).ToList();
				i++;
			}

			testPage.UrlTest = urlTest;
			testPage.UrlTestAttend = urlAttendBase;
			testPage.Test = test;
			testPage.Answers = answers;
			testPage.Questions = questions;
			return View(testPage);
		}

		[HttpPost]
		public IActionResult TestUser(TestUserViewModel userTest)
		{
			var testPage = new TestUserViewModel();
			var urlAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == userTest.UrlTestAttend.Id);
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == urlAttend.UrlTestUrl);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).ToList();
			var i = 0;
			if (ModelState.IsValid)
			{
				foreach (var question in questions)
				{
					foreach (var answer in userTest.Answers[i])
					{
						if (answer.Selected)
						{
							var result = new Result() {AnswerId = answer.Id, UrlTestAttendId = urlAttend.Id};
							context.Results.Add(result);
						}
					}

					i++;
				}

				context.SaveChanges();
				return RedirectToAction("CheckTest", urlAttend);
			}

			var answers = new List<Answer>[questions.Count()];
			foreach (var question in questions)
			{
				answers[i] = context.Answers.AsNoTracking().Where(a => a.QuestionId == question.Id).ToList();
				var j = 0;
				foreach (var answer in answers[i])
				{
					answer.Selected = userTest.Answers[i][j].Selected;
					j++;
				}

				i++;
			}

			testPage.UrlTest = urlTest;
			testPage.Test = test;
			testPage.Answers = answers;
			testPage.Questions = questions;
			return View(testPage);
		}

		public IActionResult QuestionPage(UrlTest urlTest)
		{
			var questionPage=new QuestionPageViewModel();
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).OrderBy(a=>a.Id).ToList();
			var urlAttends = context.UrlTestAttends.AsNoTracking().Where(a => a.UrlTestUrl == urlTest.Url)
				.OrderBy(a => a.NumberOfRun).ToList();
			var urlAttend = new UrlTestAttend() { UrlTestUrl = urlTest.Url };
			if (urlAttends.Count == 0)
				urlAttend.NumberOfRun = 1;
			else
				urlAttend.NumberOfRun = urlAttends[0].NumberOfRun + 1;
			context.UrlTestAttends.Add(urlAttend);
			context.SaveChanges();
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == urlTest.Url & a.NumberOfRun == urlAttend.NumberOfRun);
			var answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == questions[0].Id).ToList();
			questionPage.UrlTest = urlTest;
			questionPage.Answers = answers;
			questionPage.UrlTestAttend = urlAttendBase;
			questionPage.Test = test;
			questionPage.Question = questions[0];
			questionPage.PrevId = -1;
			if (questions[1] != null)
				questionPage.NextId = questions[1].Id;
			else
				questionPage.NextId = -1;

			return View(questionPage);
		}

		[HttpPost]
		public IActionResult QuestionPage(int id, QuestionPageViewModel userQuestionPage)
		{
			var questionPage = new QuestionPageViewModel();
			var urlAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == userQuestionPage.UrlTestAttend.Id);
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == urlAttend.UrlTestUrl);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var urlAttendBase = context.UrlTestAttends.AsNoTracking().First(a =>
				a.UrlTestUrl == urlTest.Url & a.NumberOfRun == urlAttend.NumberOfRun);

			var resultsBase = context.Results.AsNoTracking()
				.Where(a => a.UrlTestAttendId == urlAttendBase.Id).ToList();
			var answersUser = new List<Answer>();
			foreach (var result in resultsBase)
			{
				var answer = context.Answers.FirstOrDefault(a =>
					a.Id == result.AnswerId & a.QuestionId == userQuestionPage.Question.Id);
				if (answer != null)
					answersUser.Add(answer);
			}

			foreach (var answer in userQuestionPage.Answers)
			{
				if (answersUser.Count == 0)
				{
					if (answer.Selected)
					{
						var result = new Result() {AnswerId = answer.Id, UrlTestAttendId = urlAttend.Id};
						context.Results.Add(result);
					}
				}
				else
				{
					if (answersUser.FirstOrDefault(a => a.Id == answer.Id) == null)
					{
						if (answer.Selected)
						{
							var result = new Result() {AnswerId = answer.Id, UrlTestAttendId = urlAttend.Id};
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
			if (id == -2)
			{
				return RedirectToAction("CheckTest", urlAttend);
			}

			questionPage.Test = test;
			questionPage.UrlTest = urlTest;
			questionPage.UrlTestAttend = urlAttendBase;
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id).OrderBy(a => a.Id)
				.ToList();
			var i = 0;
			foreach (var question in questions)
			{
				if (question.Id == id)
					break;
				i++;
			}

			questionPage.Answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == id).ToList();
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
					questionPage.Answers.Find(a => a.Id == answer.Id).Selected = true;
				}
			}

			questionPage.Question = questions[i];
			if (id == userQuestionPage.NextId)
			{
				questionPage.PrevId = questions[i-1].Id;
				if(questions.Count-1==i)
					questionPage.NextId = -1;
				else
					questionPage.NextId = 3;
					//questionPage.NextId = questions[i + 1].Id;
			}
			else
			{
				questionPage.NextId = questions[i+1].Id;
				if (i!=0)
					questionPage.PrevId = questions[i - 1].Id;
				else
					questionPage.PrevId = -1;
			}

			return View(questionPage);

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
				Name = urlTest.Name, Point = urlTestAttend.Point, MaxPoint = questions.Count,
				NumberOfRun = urlTestAttend.NumberOfRun
			};
			return View(urlResult);
		}
	}
}
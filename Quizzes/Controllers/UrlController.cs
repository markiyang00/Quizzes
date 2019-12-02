using System;
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
	public class UrlController:Controller
	{
		private readonly AppDBContext context;

		public UrlController(AppDBContext context)
		{
			this.context = context;
		}

		public IActionResult Check(string url)
		{
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == url);
			return RedirectToAction("CheckTest", urlTest);
		}

		public ViewResult Exit()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Exit(UrlViewModel urlModel)
		{
			var urlTest = context.UrlTests.FirstOrDefault(a => a.Url == urlModel.Url);
			if (urlTest != null)
			{
				if (urlTest.NumberOfRuns!=null)
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

				return RedirectToAction("TestUser", urlTest);
			}

			urlModel.Mes = "Url not Equals";
			return View(urlModel);
		}

		public IActionResult TestUser(UrlTest urlTest)
		{
			var testPage=new TestUserViewModel();
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).ToList();
			var i = 0;
			var answers = new List<Answer>[questions.Count()];
			foreach (var question in questions)
			{
				answers[i] = context.Answers.AsNoTracking().Where(a => a.QuestionId == question.Id).ToList();
				i++;
			}

			testPage.UrlTest=urlTest;
			testPage.Test = test;
			testPage.Answers = answers;
			testPage.Questions = questions;
			return View(testPage);
		}

		[HttpPost]
		public IActionResult TestUser(TestUserViewModel userTest)
		{
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == userTest.UrlTest.Url);
			var testPage = new TestUserViewModel();
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
							var result=new Result(){AnswerId = answer.Id,UrlTestUrl = urlTest.Url};
							context.Results.Add(result);
						}
					}
					i++;
				}

				context.SaveChanges();
				return RedirectToAction("CheckTest", urlTest);
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
		public IActionResult CheckTest(UrlTest urlTest)
		{
			var point = 0;
			var results = context.Results.AsNoTracking().Where(a => a.UrlTestUrl == urlTest.Url).ToList();
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).OrderBy(a=>a.Id).ToList();
			var answers=new List<Answer>();
			foreach (var result in results)
			{
				answers.Add(context.Answers.AsNoTracking().First(a=>a.Id==result.AnswerId));
			}

			foreach (var question in questions)
			{
				var answerQuestion = answers.Where(a => a.QuestionId == question.Id).ToList();
				var trueAnswer = true;
				foreach (var answer in answerQuestion)
				{
					if (!answer.True)
						trueAnswer = false;
				}

				if (trueAnswer)
				{
					point++;
				}
			}


			urlTest.Point = point;
			context.Update(urlTest);
			context.SaveChanges();
			var urlResult=new UrlResultViewModel(){UrlTest = urlTest,MaxPoint = questions.Count};
			return View(urlResult);
		}

	}
}
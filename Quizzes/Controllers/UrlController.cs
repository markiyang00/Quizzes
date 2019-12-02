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
	public class UrlController:Controller
	{
		private readonly AppDBContext context;

		public UrlController(AppDBContext context)
		{
			this.context = context;
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
			var results2 = context.Results.AsNoTracking().Where(a => a.UrlTestUrl == urlTest.Url).ToList();
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).ToList();
			var answers=new List<Answer>();
			return View();
		}

	}
}
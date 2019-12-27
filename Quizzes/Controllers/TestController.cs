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
	public class TestController:Controller
	{
		
		private readonly AppDBContext context;

		public TestController(AppDBContext context)
		{
			this.context = context;
		}

		public IActionResult AddTest()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AddTest(TestUpdatedViewModel testModel)
		{
			if (ModelState.IsValid)
			{
				context.Tests.Add(testModel.Test);
				context.SaveChanges();
			}
			return View(testModel);
		}

		public IActionResult TestPage(Test test)
		{
			var testPage = new TestPageViewModel() { Test = test };
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel).ToList();
			var i = 0;
			var answers= new List<Answer>[questions.Count()];
			foreach (var question in questions)
			{
				answers[i] = context.Answers.AsNoTracking().Where(a => a.QuestionId == question.Id).ToList();
				i++;
			}

			testPage.Answers = answers;
			testPage.Questions = questions;
			return View(testPage);
		}

		public IActionResult AddQuestions(int testId)
		{
			var question=new Question(){TestId = testId};
			return View(question);
		}

		[HttpPost]
		public IActionResult AddQuestions(Question question)
		{
			if (ModelState.IsValid)
			{
				context.Questions.Add(question);
				context.SaveChanges();
				var testBase = context.Tests.AsNoTracking().First(a => a.Id==question.TestId & !a.IsDel);
				return RedirectToAction("TestPage", testBase);
			}
			return View(question);
		}

		public IActionResult AddAnswer(int id)
		{
			var answer = new Answer() {QuestionId = id};
			return View(answer);
		}

		[HttpPost]
		public IActionResult AddAnswer(Answer answer)
		{
			if (ModelState.IsValid)
			{
				context.Answers.Add(answer);
				context.SaveChanges();
				var question = context.Questions.First(a => a.Id == answer.QuestionId & !a.IsDel);
				var testBase = context.Tests.AsNoTracking().First(a => a.Id == question.TestId & !a.IsDel);
				return RedirectToAction("TestPage", testBase);
			}
			return View(answer);
		}

		public IActionResult UpdatedTest(int id)
		{
			var testBase = context.Tests.AsNoTracking().First(a => a.Id == id & !a.IsDel);
			var obg = new TestUpdatedViewModel
			{
				Test = testBase,
				Questions = context.Questions.AsNoTracking().Where(a => a.TestId == id & !a.IsDel).ToList()
			};
			return View(obg);
		}

		[HttpPost]
		public IActionResult UpdatedTest(TestUpdatedViewModel testModel)
		{
			if (testModel.Test.Id == 0)
			{
				context.Tests.Add(testModel.Test);
				context.SaveChanges();
			}
			else
			{
				context.Update(testModel.Test);
				context.SaveChanges();
			}

			testModel.Test = context.Tests.AsNoTracking().FirstOrDefault(a => a.Id == testModel.Test.Id & !a.IsDel);
			testModel.Questions = context.Questions.AsNoTracking().Where(a => a.TestId == testModel.Test.Id & !a.IsDel)
				.ToList();
			return View(testModel);
		}

		public IActionResult UpdatedQuestions(int id)
		{
			var question = context.Questions.First(a => a.Id == id & !a.IsDel);
			var obg = new QuestionUpdatedViewModel
			{
				Question = question,
				Answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == id).ToList()
			};
			return View(obg);
		}

		[HttpPost]
		public IActionResult UpdatedQuestions(QuestionUpdatedViewModel questionModel)
		{
			if (ModelState.IsValid)
			{
				context.Update(questionModel.Question);
				context.SaveChanges();
			}

			questionModel.Answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == questionModel.Question.Id).ToList();
			return View(questionModel);
		}

		public IActionResult UpdatedAnswer(int id)
		{
			var answer = context.Answers.AsNoTracking().First(a => a.Id == id);
			return View(answer);
		}

		[HttpPost]
		public IActionResult UpdatedAnswer(Answer answer)
		{
			if (ModelState.IsValid)
			{
				context.Update(answer);
				context.SaveChanges();
			}
			return View(answer);
		}

		public IActionResult DelTest(int id)
		{
			var elem = context.Tests.AsNoTracking().First(p => p.Id == id);
			elem.IsDel = true;
			context.Update(elem);
			context.SaveChanges();
			return RedirectToAction("Admin","Admin",elem);
		}

		public IActionResult DelQuestions(int id)
		{
			var elem = context.Questions.AsNoTracking().First(p => p.Id == id);
			elem.IsDel = true;
			context.Update(elem);
			context.SaveChanges();
			var testBase = context.Tests.AsNoTracking().First(a => a.Id == elem.TestId & !a.IsDel);
			return RedirectToAction("TestPage", testBase);
		}

		public IActionResult DelAnswer(int id)
		{
			var elem = context.Answers.AsNoTracking().First(p => p.Id == id);
			context.Remove(elem);
			context.SaveChanges();
			var question = context.Questions.First(a => a.Id == elem.QuestionId & !a.IsDel);
			var testBase = context.Tests.AsNoTracking().First(a => a.Id == question.TestId & !a.IsDel);
			return RedirectToAction("TestPage", testBase);
		}

	}
}
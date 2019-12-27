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

		public IActionResult AddQuestions(int testId)
		{
			var question=new Question(){TestId = testId};
			var obg = new QuestionUpdatedViewModel
			{
				Question = question
			};
			return View(obg);
		}


		public IActionResult AddAnswer(int id)
		{
			var answer = new Answer() {QuestionId = id};
			return View(answer);
		}

		[Route("Test/UpdatedTest/{id}")]
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
			context.Update(testModel.Test);
			context.SaveChanges();
			testModel.Test = context.Tests.AsNoTracking().FirstOrDefault(a => a.Id == testModel.Test.Id & !a.IsDel);
			testModel.Questions = context.Questions.AsNoTracking().Where(a => a.TestId == testModel.Test.Id & !a.IsDel)
				.ToList();
			return View(testModel);
		}

		[Route("Test/UpdatedQuestions/{id}")]
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
			if (questionModel.Question.Id == 0)
			{
				context.Questions.Add(questionModel.Question);
				context.SaveChanges();
				return RedirectToAction("UpdatedTest", new { id = questionModel.Question.TestId });
			}
			context.Update(questionModel.Question);
			context.SaveChanges();
			questionModel.Answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == questionModel.Question.Id).ToList();
			return View(questionModel);
		}

		[Route("Test/UpdatedAnswer/{id}")]
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
				if (answer.Id == 0)
				{
					context.Answers.Add(answer);
					context.SaveChanges();
					return RedirectToAction("UpdatedQuestions", new { id = answer.QuestionId });
				}
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
			return RedirectToAction("Admin","Admin");
		}

		public IActionResult DelQuestions(int id)
		{
			var elem = context.Questions.AsNoTracking().First(p => p.Id == id);
			elem.IsDel = true;
			context.Update(elem);
			context.SaveChanges();
			return RedirectToAction("UpdatedTest", new { id = elem.TestId});
		}

		public IActionResult DelAnswer(int id)
		{
			var elem = context.Answers.AsNoTracking().First(p => p.Id == id);
			context.Remove(elem);
			context.SaveChanges();
			return RedirectToAction("UpdatedQuestions",new{id=elem.QuestionId});
		}
	}
}
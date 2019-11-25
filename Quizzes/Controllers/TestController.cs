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
		private TestPageViewModel testPage;

		public TestController(AppDBContext context)
		{
			this.context = context;
			testPage=new TestPageViewModel();
		}

		public IActionResult AddTest()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AddTest(Test test)
		{
			if (ModelState.IsValid)
			{
				context.Tests.Add(test);
				context.SaveChanges();
				var testBase = context.Tests.AsNoTracking().First(a => a.Name == test.Name);
				testPage=new TestPageViewModel(){Test = testBase};
				return RedirectToAction("TestPage",testBase);
			}
			return View(test);
		}

		public IActionResult TestPage(Test test)
		{
			testPage.Test = test;
			return View(testPage);
		}

		public IActionResult AddQuestions()
		{
			return View();
		}

		[HttpPost]
		public IActionResult AddQuestions(Question question)
		{
			if (ModelState.IsValid)
			{
				question.TestId = testPage.Test.Id;
				context.Questions.Add(question);
				context.SaveChanges();
				var questionBase = context.Questions.AsNoTracking()
					.First(a => a.Text == question.Text & a.TestId == question.TestId &
					            a.TrueAnswer == question.TrueAnswer);
				if (testPage.Questions!=null)
				{
					testPage.Questions.Add(questionBase);
				}
				else
				{
					testPage.Questions = new List<Question> {questionBase};
				}

				return RedirectToAction("TestPage", testPage.Test);
			}
			return View(question);
		}

		public IActionResult AddAnswer(int index)
		{
			var obj=new AnswerViewModel(){Index = index};
			return View(obj);
		}

		[HttpPost]
		public IActionResult AddAnswer(AnswerViewModel answer)
		{
			if (ModelState.IsValid)
			{
				context.Answers.Add(answer.Answer);
				context.SaveChanges();
				var answerBase = context.Answers.AsNoTracking()
					.First(a => a.Text == answer.Answer.Text);
				if (testPage.Answers[answer.Index] != null)
				{
					testPage.Answers[answer.Index].Add(answerBase);
				}
				else
				{
					testPage.Answers[answer.Index] = new List<Answer> {answerBase};
				}
				
				return RedirectToAction("TestPage", testPage.Test);
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

	}
}
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzes.Data;
using Quizzes.Data.Model;
using Quizzes.ViewModels;

namespace Quizzes.Controllers
{
	public class QuestionController:Controller
	{
		private readonly AppDBContext context;

		public QuestionController(AppDBContext context)
		{
			this.context = context;
		}

		public IActionResult Add(int testId)
		{
			var question = new Question() { TestId = testId };
			var obg = new QuestionUpdatedViewModel
			{
				Question = question
			};
			return View(obg);
		}

		[Route("Question/Edit/{id}")]
		public IActionResult Edit(int id)
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
		public IActionResult Edit(QuestionUpdatedViewModel questionModel)
		{
			if (questionModel.Question.Id == 0)
			{
				context.Questions.Add(questionModel.Question);
				context.SaveChanges();
				return RedirectToAction("Edit","Test", new { id = questionModel.Question.TestId });
			}
			context.Update(questionModel.Question);
			context.SaveChanges();
			questionModel.Answers = context.Answers.AsNoTracking().Where(a => a.QuestionId == questionModel.Question.Id).ToList();
			return View(questionModel);
		}

		public IActionResult Del(int id)
		{
			var elem = context.Questions.AsNoTracking().First(p => p.Id == id);
			elem.IsDel = true;
			context.Update(elem);
			context.SaveChanges();
			return RedirectToAction("Edit", "Test", new { id = elem.TestId });
		}
	}
}
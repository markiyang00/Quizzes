using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzes.Data;
using Quizzes.Data.Model;

namespace Quizzes.Controllers
{
	public class AnswerController:Controller
	{
		private readonly AppDBContext context;

		public AnswerController(AppDBContext context)
		{
			this.context = context;
		}

		public IActionResult Add(int id)
		{
			var answer = new Answer() { QuestionId = id };
			return View(answer);
		}

		[Route("Answer/Edit/{id}")]
		public IActionResult Edit(int id)
		{
			var answer = context.Answers.AsNoTracking().First(a => a.Id == id & !a.IsDel);
			return View(answer);
		}

		[Route("Answer/Edit")]
		[Route("Answer/Edit/{id}")]
		[HttpPost]
		public IActionResult Edit(int id,Answer answer)
		{
			if (ModelState.IsValid)
			{
				if (answer.Id == 0)
				{
					context.Answers.Add(answer);
					context.SaveChanges();
					return RedirectToAction("Edit","Question", new { id = answer.QuestionId });
				}
				context.Update(answer);
				context.SaveChanges();
			}
			return View(answer);
		}


		public IActionResult Del(int id)
		{
			var elem = context.Answers.AsNoTracking().First(p => p.Id == id);
			elem.IsDel = true;
			context.Update(elem);
			context.SaveChanges();
			return RedirectToAction("Edit", "Question", new { id = elem.QuestionId });
		}
	}
}
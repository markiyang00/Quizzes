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

		public IActionResult Add()
		{
			return View();
		}

		[Route("Test/Edit/{id}")]
		public IActionResult Edit(int id)
		{
			var testBase = context.Tests.AsNoTracking().First(a => a.Id == id & !a.IsDel);
			var obg = new TestUpdatedViewModel
			{
				Test = testBase,
				Questions = context.Questions.AsNoTracking().Where(a => a.TestId == id & !a.IsDel).ToList(),
				ImgDel = "/img/Delete.jpg",
				ImgEdit = "/img/Edit.jpg"
			};
			return View(obg);
		}

		[HttpPost]
		public IActionResult Edit(TestUpdatedViewModel testModel)
		{
			if (ModelState.IsValid)
			{
				context.Update(testModel.Test);
				context.SaveChanges();
			}

			testModel.Test = context.Tests.AsNoTracking().FirstOrDefault(a => a.Id == testModel.Test.Id & !a.IsDel);
				testModel.Questions = context.Questions.AsNoTracking()
					.Where(a => a.TestId == testModel.Test.Id & !a.IsDel)
					.ToList();
				return View(testModel);
		}

		public IActionResult Del(int id)
		{
			var elem = context.Tests.AsNoTracking().First(p => p.Id == id);
			elem.IsDel = true;
			context.Update(elem);
			context.SaveChanges();
			return RedirectToAction("Admin","Admin");
		}

	}
}
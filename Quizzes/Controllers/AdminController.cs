using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quizzes.Data;
using Quizzes.Data.Model;
using Quizzes.ViewModels;

namespace Quizzes.Controllers
{
	public class AdminController:Controller
	{
		private readonly AppDBContext context;

		public AdminController(AppDBContext context)
		{
			this.context = context;
		}

		public ViewResult Exit()
		{
			//var admin=new Admin(){Name = "mark",Password = "007"};
			//context.Admins.Add(admin);
			//context.SaveChanges();
			return View();
		}

		[HttpPost]
		public IActionResult Exit(AdminViewModel admin)
		{
			var adminBase = context.Admins.FirstOrDefault(a => a.Name == admin.Name && a.Password == admin.Password);
			if (adminBase != null)
			{
				return RedirectToAction("Admin", adminBase);
			}

			admin.Mes = "Password or Name not Equals";
			return View(admin);
		}
		public ViewResult Admin(Admin admin)
		{
			var tests = context.Tests.Where(a=>!a.IsDel).ToList();
			var obg=new AdminTestViewModel(){Admin = admin,Tests = tests};
			return View(obg);
		}

		public IActionResult AddUrlTest(int id)
		{
			var urlTest = new UrlTest() { TestId = id };
			return View(urlTest);
		}

		[HttpPost]
		public IActionResult AddUrlTest(UrlTest urlTest)
		{
			if (ModelState.IsValid)
			{
				context.UrlTests.Add(urlTest);
				context.SaveChanges();
				return RedirectToAction("Admin");
			}
			return View(urlTest);
		}

		public IActionResult AllUrlTest(int id)
		{
			var urlTests = context.UrlTests.Where(a => a.TestId == id).ToList();
			var obj=new AllUrlViewModel(){UrlTests = urlTests};
			return View(obj);
		}
	}
}
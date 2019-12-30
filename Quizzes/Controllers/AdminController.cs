using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzes.Data;
using Quizzes.Data.Model;
using Quizzes.ViewModels;

namespace Quizzes.Controllers
{
	public class AdminController : Controller
	{
		private readonly AppDBContext context;

		public AdminController(AppDBContext context)
		{
			this.context = context;
		}

		public ViewResult Exit()
		{
			//var admin = new Admin() { Name = "mark", Password = "007" };
			//context.Admins.Add(admin);
			//context.SaveChanges();
			return View();
		}

		[HttpPost]
		public IActionResult Exit(AdminViewModel admin)
		{
			if (ModelState.IsValid)
			{
				var adminBase =
					context.Admins.FirstOrDefault(a => a.Name == admin.Name && a.Password == admin.Password);
				if (adminBase != null)
				{
					return RedirectToAction("Admin");
				}

				admin.Mes = "Password or Name not Equals";
			}

			return View(admin);
		}

		public ViewResult Admin(Admin admin)
		{
			var tests = context.Tests.Where(a => !a.IsDel).ToList();
			var obg = new AdminTestViewModel() {Admin = admin, Tests = tests,ImgEdit = "/img/Edit.jpg" ,ImgDel = "/img/Delete.jpg" };
			return View(obg);
		}

		public IActionResult CreateUrlTest(string url)
		{
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == url);
			return View(urlTest);
		}

		public IActionResult AddUrlTest(int id)
		{
			var urlTest = new UrlTest() {TestId = id};
			return View(urlTest);
		}

		[HttpPost]
		public IActionResult AddUrlTest(UrlTest urlTest)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrEmpty(urlTest.Url))
				{
					urlTest.Url = Guid.NewGuid().ToString();
				}

				context.UrlTests.Add(urlTest);
				context.SaveChanges();
				return RedirectToAction("AddUrlTestUrl", urlTest);
			}

			return View(urlTest);
		}

		public IActionResult AddUrlTestUrl(UrlTest urlTest)
		{
			return View(urlTest);
		}

		public IActionResult AllUrlTest(int id)
		{
			var urlTests = context.UrlTests.Where(a => a.TestId == id).ToList();
			var obj = new AllUrlViewModel() {UrlTests = urlTests};
			return View(obj);
		}
	}
}
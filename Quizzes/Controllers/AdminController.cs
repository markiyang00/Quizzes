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


	}
}
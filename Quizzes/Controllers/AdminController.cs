using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quizzes.Data;
using Quizzes.ViewModals;

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
			var AdminBase = context.Admins.FirstOrDefault(a => a.Name == admin.Name && a.Password == admin.Password);
			if (AdminBase != null)
			{
				return RedirectToAction("Admin", AdminBase);
			}

			admin.Mes = "Password or Name not Equals";
			return View(admin);
		}
	}
}
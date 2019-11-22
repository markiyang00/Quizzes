using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzes.Data;

namespace Quizzes.Controllers
{
	public class TestController:Controller
	{
		
		private readonly AppDBContext context;

		public TestController(AppDBContext context)
		{
			this.context = context;
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
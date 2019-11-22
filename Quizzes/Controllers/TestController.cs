using Microsoft.AspNetCore.Mvc;
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



	}
}
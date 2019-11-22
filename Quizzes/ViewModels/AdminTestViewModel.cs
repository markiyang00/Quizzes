using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class AdminTestViewModel
	{
		public Admin Admin { get; set; }
		public List<Test> Tests { get; set; }
	}
}
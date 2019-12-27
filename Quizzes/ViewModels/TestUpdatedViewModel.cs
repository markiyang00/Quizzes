using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class TestUpdatedViewModel
	{
		public Test Test { get; set; }
		public List<Question> Questions { get; set; }
	}
}
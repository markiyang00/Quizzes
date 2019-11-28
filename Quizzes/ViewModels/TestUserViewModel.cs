using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class TestUserViewModel
	{
		public Test Test { get; set; }
		public List<Question> Questions { get; set; }
		public List<Answer>[] Answers { get; set; }
		public List<Result>[] Results { get; set; }
	}
}
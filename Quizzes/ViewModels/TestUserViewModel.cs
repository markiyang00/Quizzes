using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class TestUserViewModel
	{
		public Test Test { get; set; }
		public UrlTest UrlTest { get; set; }
		public UrlTestAttend UrlTestAttend { get; set; }
		public List<Question> Questions { get; set; }
		public List<Answer>[] Answers { get; set; }
	}
}
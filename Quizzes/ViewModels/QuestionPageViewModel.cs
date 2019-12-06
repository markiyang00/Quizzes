using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class QuestionPageViewModel
	{
		public Question Question { get; set; }
		public Question PrevQuestion { get; set; }
		public Question NextQuestion { get; set; }
		public List<Answer> Answers { get; set; }
		public Test Test { get; set; }
		public UrlTest UrlTest { get; set; }
		public UrlTestAttend UrlTestAttend { get; set; }
	}
}
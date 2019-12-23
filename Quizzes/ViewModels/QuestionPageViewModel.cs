using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class QuestionPageViewModel
	{
		public Question Question { get; set; }
		public int QuestionId { get; set; }
		public int PrevId { get; set; }
		public int NextId { get; set; }
		public List<Answer> Answers { get; set; }
		public int TestId { get; set; }
		public string TestName { get; set; }
		public string UrlTestName { get; set; }
		public int UrlTestAttendId { get; set; }
	}
}
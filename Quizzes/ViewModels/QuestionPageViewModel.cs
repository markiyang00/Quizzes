using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class QuestionPageViewModel
	{
		public Question Question { get; set; }
		public int QuestionId { get; set; }
		public int? PrevId{ get; set; }
		public int? NextId { get; set; }
		public List<Answer> Answers { get; set; }
		public string Mes { get; set; }
		public string TestName { get; set; }
		public string UrlTestName { get; set; }
		public string Url { get; set; }
		public int UrlTestAttendId { get; set; }
	}
}
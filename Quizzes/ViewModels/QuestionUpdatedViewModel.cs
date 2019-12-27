using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class QuestionUpdatedViewModel
	{
		public Question Question { get; set; }
		public List<Answer> Answers { get; set; }
	}
}
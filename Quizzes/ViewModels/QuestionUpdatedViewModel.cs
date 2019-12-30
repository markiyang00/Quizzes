using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class QuestionUpdatedViewModel
	{
		public Question Question { get; set; }
		public string ImgDel { get; set; }
		public string ImgEdit { get; set; }
		public List<Answer> Answers { get; set; }
	}
}
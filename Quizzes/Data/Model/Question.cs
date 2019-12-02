using System.Collections.Generic;

namespace Quizzes.Data.Model
{
	public class Question
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public bool IsDel { get; set; }
		public int TestId { get; set; }
		public List<Answer> Replies { get; set; }
		public virtual Test Test { get; set; }

	}
}
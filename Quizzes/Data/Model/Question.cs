using System.Collections.Generic;

namespace Quizzes.Data.Model
{
	public class Question
	{
		public int Id { get; set; }
		public string Questions { get; set; }
		public string TrueValue { get; set; }
		public bool IsDel { get; set; }
		public List<Reply> Replies { get; set; }
		public virtual Test Test { get; set; }

	}
}
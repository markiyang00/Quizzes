using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quizzes.Data.Model
{
	public class Question
	{
		public int Id { get; set; }
		[Display(Name = "Text")]
		[StringLength(50)]
		[Required(ErrorMessage = "Lenght should be short 50")]
		public string Text { get; set; }
		public bool IsDel { get; set; }
		public int TestId { get; set; }
		public List<Answer> Replies { get; set; }
		public virtual Test Test { get; set; }

	}
}
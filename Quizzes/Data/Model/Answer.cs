using System.ComponentModel.DataAnnotations;

namespace Quizzes.Data.Model
{
	public class Answer
	{
		public int Id { get; set; }
		[Display(Name = "Text")]
		[StringLength(100)]
		[Required(ErrorMessage = "Lenght should be short 100")]
		public string Text { get; set; }
		public bool Selected { get; set; }
		public bool True { get; set; }
		public bool IsDel { get; set; }
		public int QuestionId { get; set; }
		public virtual Question Question { get; set; }

	}
}
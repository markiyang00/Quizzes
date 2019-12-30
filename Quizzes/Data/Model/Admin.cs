using System.ComponentModel.DataAnnotations;

namespace Quizzes.Data.Model
{
	public class Admin
	{
		public int Id { get; set; }
		[Display(Name = "Name")]
		[StringLength(10)]
		[Required(ErrorMessage = "Lenght should be short 10")]

		public string Name { get; set; }
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		[StringLength(10)]
		[Required(ErrorMessage = "Lenght should be short 10")]

		public string Password { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quizzes.Data.Model
{
	public class Test
	{
		public int Id { get; set; }
		[Display(Name = "Name")]
		[StringLength(30)]
		[Required(ErrorMessage = "Lenght should be short 30")]
		public string Name { get; set; }
		public bool IsDel { get; set; }
		[Display(Name = "Test Time")]
		public TimeSpan TestTime { get; set; }
		public List<Question> Questions { get; set; }
		public List<UrlTest> UrlTests { get; set; }


	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quizzes.Data.Model
{
	public class UrlTest
	{
		[Key]
		public string Url { get; set; }
		public string Name { get; set; }
		public DateTime Time { get; set; }
		public int? NumberOfRuns {get; set; }
		public int TestId { get; set; }
		public List<UrlTestAttend> UrlTestAttends { get; set; }
		public virtual Test Test { get; set; }
	}
}
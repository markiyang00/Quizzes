using System;
using System.Collections.Generic;

namespace Quizzes.Data.Model
{
	public class Test
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsDel { get; set; }
		public TimeSpan TestTime { get; set; }
		public List<Question> Questions { get; set; }
		public List<UrlTest> UrlTests { get; set; }


	}
}
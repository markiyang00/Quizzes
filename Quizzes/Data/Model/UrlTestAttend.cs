using System;
using System.Collections.Generic;

namespace Quizzes.Data.Model
{
	public class UrlTestAttend
	{
		public int Id { get; set; }
		public int NumberOfRun { get; set; }
		public int Point { get; set; }
		public DateTime StartTimeTest { get; set; }
		public TimeSpan TestTime { get; set; }
		public string UrlTestUrl { get; set; }
		public List<Result> Results { get; set; }
		public virtual UrlTest UrlTest{ get; set; }
	}
}
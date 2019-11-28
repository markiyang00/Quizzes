using System;

namespace Quizzes.ViewModels
{
	public class UrlViewModel
	{
		public string Url { get; set; }
		public string Name { get; set; }

		public DateTime Time { get; set; }
		public int? NumberOfRuns { get; set; }
		public int TestId { get; set; }
		public string Mes { get; set; }
	}
}
using System;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class UrlResultViewModel
	{
		public string Url { get; set; }
		public string Name { get; set; }
		public int Point { get; set; }
		public int MaxPoint { get; set; }
		public TimeSpan TestTime { get; set; }
		public int NumberOfRun { get; set; }
	}
}
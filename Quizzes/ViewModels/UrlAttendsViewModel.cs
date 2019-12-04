using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class UrlAttendsViewModel
	{
		public string Name { get; set; }
		public int MaxPoint { get; set; }
		public List<UrlTestAttend> UrlTestAttends { get; set; }
	}
}
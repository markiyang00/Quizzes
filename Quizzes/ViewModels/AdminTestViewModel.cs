using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class AdminTestViewModel
	{
		public Admin Admin { get; set; }
		public string ImgEdit { get; set; }
		public string ImgDel { get; set; }
		public List<Test> Tests { get; set; }
	}
}
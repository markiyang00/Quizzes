﻿using System.Collections.Generic;
using Quizzes.Data.Model;

namespace Quizzes.ViewModels
{
	public class TestPageViewModel
	{
		public Test Test { get; set; }
		public List<Question> Questions { get; set; }
		public List<Answer>[] Answers { get; set; }
	}
}
﻿namespace Quizzes.Data.Model
{
	public class Answer
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public bool Selected { get; set; }
		public bool True { get; set; }
		public int QuestionId { get; set; }
		public virtual Question Question { get; set; }

	}
}
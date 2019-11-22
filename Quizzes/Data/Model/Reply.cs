﻿namespace Quizzes.Data.Model
{
	public class Reply
	{
		public int Id { get; set; }

		public bool Selected { get; set; }
		public bool IsDel { get; set; }
		public virtual Question Question { get; set; }

	}
}
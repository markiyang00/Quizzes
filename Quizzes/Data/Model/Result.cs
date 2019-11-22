namespace Quizzes.Data.Model
{
	public class Result
	{
		public int Id { get; set; }

		public string Reply { get; set; }
		public bool IsDel { get; set; }

		public virtual UrlTest UrlTest { get; set; }
	}
}
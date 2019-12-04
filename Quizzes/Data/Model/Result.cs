namespace Quizzes.Data.Model
{
	public class Result
	{
		public int Id { get; set; }

		public int AnswerId { get; set; }
		public int UrlTestAttendId { get; set; }
		
		public virtual UrlTest UrlTest { get; set; }
	}
}
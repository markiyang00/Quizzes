using Microsoft.EntityFrameworkCore;
using Quizzes.Data.Model;

namespace Quizzes.Data
{
	public class AppDBContext:DbContext
	{
		public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
		{

		}

		public DbSet<Admin> Admins{get;set;}
		public DbSet<Test> Tests {get;set;}
		public DbSet<Question> Questions {get;set;}
		public DbSet<Answer> Answers {get;set;}
		public DbSet<UrlTest> UrlTests {get;set;}
		public DbSet<UrlTestAttend> UrlTestAttends {get;set;}
		public DbSet<Result> Results {get;set;}
	}
}
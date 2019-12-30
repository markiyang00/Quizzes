using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizzes.Data;
using Quizzes.Data.Model;
using Quizzes.ViewModels;

namespace Quizzes.Controllers
{
	public class ResultController:Controller
	{
		private readonly AppDBContext context;

		public ResultController(AppDBContext context)
		{
			this.context = context;
		}

		public IActionResult CheckAnswers(string url, string user)
		{
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == url);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel);
			var urlTestAttends = context.UrlTestAttends.AsNoTracking().Where(a => a.UrlTestUrl == url)
				.OrderBy(a => a.NumberOfRun).ToList();
			var obj = new UrlAttendsViewModel()
				{ Name = urlTest.Name, User = user, MaxPoint = questions.Count(), UrlTestAttends = urlTestAttends };
			return View(obj);
		}

		[Route("Result/CheckTest/{testAttemptId}")]
		public IActionResult CheckTest(int testAttemptId)
		{
			var urlTestAttend = context.UrlTestAttends.AsNoTracking().First(a => a.Id == testAttemptId);
			var point = 0;
			var results = context.Results.AsNoTracking().Where(a => a.UrlTestAttendId == urlTestAttend.Id).ToList();
			var urlTest = context.UrlTests.AsNoTracking().First(a => a.Url == urlTestAttend.UrlTestUrl);
			var test = context.Tests.AsNoTracking().First(a => a.Id == urlTest.TestId & !a.IsDel);
			var questions = context.Questions.AsNoTracking().Where(a => a.TestId == test.Id & !a.IsDel)
				.OrderBy(a => a.Id).ToList();
			var answers = new List<Answer>();

			foreach (var result in results)
			{
				answers.Add(context.Answers.AsNoTracking().First(a => a.Id == result.AnswerId));
			}

			foreach (var question in questions)
			{
				var answersBase = context.Answers.AsNoTracking().Where(a => a.QuestionId == question.Id).ToList();
				var answerQuestion = answers.Where(a => a.QuestionId == question.Id).ToList();
				var trueAnswer = true;
				var trueAnswerCount = 0;
				foreach (var answer in answersBase)
					if (answer.True)
						trueAnswerCount++;

				if (trueAnswerCount == answerQuestion.Count)
				{
					foreach (var answer in answerQuestion)
						if (!answer.True)
						{
							trueAnswer = false;
							break;
						}
				}
				else
					trueAnswer = false;

				if (trueAnswer)
				{
					point++;
				}
			}


			urlTestAttend.Point = point;
			context.Update(urlTestAttend);
			context.SaveChanges();
			var urlResult = new UrlResultViewModel()
			{
				Url = urlTest.Url,
				Name = urlTest.Name,
				Point = urlTestAttend.Point,
				MaxPoint = questions.Count,
				NumberOfRun = urlTestAttend.NumberOfRun,
				TestTime = urlTestAttend.TestTime
			};
			return View(urlResult);
		}
	}
}
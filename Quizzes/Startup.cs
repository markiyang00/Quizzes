using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quizzes.Data;

namespace Quizzes
{
	public class Startup
	{
		private IConfigurationRoot confSting;
		public Startup(IHostingEnvironment hostEnv)
		{
			confSting = new ConfigurationBuilder().SetBasePath(hostEnv.ContentRootPath).AddJsonFile("dbsettings.json").Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddDbContext<AppDBContext>(options =>
				options.UseSqlServer(confSting.GetConnectionString("DefaultConnection")));
		

			services.AddMvc();
			services.AddMemoryCache();
			services.AddSession();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseDeveloperExceptionPage();
			app.UseStatusCodePages();
			app.UseStaticFiles();
			app.UseSession();
			app.UseMvc(routes =>
			{
				routes.MapRoute("default", "{controller=Admin}/{action=Exit}");
				routes.MapRoute("userExit", "{controller=Url}/{action=Exit}/{url?}");
			//	routes.MapRoute(
			//		name: "Url",
			//		url: "{controller}/{action}/{id}",
			//		defaults: new
			//		{
			//			controller = "Url",
			//			action = "Index",
			//			id = UrlParameter.Optional
			//		},
			//	);
			});
		}
	}
}

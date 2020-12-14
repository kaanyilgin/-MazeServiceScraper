using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Config;
using MazeServiceScraper.Infrastructure.Database;
using MazeServiceScraper.Infrastructure.MazeWebService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MazeServiceScraper.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddHttpClient();

			services.AddSingleton<IMazeService, MazeService>();
			services.AddScoped<IShowRepository, ShowRepository>();

			services.AddOptions();

			services.Configure<MazeServiceConfig>(Configuration.GetSection("MazeService"));
			services.Configure<MazeCacheConfig>(Configuration.GetSection("MazeCacheConfig"));

			services.AddDbContext<MazeDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MazeConnectionString")));

			services.AddScoped<ShowApplication>();
			services.AddScoped<CachedShowApplication>(provider => new CachedShowApplication(provider.GetRequiredService<IShowRepository>(),
				provider.GetRequiredService<IOptions<MazeCacheConfig>>(),
				new ShowApplication(provider.GetRequiredService<IMazeService>())
				));
			services.AddScoped<IShowApplication>(provider =>
				new PaginatedShowApplication(provider.GetRequiredService<CachedShowApplication>()));

			services.AddSwaggerGen();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMvc();
			app.UseSwagger();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});
		}
	}
}

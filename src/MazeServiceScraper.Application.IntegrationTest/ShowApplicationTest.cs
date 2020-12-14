using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Config;
using MazeServiceScraper.Infrastructure.Database;
using MazeServiceScraper.Infrastructure.MazeWebService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace MazeServiceScraper.Application.IntegrationTest
{
	[TestFixture]
	public class ShowApplicationTest
	{
		private ShowApplication _sut;
		private MazeDbContext _mazeDbContext;
		private IOptions<MazeCacheConfig> _mazeCacheConfig;

		[SetUp]

		public void SetUp()
		{
			var configuration = GetIConfigurationRoot(TestContext.CurrentContext.TestDirectory);
			var mazeServiceConfig = configuration.GetSection("MazeService");

			var mazeServiceConfigOption = Options.Create<MazeServiceConfig>(mazeServiceConfig.Get<MazeServiceConfig>());
			_mazeCacheConfig = Options.Create<MazeCacheConfig>(mazeServiceConfig.Get<MazeCacheConfig>());

			var httpClientFactory = Substitute.For<IHttpClientFactory>();
			httpClientFactory.CreateClient().Returns(new HttpClient());

			var options = CreateDbContextOptions();
			var mazeService = new MazeService(httpClientFactory, mazeServiceConfigOption);
			_mazeDbContext = new MazeDbContext(options);
			_sut = new ShowApplication(mazeService, _mazeDbContext, _mazeCacheConfig);
		}

		[Test]
		public async Task TestGetAllCastsAndPersistInDb()
		{
			var showAndCastDetails = await _sut.GetShowAsync();

			Assert.That(showAndCastDetails, Is.Not.Null);
		}

		public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
		{
			var iConfigurationRoot = new ConfigurationBuilder()
				.SetBasePath(outputPath)
				.AddJsonFile("appsettings.json", optional: true)
				.Build();
			return iConfigurationRoot;
		}

		private static DbContextOptions<MazeDbContext> CreateDbContextOptions()
		{
			var options = new DbContextOptionsBuilder<MazeDbContext>()
				.UseInMemoryDatabase(databaseName: "IntegrationTesting")
				.Options;
			return options;
		}
	}
}

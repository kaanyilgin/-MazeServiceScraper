using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Application.Show.Model;
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

		[SetUp]

		public void SetUp()
		{
			_sut = TestUtility.CreateShowApplication();
		}

		[Test]
		public async Task TestGetAllCastsAndPersistInDb()
		{
			var showAndCastDetails = await _sut.GetShowAsync(new GetShowRequest());

			Assert.That(showAndCastDetails, Is.Not.Null);
		}
	}

	[TestFixture]
	public class CachedShowApplicationTest
	{
		private IOptions<MazeCacheConfig> _mazeCacheConfig;
		private MazeDbContext _mazeDbContext;
		private CachedShowApplication _sut;

		[SetUp]
		public void SetUp()
		{
			_mazeDbContext = TestUtility.GetDbContext();
			_sut = TestUtility.CreateCachedShowApplication(_mazeDbContext);
		}

		[Test]
		public async Task TestDataPersisInDb()
		{
			var getShowRequest = new GetShowRequest();
			var showAndCastDetails = await _sut.GetShowAsync(getShowRequest);

			var dbShowCount = _mazeDbContext.Shows.Count();

			// Check if values inserted in db

			Assert.That(showAndCastDetails.Count, Is.EqualTo(dbShowCount));

			var serviceCastCount = 0;

			foreach (var show in showAndCastDetails)
			{
				serviceCastCount += show.Casts.Count;
			}

			Assert.That(serviceCastCount, Is.EqualTo(_mazeDbContext.Casts.Count()));

			// Check if values are not inserted in db again

			showAndCastDetails = await _sut.GetShowAsync(getShowRequest);

			Assert.That(showAndCastDetails.Count, Is.EqualTo(dbShowCount));
			Assert.That(serviceCastCount, Is.EqualTo(_mazeDbContext.Casts.Count()));
		}
	}

	public static class TestUtility
	{
		public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
		{
			var iConfigurationRoot = new ConfigurationBuilder()
				.SetBasePath(outputPath)
				.AddJsonFile("appsettings.json", optional: true)
				.Build();
			return iConfigurationRoot;
		}

		public static ShowApplication CreateShowApplication()
		{
			var configuration = TestUtility.GetIConfigurationRoot(TestContext.CurrentContext.TestDirectory);
			var mazeServiceConfig = configuration.GetSection("MazeService"); var mazeServiceConfigOption = Options.Create<MazeServiceConfig>(mazeServiceConfig.Get<MazeServiceConfig>());

			var httpClientFactory = Substitute.For<IHttpClientFactory>();
			httpClientFactory.CreateClient().Returns(new HttpClient());
			var mazeService = new MazeService(httpClientFactory, mazeServiceConfigOption);

			return new ShowApplication(mazeService);
		}

		public static CachedShowApplication CreateCachedShowApplication(MazeDbContext mazeDbContext)
		{
			var configuration = TestUtility.GetIConfigurationRoot(TestContext.CurrentContext.TestDirectory);
			var mazeCachedServiceConfig = configuration.GetSection("MazeCacheConfig");
			var mazeCachedServiceConfigOption = Options.Create<MazeCacheConfig>(mazeCachedServiceConfig.Get<MazeCacheConfig>());

			var decorated = CreateShowApplication();
			var showRepository = new ShowRepository(mazeDbContext);
			return new CachedShowApplication(showRepository, mazeCachedServiceConfigOption, decorated);
		}

		public static MazeDbContext GetDbContext()
		{
			return new MazeDbContext(CreateDbContextOptions());
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

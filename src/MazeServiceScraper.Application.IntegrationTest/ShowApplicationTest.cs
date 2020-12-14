using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Config;
using MazeServiceScraper.Infrastructure.MazeWebService;
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
			var appSetting = ConfigurationManager.AppSettings["MazeService"];

			var httpClientFactory = Substitute.For<IHttpClientFactory>();
			httpClientFactory.CreateClient().Returns(new HttpClient());
			var optionsManager = Substitute.For<IOptions<MazeServiceConfig>>();
			optionsManager.Value.Returns(new MazeServiceConfig()
			{
				Shows = "http://api.tvmaze.com/shows", // todo read it from config
				ShowsCast = "http://api.tvmaze.com/shows/{0}/cast" // todo read it from config
			});

			var mazeService = new MazeService(httpClientFactory, optionsManager);
			_sut = new ShowApplication(mazeService);
		}

		[Test]
		public async Task TestGetAllCasts()
		{
			var showAndCastDetails = await _sut.GetShowAsync();

			Assert.That(showAndCastDetails, Is.Not.Null);
		}
	}
}

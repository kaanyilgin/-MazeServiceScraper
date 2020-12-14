using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Application.Show.Model;
using MazeServiceScraper.Config;
using MazeServiceScraper.Infrastructure.Database;
using MazeServiceScraper.Infrastructure.MazeWebService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Cast = MazeServiceScraper.Infrastructure.MazeWebService.Cast;

namespace MazeServiceScraper.Application.UnitTest
{
	[TestFixture]
	public class ShowApplicationTest
	{
		private ShowApplication _sut;
		private IMazeService _mazeService;
		private GetShowRequest _getShowRequest;

		[SetUp]
		public void SetUp()
		{
			_mazeService = Substitute.For<IMazeService>();
			_sut = new ShowApplication(_mazeService);
			_getShowRequest = new GetShowRequest();
		}

		[Test]
		public async Task TestServiceDataMapToDomainModel()
		{
			MockGetShowsAsync();
			MockGetCasOfShowAsync();

			IList<Domain.ShowDomain.Show> shows = await _sut.GetShowAsync(_getShowRequest);

			Assert.That(shows, Is.Not.Null);
		}

		private void MockGetCasOfShowAsync(bool changeId = false)
		{
			_mazeService.GetCastOfAShowAsync(changeId == false ? 1 : 2).Returns(new List<Infrastructure.MazeWebService.Cast>()
			{
				new Cast()
				{
					person = new Person()
					{
						id =9,
						name = "Dean Norris",
						birthday = "1963-04-08"
					}
				},
				new Cast()
				{
					person = new Person()
					{
						id = 7,
						name = "Mike Vogel",
						birthday = "1979-07-17"
					}
				}
			});
			_mazeService.GetCastOfAShowAsync(4).Returns(new List<Infrastructure.MazeWebService.Cast>()
			{
				new Cast()
				{
					person = new Person()
					{
						id = 6,
						name = "Michael Emerson",
						birthday = "1950-01-01"
					}
				}
			});
		}

		private void MockGetShowsAsync(bool changeId = false)
		{
			_mazeService.GetShowsAsync().Returns(new List<Infrastructure.MazeWebService.Show>()
			{
				new Infrastructure.MazeWebService.Show()
				{
					id = changeId == false ? 1 : 2,
					name = "Game Of Thrones"
				},
				new Infrastructure.MazeWebService.Show()
				{
					id = 4,
					name = "Big Bang Theory"
				}
			});
		}
	}

	[TestFixture]
	public class CachedShowApplicationTest
	{
		private CachedShowApplication _sut;
		private IShowApplication _decorated;
		private IShowRepository _showRepository;
		private List<Domain.ShowDomain.Show> _decoratedShows;
		private IOptions<MazeCacheConfig> _mazeCacheConfig;
		private GetShowRequest _getShowRequest;

		[SetUp]
		public void SetUp()
		{
			_mazeCacheConfig = Substitute.For<IOptions<MazeCacheConfig>>();
			_mazeCacheConfig.Value.Returns(new MazeCacheConfig() { DbCacheSecond = 60 });
			_showRepository = Substitute.For<IShowRepository>();
			_decorated = Substitute.For<IShowApplication>();
			_sut = new CachedShowApplication(_showRepository, _mazeCacheConfig, _decorated);

			_decoratedShows = new List<Domain.ShowDomain.Show>();
			_getShowRequest = new GetShowRequest();
			_decorated.GetShowAsync(_getShowRequest).Returns(_decoratedShows);
		}

		[Test]
		public async Task TestDataPersisInDb()
		{
			_decoratedShows.Add(new Domain.ShowDomain.Show(1, "Tv show", new List<Domain.ShowDomain.Cast>()));

			await _sut.GetShowAsync(_getShowRequest);

			_showRepository.Received(1).AddShows(Arg.Any<IList<Infrastructure.Database.Show>>());
		}

		private void MockDecoratedGetShowAsync()
		{
			_decorated.GetShowAsync(new GetShowRequest()).Returns(_decoratedShows);
		}

		[Test]
		public async Task TestDataRetrieveFromCache()
		{
			_showRepository.GetShowsCreatedBeforeSecond(_mazeCacheConfig.Value.DbCacheSecond).Returns(
				new List<Infrastructure.Database.Show>()
				{
					new Infrastructure.Database.Show()
					{
						CreatedTime = DateTime.Now.AddSeconds(-1),
						ShowId = 1,
						Name = "The good show",
						Casts = new List<Infrastructure.Database.Cast>()
					}
				});
			_decoratedShows.Add(new Domain.ShowDomain.Show(2, "Tv show", new List<Domain.ShowDomain.Cast>()));

			// Get value from cache
			var showAndCastDetails = await _sut.GetShowAsync(_getShowRequest);

			Assert.That(showAndCastDetails.FirstOrDefault().Id, Is.EqualTo(1), "Value is not retrieved from cache");
		}
	}

	[TestFixture]
	public class PaginatedShowApplicationTest
	{
		private FilteringShowApplication _sut;
		private IShowApplication _decorated;
		private GetShowRequest _getShowRequest;

		[SetUp]
		public void SetUp()
		{
			_getShowRequest = new GetShowRequest()
			{
				PageNumber = 1,
				PageSize = 10
			};
			_decorated = Substitute.For<IShowApplication>();
			_sut = new FilteringShowApplication(_decorated);
		}

		[Test]
		public async Task TestGetShowAsyncPaginateAllShowsIfThereIsNoFilteringAndOrderByDescBirthday()
		{
			MockDecoratedService();

			var shows = await _sut.GetShowAsync(_getShowRequest);

			var firstShowsCast = shows.FirstOrDefault().Casts.FirstOrDefault();
			Assert.That(shows, Has.Count.EqualTo(_getShowRequest.PageSize));
			Assert.That(firstShowsCast.Id, Is.EqualTo(2), "Cast is not ordered by birthday");
		}

		[Test]
		public async Task TestGetShowFilterShowsById()
		{
			_getShowRequest.ShowsIds = new List<int>() { 1 };
			MockDecoratedService();

			var shows = await _sut.GetShowAsync(_getShowRequest);

			Assert.That(shows, Has.Count.EqualTo(1));
			Assert.That(shows.FirstOrDefault().Id, Is.EqualTo(1));
		}

		private void MockDecoratedService()
		{
			var response = new List<Domain.ShowDomain.Show>();
			for (int i = 0; i < 20; i++)
			{
				response.Add(new Domain.ShowDomain.Show(i, $"Show: {i}", new List<Domain.ShowDomain.Cast>()
				{
					new Domain.ShowDomain.Cast(1, "Older Cast", new DateTime(1970, 12, 12)),
					new Domain.ShowDomain.Cast(2, "Younger Cast", new DateTime(2000, 12, 12))
				}));
			}

			_decorated.GetShowAsync(_getShowRequest).Returns(response);
		}
	}
}

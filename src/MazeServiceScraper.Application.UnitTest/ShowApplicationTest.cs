using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Config;
using MazeServiceScraper.Infrastructure.Database;
using MazeServiceScraper.Infrastructure.MazeWebService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Cast = MazeServiceScraper.Infrastructure.MazeWebService.Cast;

namespace MazeServiceScraper.Application.UnitTest
{
	[TestFixture]
	public class ShowApplicationTest
	{
		private ShowApplication _sut;
		private IMazeService _mazeService;

		[SetUp]
		public void SetUp()
		{
			var options = new DbContextOptionsBuilder<MazeDbContext>()
				.UseInMemoryDatabase(databaseName: "UnitTesting")
				.Options;
			var mazeDbContext = new MazeDbContext(options);
			_mazeService = Substitute.For<IMazeService>();
			var mazeCacheConfig = Substitute.For<IOptions<MazeCacheConfig>>();
			_sut = new ShowApplication(_mazeService, mazeDbContext, mazeCacheConfig);
		}

		[Test]
		public async Task TestCastsOfShowOrderedByBirthday()
		{
			MockGetShowsAsync();
			MockGetCasOfShowAsync();

			IList<Domain.ShowDomain.Show> shows = await _sut.GetShowAsync();

			Assert.That(shows, Is.Not.Null);
			var casts = shows[0].Casts;
			Assert.That(casts.First().Id, Is.EqualTo(7), "Casts are not ordered by birthday");
		}

		[Test]
		public async Task TestDataPersisInDb()
		{
			MockGetShowsAsync();
			MockGetCasOfShowAsync();

			IList<Domain.ShowDomain.Show> shows = await _sut.GetShowAsync();

			int castCount = 0;

			foreach (var show in shows)
			{
				castCount += show.Casts.Count();
			}

			Assert.That(castCount, Is.EqualTo(castCount));
		}

		[Test]
		public async Task TestDataRetrieveFromCache()
		{
			MockGetShowsAsync();
			MockGetCasOfShowAsync();

			// Get value from cache

			var showAndCastDetails = await _sut.GetShowAsync();

			MockGetShowsAsync(true);
			MockGetCasOfShowAsync(true);

			showAndCastDetails = await _sut.GetShowAsync();

			Assert.That(showAndCastDetails.FirstOrDefault().Id, Is.EqualTo(1), "Value is not retrieved from cache");
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
					name = "Big Bang Theort"
				}
			});
		}
	}
}

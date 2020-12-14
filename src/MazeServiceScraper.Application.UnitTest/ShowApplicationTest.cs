using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Infrastructure.MazeWebService;
using NSubstitute;
using NUnit.Framework;

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
			_mazeService = Substitute.For<IMazeService>();
			_sut = new ShowApplication(_mazeService);
		}

		[Test]
		public async Task TestCastsOfShowOrderedByBirthday()
		{
			_mazeService.GetShowsAsync().Returns(new List<Infrastructure.MazeWebService.Show>()
			{
				new Infrastructure.MazeWebService.Show()
				{
					id = 1,
					name = "Game Of Thrones"
				},
				new Infrastructure.MazeWebService.Show()
				{
					id = 4,
					name = "Big Bang Theort"
				}
			});
			_mazeService.GetCastOfAShowAsync(1).Returns(new List<Infrastructure.MazeWebService.Cast>()
			{
				new Cast()
				{
					person = new Person()
					{
						id = 9,
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

			IList<Domain.ShowDomain.Show> shows = await _sut.GetShowAsync();

			Assert.That(shows, Is.Not.Null);
			var casts = shows[0].Casts;
			Assert.That(casts.First().Id, Is.EqualTo(7), "Casts are not ordered by birthday");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using MazeServiceScraper.Domain.ShowDomain;
using MazeServiceScraper.Infrastructure.MazeWebService;

namespace MazeServiceScraper.Application.Show
{
	public class ShowApplication : IShowApplication
	{
		private readonly IMazeService _mazeService;

		public ShowApplication(IMazeService mazeService)
		{
			_mazeService = mazeService;
		}

		public IList<Domain.ShowDomain.Show> GetShowAndCastDetails()
		{
			var shows = _mazeService.GetShows();
			var domainShows = new List<Domain.ShowDomain.Show>();

			foreach (var show in shows)
			{
				var casts = _mazeService.GetCastOfAShow(show.id);
				var domainCasts = casts.Select(x =>
					new MazeServiceScraper.Domain.ShowDomain.Cast(x.person.id, x.person.name, DateTime.Parse(x.person.birthday))).ToList();
				var orderedCasts = domainCasts.OrderByDescending(x=>x.Birthday).ToList();
				var domainShow = new Domain.ShowDomain.Show(show.id, show.name, orderedCasts);
				domainShows.Add(domainShow);
			}

			return domainShows;
		}
	}

	internal interface IShowApplication
	{
		IList<Domain.ShowDomain.Show> GetShowAndCastDetails();
	}
}

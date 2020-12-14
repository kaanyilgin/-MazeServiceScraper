using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MazeServiceScraper.Config;
using MazeServiceScraper.Infrastructure.Database;
using MazeServiceScraper.Infrastructure.MazeWebService;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Cast = MazeServiceScraper.Infrastructure.Database.Cast;

namespace MazeServiceScraper.Application.Show
{
	public class ShowApplication : IShowApplication
	{
		private readonly IMazeService _mazeService;

		public ShowApplication(IMazeService mazeService)
		{
			_mazeService = mazeService;
		}

		public async Task<IList<Domain.ShowDomain.Show>> GetShowAsync()
		{
			var shows = await _mazeService.GetShowsAsync();
			var domainShows = new List<Domain.ShowDomain.Show>();

			foreach (var show in shows)
			{
				var casts = await _mazeService.GetCastOfAShowAsync(show.id);
				var domainCasts = casts.Select(x =>
					{
						var birthday = x.person.birthday != null
							? DateTime.Parse(x.person.birthday)
							: DateTime.MinValue;
						return new MazeServiceScraper.Domain.ShowDomain.Cast(x.person.id, x.person.name, birthday);
					}).ToList();
				var orderedCasts = domainCasts.OrderByDescending(x => x.Birthday).ToList();
				var domainShow = new Domain.ShowDomain.Show(show.id, show.name, orderedCasts);
				domainShows.Add(domainShow);
			}

			return domainShows;
		}
	}
}

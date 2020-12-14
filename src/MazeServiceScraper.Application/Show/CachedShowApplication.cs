using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show.Model;
using MazeServiceScraper.Config;
using MazeServiceScraper.Infrastructure.Database;
using Microsoft.Extensions.Options;

namespace MazeServiceScraper.Application.Show
{
	public class CachedShowApplication : IShowApplication
	{
		private readonly IShowRepository _showRepository;
		private readonly IOptions<MazeCacheConfig> _mazeCacheConfig;
		private readonly IShowApplication _decorated;

		public CachedShowApplication(IShowRepository showRepository, IOptions<MazeCacheConfig> mazeCacheConfig, IShowApplication decorated)
		{
			_showRepository = showRepository;
			_mazeCacheConfig = mazeCacheConfig;
			_decorated = decorated;
		}

		public async Task<IList<Domain.ShowDomain.Show>> GetShowAsync(GetShowRequest getShowRequest)
		{
			IList<Domain.ShowDomain.Show> shows;

			var cachedShows = GetCachedShowsIfThereIsAny();

			if (cachedShows.Count > 0)
			{
				shows = cachedShows;
			}
			else
			{
				shows = await this._decorated.GetShowAsync(getShowRequest);
				InsertShowsIntoRepository(shows);
			}

			return shows;
		}

		private List<MazeServiceScraper.Domain.ShowDomain.Show> GetCachedShowsIfThereIsAny()
		{
			var cachedValues = _showRepository.GetShowsCreatedBeforeSecond(_mazeCacheConfig.Value.DbCacheSecond);
			var shows = new List<Domain.ShowDomain.Show>();

			foreach (var cachedShow in cachedValues)
			{
				var dbCasts = cachedShow.Casts.Select(x => new Domain.ShowDomain.Cast(x.CastId, x.Name, x.Birthday)).ToList();
				var dbShow = new Domain.ShowDomain.Show(cachedShow.ShowId, cachedShow.Name, dbCasts);
				shows.Add(dbShow);
			}

			return shows;
		}

		private void InsertShowsIntoRepository(IList<Domain.ShowDomain.Show> shows)
		{
			var dbShows = new List<Infrastructure.Database.Show>();

			foreach (var show in shows)
			{
				var insertShowToDb = CreateRepositoryShow(show);
				dbShows.Add(insertShowToDb);
			}

			this._showRepository.AddShows(dbShows);
		}

		private Infrastructure.Database.Show CreateRepositoryShow(Domain.ShowDomain.Show show)
		{
			var dbShow = new Infrastructure.Database.Show()
			{
				Name = show.Name,
				ShowId = show.Id,
				CreatedTime = DateTime.Now,
				Casts = show.Casts.Select(x => new Infrastructure.Database.Cast()
				{
					Name = x.Name,
					Birthday = x.Birthday,
					CastId = x.Id
				}).ToList()
			};

			return dbShow;
		}
	}
}

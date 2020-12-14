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
		private readonly MazeDbContext _mazeDbContext;
		private readonly IOptions<MazeCacheConfig> _mazeCacheConfig;

		public ShowApplication(IMazeService mazeService, MazeDbContext mazeDbContext, IOptions<MazeCacheConfig> mazeCacheConfig)
		{
			_mazeService = mazeService;
			_mazeDbContext = mazeDbContext;
			_mazeCacheConfig = mazeCacheConfig;
		}

		public async Task<IList<Domain.ShowDomain.Show>> GetShowAsync()
		{
			var cachedValues = _mazeDbContext.Shows.Where(x =>
				x.CreatedTime > GetDateTimeNow().AddSeconds(_mazeCacheConfig.Value.DbCacheSecond));

			if (cachedValues.Count() > 0)
			{
				List<Domain.ShowDomain.Show> shows = new List<Domain.ShowDomain.Show>();

				foreach (var cachedShow in cachedValues)
				{
					var dbCasts = cachedShow.Casts.Select(x => new Domain.ShowDomain.Cast(x.CastId, x.Name, x.Birthday)).ToList();
					var dbShow = new Domain.ShowDomain.Show(cachedShow.ShowId, cachedShow.Name, dbCasts);
					shows.Add(dbShow);
				}
			}

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
				InsertShowToDb(domainShow);
			}

			_mazeDbContext.SaveChanges();
			return domainShows;
		}

		private void InsertShowToDb(Domain.ShowDomain.Show show)
		{
			_mazeDbContext.Shows.Add(new Infrastructure.Database.Show()
			{
				Name = show.Name,
				ShowId = show.Id,
				CreatedTime = GetDateTimeNow(),
				Casts = show.Casts.Select(x => new Infrastructure.Database.Cast()
				{
					Name = x.Name,
					Birthday = x.Birthday,
					CastId = x.Id
				}).ToList()
			});
		}

		public virtual DateTime GetDateTimeNow()
		{
			return DateTime.Now;
		}
	}

	public interface IShowApplication
	{
		Task<IList<Domain.ShowDomain.Show>> GetShowAsync();
	}
}

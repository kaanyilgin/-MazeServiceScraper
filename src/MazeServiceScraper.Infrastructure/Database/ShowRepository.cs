using System;
using System.Collections.Generic;
using System.Text;

namespace MazeServiceScraper.Infrastructure.Database
{
	public class ShowRepository : IShowRepository
	{
		ctor

		public List<Show> GetShowsCreatedBeforeSecond(int second)
		{
			_mazeDbContext.Shows.Where(x =>
				x.CreatedTime > GetDateTimeNow().AddSeconds(_mazeCacheConfig.Value.DbCacheSecond));
		}
	}

	public interface IShowRepository
	{
		List<Show> GetShowsCreatedBeforeSecond(int second);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MazeServiceScraper.Infrastructure.Database
{
	public class ShowRepository : IShowRepository
	{
		private readonly MazeDbContext _context;

		public ShowRepository(MazeDbContext context)
		{
			_context = context;
		}

		public IList<Show> GetShowsCreatedBeforeSecond(int second)
		{
			return _context.Shows.Where(x => x.CreatedTime > DateTime.Now.AddSeconds(-1 * second))
				.Include(x=>x.Casts)
				.ToList();
		}

		public void AddShows(IList<Show> shows)
		{
			_context.Shows.AddRange(shows);
			_context.SaveChanges();
		}
	}

	public interface IShowRepository
	{
		IList<Show> GetShowsCreatedBeforeSecond(int second);

		void AddShows(IList<Show> shows);
	}
}

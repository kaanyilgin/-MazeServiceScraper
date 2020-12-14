using System.Collections.Generic;

namespace MazeServiceScraper.Infrastructure.Database
{
	public interface IShowRepository
	{
		IList<Show> GetShowsCreatedBeforeSecond(int second);

		void AddShows(IList<Show> shows);
	}
}
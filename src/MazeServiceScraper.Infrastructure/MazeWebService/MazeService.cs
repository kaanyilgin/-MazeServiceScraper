using System.Collections.Generic;

namespace MazeServiceScraper.Infrastructure.MazeWebService
{
	public class MazeService
	{
	}

	public interface IMazeService
	{
		List<Show> GetShows();
		List<Cast> GetCastOfAShow(int any);
	}
}

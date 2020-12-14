using System.Collections.Generic;
using System.Threading.Tasks;

namespace MazeServiceScraper.Infrastructure.MazeWebService
{
	public interface IMazeService
	{
		Task<List<Show>> GetShowsAsync();
		Task<List<Cast>> GetCastOfAShowAsync(int showId);
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show.Model;

namespace MazeServiceScraper.Application.Show
{
	public interface IShowApplication
	{
		Task<IList<Domain.ShowDomain.Show>> GetShowAsync(GetShowRequest getShowRequest);
	}
}
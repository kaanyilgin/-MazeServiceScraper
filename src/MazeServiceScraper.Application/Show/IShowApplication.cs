using System.Collections.Generic;
using System.Threading.Tasks;

namespace MazeServiceScraper.Application.Show
{
	public interface IShowApplication
	{
		Task<IList<Domain.ShowDomain.Show>> GetShowAsync();
	}
}
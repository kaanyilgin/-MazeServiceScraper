using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show.Model;

namespace MazeServiceScraper.Application.Show
{
	public class PaginatedShowApplication : IShowApplication
	{
		private readonly IShowApplication _decorated;

		public PaginatedShowApplication(IShowApplication decorated)
		{
			_decorated = decorated;
		}

		public async Task<IList<Domain.ShowDomain.Show>> GetShowAsync(GetShowRequest getShowRequest)
		{
			var showAsync = await _decorated.GetShowAsync(getShowRequest);
			var paginatedShows = showAsync
				.Skip((getShowRequest.PageNumber - 1) * getShowRequest.PageSize)
				.Take(getShowRequest.PageSize)
				.ToList();

			foreach (var show in paginatedShows)
			{
				show.Casts = show.Casts.OrderByDescending(x => x.Birthday).ToList();
			}

			return paginatedShows;
		}
	}
}

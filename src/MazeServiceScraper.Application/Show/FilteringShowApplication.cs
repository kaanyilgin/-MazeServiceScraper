using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show.Model;

namespace MazeServiceScraper.Application.Show
{
	public class FilteringShowApplication : IShowApplication
	{
		private readonly IShowApplication _decorated;

		public FilteringShowApplication(IShowApplication decorated)
		{
			_decorated = decorated;
		}

		public async Task<IList<Domain.ShowDomain.Show>> GetShowAsync(GetShowRequest getShowRequest)
		{
			var shows = await _decorated.GetShowAsync(getShowRequest);

			if (getShowRequest.ShowsIds != null)
			{
				shows = shows.Where(x => getShowRequest.ShowsIds.Contains(x.Id)).ToList();
			}

			var paginatedShows = ApplyPagination(getShowRequest, shows);
			OrderCastByBirtdayDesc(paginatedShows);

			return paginatedShows;
		}

		private static void OrderCastByBirtdayDesc(List<Domain.ShowDomain.Show> paginatedShows)
		{
			foreach (var show in paginatedShows)
			{
				show.Casts = show.Casts.OrderByDescending(x => x.Birthday).ToList();
			}
		}

		private static List<Domain.ShowDomain.Show> ApplyPagination(GetShowRequest getShowRequest, IList<Domain.ShowDomain.Show> shows)
		{
			var paginatedShows = shows
				.Skip((getShowRequest.PageNumber - 1) * getShowRequest.PageSize)
				.Take(getShowRequest.PageSize)
				.ToList();
			return paginatedShows;
		}
	}
}

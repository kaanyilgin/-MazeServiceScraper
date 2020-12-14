using System.Collections.Generic;

namespace MazeServiceScraper.Application.Show.Model
{
	public class GetShowRequest
	{
		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 20;

		public List<int> ShowsIds { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MazeServiceScraper.Application.Show.Model
{
	public class GetShowRequest
	{
		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 20;
	}
}

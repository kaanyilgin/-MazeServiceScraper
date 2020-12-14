using System;
using System.ComponentModel.DataAnnotations;

namespace MazeServiceScraper.Infrastructure.Database
{
	public class Cast
	{
		[Key]
		public int Id { get; set; }

		public int CastId { get; set; }

		public string Name { get; set; }

		public DateTime Birthday { get; set; }

		
	}
}
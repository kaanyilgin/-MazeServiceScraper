using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text;

namespace MazeServiceScraper.Infrastructure.Database
{
	public class Show
	{
		[Key]
		public int Id { get; set; }
		public int ShowId { get; set; }
		public string Name { get; set; }
		public DateTime CreatedTime { get; set; }
		public ICollection<Cast> Casts { get; set; }
	}
}

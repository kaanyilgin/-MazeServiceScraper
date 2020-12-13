using System.Collections.Generic;

namespace MazeServiceScraper.Domain.ShowDomain
{
	public class Show
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public IList<Cast> Casts { get; set; }

		public Show(int id, string name, IList<Cast> casts)
		{
			Id = id;
			Name = name;
			Casts = casts;
		}
	}
}

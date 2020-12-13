using System;

namespace MazeServiceScraper.Domain.ShowDomain
{
	public class Cast
	{

		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime Birthday { get; set; }
		public Cast(int id, string name, DateTime birthday)
		{
			Id = id;
			Name = name;
			Birthday = birthday;
		}
	}
}
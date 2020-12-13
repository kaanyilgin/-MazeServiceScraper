using System;
using System.Collections.Generic;
using System.Text;

namespace MazeServiceScraper.Infrastructure.MazeWebService
{
	public class Person
	{
		public int id { get; set; }
		public string url { get; set; }
		public string name { get; set; }
		public Country country { get; set; }
		public string birthday { get; set; }
		public object deathday { get; set; }
		public string gender { get; set; }
		public Image image { get; set; }
		public Links _links { get; set; }
	}

	public class Character
	{
		public int id { get; set; }
		public string url { get; set; }
		public string name { get; set; }
		public Image image { get; set; }
		public Links _links { get; set; }
	}

	public class Cast
	{
		public Person person { get; set; }
		public Character character { get; set; }
		public bool self { get; set; }
		public bool voice { get; set; }
	}
}

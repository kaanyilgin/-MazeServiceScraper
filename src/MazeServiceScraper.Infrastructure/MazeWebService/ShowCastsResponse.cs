using System;
using System.Collections.Generic;
using System.Text;

namespace MazeServiceScraper.Infrastructure.MazeWebService
{
	public class Person
	{
		public int id { get; set; }
		public string name { get; set; }
		public string birthday { get; set; }
	}

	public class Cast
	{
		public Person person { get; set; }
	}
}

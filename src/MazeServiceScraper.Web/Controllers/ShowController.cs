using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MazeServiceScraper.Web.Controllers
{
	public class ShowController : ControllerBase
	{
		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<Show>> Get()
		{
			return new string[] { "value1", "value2" };
		}
	}
}

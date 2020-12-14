﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MazeServiceScraper.Application.Show;
using MazeServiceScraper.Domain.ShowDomain;
using Microsoft.AspNetCore.Mvc;

namespace MazeServiceScraper.Web.Controllers
{
	[Route("api/[controller]")]
	public class ShowsController : ControllerBase
	{
		private readonly IShowApplication _showApplication;

		public ShowsController(IShowApplication showApplication)
		{
			_showApplication = showApplication;
		}

		// GET api/values
		[HttpGet]
		public async Task<IList<Show>> Get()
		{
			return await _showApplication.GetShowAsync();
		}
	}
}

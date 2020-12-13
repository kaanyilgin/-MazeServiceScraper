using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MazeServiceScraper.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MazeServiceScraper.Infrastructure.MazeWebService
{
	public class MazeService  : IMazeService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IOptions<MazeServiceConfig> _config;

		public MazeService(IHttpClientFactory httpClientFactory, IOptions<MazeServiceConfig> config)
		{
			_httpClientFactory = httpClientFactory;
			_config = config;
		}

		public async Task<List<Show>> GetShows()
		{
			var client = _httpClientFactory.CreateClient();

			var showsResult = await client.GetStringAsync(_config.Value.Shows);

			return JsonConvert.DeserializeObject<List<MazeServiceScraper.Infrastructure.MazeWebService.Show>>(showsResult);
		}

		public async Task<List<Cast>> GetCastOfAShow(int showId)
		{
			var client = _httpClientFactory.CreateClient();

			var showsCastsResult= await client.GetStringAsync(string.Format(_config.Value.ShowsCast, showId));

			return JsonConvert.DeserializeObject<List<MazeServiceScraper.Infrastructure.MazeWebService.Cast>>(showsCastsResult);
		}
	}

	public interface IMazeService
	{
		Task<List<Show>> GetShows();
		Task<List<Cast>> GetCastOfAShow(int showId);
	}
}

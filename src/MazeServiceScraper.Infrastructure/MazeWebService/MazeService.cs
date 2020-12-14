using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MazeServiceScraper.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace MazeServiceScraper.Infrastructure.MazeWebService
{
	public class MazeService  : IMazeService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IOptions<MazeServiceConfig> _config;
		private AsyncRetryPolicy _retryPolicy;

		public MazeService(IHttpClientFactory httpClientFactory, IOptions<MazeServiceConfig> config)
		{
			_httpClientFactory = httpClientFactory;
			_config = config;
			_retryPolicy = Policy
				.Handle<Exception>()
				.WaitAndRetryAsync(config.Value.RetryCount, retryAttempt => {
						var timeToWait = TimeSpan.FromSeconds(Math.Pow(config.Value.TooManyRequestWaitingSecond, retryAttempt));
						return timeToWait;
					}
				);
		}

		public async Task<List<Show>> GetShowsAsync()
		{
			var client = _httpClientFactory.CreateClient();
			var showsResult = await _retryPolicy.ExecuteAsync<string>(async () => await client.GetStringAsync(_config.Value.Shows));
			return JsonConvert.DeserializeObject<List<MazeServiceScraper.Infrastructure.MazeWebService.Show>>(showsResult);
		}

		public async Task<List<Cast>> GetCastOfAShowAsync(int showId)
		{
			var client = _httpClientFactory.CreateClient();

			var showsCastsResult = await _retryPolicy.ExecuteAsync<string>(async () => await client.GetStringAsync(string.Format(_config.Value.ShowsCast, showId)));

			return JsonConvert.DeserializeObject<List<MazeServiceScraper.Infrastructure.MazeWebService.Cast>>(showsCastsResult);
		}
	}

	public interface IMazeService
	{
		Task<List<Show>> GetShowsAsync();
		Task<List<Cast>> GetCastOfAShowAsync(int showId);
	}
}

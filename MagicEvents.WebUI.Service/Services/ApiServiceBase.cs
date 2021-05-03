using System.Net.Http;
using MagicEvents.WebUI.Service.Settings;
using Microsoft.Extensions.Options;

namespace MagicEvents.WebUI.Service.Services
{
    public class ApiServiceBase
    {
        protected readonly HttpClient _httpClient;
        private readonly ApiService _apiSettings;
        protected readonly string _apiBaseUrl;
        public ApiServiceBase(HttpClient httpClient, IOptions<ApiService> options)
        {
            _httpClient = httpClient;
            _apiSettings = options.Value;
            _apiBaseUrl = _apiSettings.Url;
        }
    }
}
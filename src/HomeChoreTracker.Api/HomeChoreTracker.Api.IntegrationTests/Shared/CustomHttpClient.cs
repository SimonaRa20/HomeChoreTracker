using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeChoreTracker.Api.IntegrationTests.Shared
{
	public class CustomHttpClient
	{
		public readonly HttpClient _httpClient;

		public CustomHttpClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public void AddBearerToken(string token)
		{
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		}

		public async Task<Response<TResponse>> Post<TRequest, TResponse>(string path, TRequest requestBody)
		{
			return await Send<TRequest, TResponse>(path, HttpMethod.Post, requestBody);
		}

		public async Task<Response<TResponse>> Get<TResponse>(string path)
		{
			return await Send<TResponse>(path, HttpMethod.Get);
		}

		public async Task<HttpResponseMessage> SendGetRequest(string path)
		{
			Uri uri = new Uri(path.TrimStart('/').TrimStart(), UriKind.Relative);
			using HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(HttpMethod.Get, uri);
			return await _httpClient.SendAsync(httpRequestMessage);
		}

		public async Task<Response<TResponse>> Send<TRequest, TResponse>(string path, HttpMethod httpMethod, TRequest requestBody)
		{
			Uri uri = new Uri(path.TrimStart('/').TrimStart(), UriKind.Relative);
			using HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(httpMethod, uri);

			if (requestBody != null)
			{
				var httpContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
				httpRequestMessage.Content = httpContent;
			}

			return await SendRequest<TResponse>(httpRequestMessage);
		}

		public async Task<Response<TResponse>> Send<TResponse>(string path, HttpMethod httpMethod)
		{
			Uri uri = new Uri(path.TrimStart('/').TrimStart(), UriKind.Relative);
			using HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(httpMethod, uri);

			return await SendRequest<TResponse>(httpRequestMessage);
		}

		public async Task<Response<TResponse>> SendRequest<TResponse>(HttpRequestMessage httpRequestMessage)
		{
			var response = await _httpClient.SendAsync(httpRequestMessage);

			var responseResult = new Response<TResponse>
			{
				StatusCode = response.StatusCode,
				Message = "",
				Data = default(TResponse)
			};

			if (!response.IsSuccessStatusCode)
			{
				responseResult.Message = $"Request failed with status code {response.StatusCode}.";
				return responseResult;
			}

			if (response.Content.Headers.ContentType.MediaType == "application/pdf" || response.Content.Headers.ContentType.MediaType == "text/calendar")
			{
				// Handle PDF response
				var pdfBytes = await response.Content.ReadAsByteArrayAsync();
				responseResult.Data = (TResponse)(object)pdfBytes;
			}
			else
			{
				// Handle JSON response
				string content = await response.Content.ReadAsStringAsync();
				if (string.IsNullOrEmpty(content))
				{
					return responseResult;
				}
				var responseData = JsonConvert.DeserializeObject<TResponse>(content);
				responseResult.Data = responseData;
			}

			return responseResult;
		}


		private HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, Uri relativePath)
		{
			HttpRequestMessage request = new HttpRequestMessage();
			request.Method = httpMethod;
			request.RequestUri = relativePath;
			return request;
		}
	}
}

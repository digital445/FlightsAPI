using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;

namespace FlightsAPI.Caching
{
	public sealed partial class CustomCachingPolicy : IOutputCachePolicy
	{
		public static readonly CustomCachingPolicy Instance = new();

        private CustomCachingPolicy()
		{
		}

		ValueTask IOutputCachePolicy.CacheRequestAsync(
			OutputCacheContext context,
			CancellationToken cancellationToken)
		{
			var attemptOutputCaching = AttemptOutputCaching(context);
			context.EnableOutputCaching = true;
			context.AllowCacheLookup = attemptOutputCaching;
			context.AllowCacheStorage = attemptOutputCaching;
			context.AllowLocking = true;
			context.ResponseExpirationTimeSpan = TimeSpan.FromMinutes(10);

			// Vary by any query by default
			context.CacheVaryByRules.QueryKeys = "*";

			context.HttpContext.Request.EnableBuffering();

			using var reader = new StreamReader(context.HttpContext.Request.Body, leaveOpen: true);
			var body = reader.ReadToEndAsync();

			// Reset the stream position to enable subsequent reads
			context.HttpContext.Request.Body.Position = 0;

			string normalizedBody = WhitespaceRegex().Replace(body.Result, "");
			var keyVal = new KeyValuePair<string, string>("requestBody", normalizedBody);
			context.CacheVaryByRules.VaryByValues.Add(keyVal);

			return ValueTask.CompletedTask;
		}

		ValueTask IOutputCachePolicy.ServeFromCacheAsync
			(OutputCacheContext context, CancellationToken cancellationToken)
		{
			return ValueTask.CompletedTask;
		}

		ValueTask IOutputCachePolicy.ServeResponseAsync
			(OutputCacheContext context, CancellationToken cancellationToken)
		{
			var response = context.HttpContext.Response;

			// Verify existence of cookie headers
			if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
			{
				context.AllowCacheStorage = false;
				return ValueTask.CompletedTask;
			}

			// Check response code
			if (response.StatusCode != StatusCodes.Status200OK)
			{
				context.AllowCacheStorage = false;
				return ValueTask.CompletedTask;
			}

			return ValueTask.CompletedTask;
		}

		private static bool AttemptOutputCaching(OutputCacheContext context)
		{
			// Check if the current request fulfills the requirements
			// to be cached
			var request = context.HttpContext.Request;

			// Verify the method
			if (!HttpMethods.IsPost(request.Method))
			{
				return false;
			}

			// Verify existence of authorization headers
			if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) ||
				request.HttpContext.User?.Identity?.IsAuthenticated == true)
			{
				return false;
			}

			return true;
		}

		[GeneratedRegex(@"\s+", RegexOptions.Compiled)]
		private static partial Regex WhitespaceRegex(); //regular expression for searching whitespaces. Is precompiled for better performance.
	}
}

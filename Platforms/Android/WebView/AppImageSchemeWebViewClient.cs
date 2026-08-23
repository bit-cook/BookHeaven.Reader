using System.Text;
using Android.Graphics;
using Android.Webkit;
using BookHeaven.Core;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Path = System.IO.Path;

namespace BookHeaven.Reader.WebView;

public class CustomBlazorWebViewHandler : BlazorWebViewHandler
{
	protected override void ConnectHandler(Android.Webkit.WebView platformView)
	{
		base.ConnectHandler(platformView);
		platformView.SetWebViewClient(new AppImageSchemeWebViewClient(platformView.WebViewClient));
	}
}

public class AppImageSchemeWebViewClient(WebViewClient? innerClient = null) : WebViewClient
{
	public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView? view, IWebResourceRequest? request)
	{
		return innerClient?.ShouldOverrideUrlLoading(view, request) ?? base.ShouldOverrideUrlLoading(view, request);
	}

	public override void OnPageStarted(Android.Webkit.WebView? view, string? url, Bitmap? favicon)
	{
		innerClient?.OnPageStarted(view, url, favicon);
		base.OnPageStarted(view, url, favicon);
	}

	public override void OnPageFinished(Android.Webkit.WebView? view, string? url)
	{
		innerClient?.OnPageFinished(view, url);
		base.OnPageFinished(view, url);
	}

	public override WebResourceResponse? ShouldInterceptRequest(Android.Webkit.WebView? view, IWebResourceRequest? request)
	{
		if (request?.Url != null && string.Equals(request.Url.Scheme, AppImageScheme.Scheme, StringComparison.OrdinalIgnoreCase))
		{
			var requestUrl = request.Url.ToString();
			if (!string.IsNullOrWhiteSpace(requestUrl) && requestUrl.StartsWith(AppImageScheme.Prefix, StringComparison.OrdinalIgnoreCase))
			{
				return HandleAppImageRequest(requestUrl);
			}
		}

		return innerClient?.ShouldInterceptRequest(view, request) ?? base.ShouldInterceptRequest(view, request);
	}

	private static WebResourceResponse HandleAppImageRequest(string requestUrl)
	{
		var encodedRelativePath = new Uri(requestUrl).Segments[1];
		var queryIndex = encodedRelativePath.IndexOf('?');
		if (queryIndex >= 0)
		{
			encodedRelativePath = encodedRelativePath[..queryIndex];
		}

		var relativePath = Uri.UnescapeDataString(encodedRelativePath)
			.Replace('/', Path.DirectorySeparatorChar)
			.Replace('\\', Path.DirectorySeparatorChar);

		if (relativePath.Contains("..", StringComparison.Ordinal) || Path.IsPathRooted(relativePath))
		{
			return CreateNotFoundResponse();
		}

		var absolutePath = Path.GetFullPath(Path.Combine(FileSystem.AppDataDirectory, relativePath));
		var coversRoot = Path.GetDirectoryName(CoreGlobals.CoversPath) ?? FileSystem.AppDataDirectory;
		if (!absolutePath.StartsWith(coversRoot, StringComparison.OrdinalIgnoreCase) || !File.Exists(absolutePath))
		{
			return CreateNotFoundResponse();
		}

		var contentType = GetContentType(absolutePath);
		var fileBytes = File.ReadAllBytes(absolutePath);
		return new WebResourceResponse(contentType, "utf-8", new MemoryStream(fileBytes));
	}

	private static WebResourceResponse CreateNotFoundResponse()
	{
		const string payload = "Image not found.";
		return new WebResourceResponse("text/plain", "utf-8", new MemoryStream(Encoding.UTF8.GetBytes(payload)));
	}

	private static string GetContentType(string filePath)
	{
		return Path.GetExtension(filePath).ToLowerInvariant() switch
		{
			".jpg" or ".jpeg" => "image/jpeg",
			".png" => "image/png",
			".gif" => "image/gif",
			".webp" => "image/webp",
			".bmp" => "image/bmp",
			".svg" => "image/svg+xml",
			_ => "application/octet-stream"
		};
	}
}

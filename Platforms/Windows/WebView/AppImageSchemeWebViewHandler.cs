using BookHeaven.Core;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace BookHeaven.Reader.WebView;

public class CustomBlazorWebViewHandler : BlazorWebViewHandler
{
	protected override void ConnectHandler(WebView2 platformView)
	{
		base.ConnectHandler(platformView);
		platformView.CoreWebView2Initialized += (_, _) => RegisterCustomScheme(platformView);
		if (platformView.CoreWebView2 is not null)
		{
			RegisterCustomScheme(platformView);
		}
	}

	private static void RegisterCustomScheme(WebView2 platformView)
	{
		if (platformView.CoreWebView2 is null)
		{
			return;
		}

		platformView.CoreWebView2.AddWebResourceRequestedFilter($"{AppImageScheme.Prefix}*", CoreWebView2WebResourceContext.Image);
		platformView.CoreWebView2.WebResourceRequested += (_, args) =>
		{
			var requestedUri = args.Request.Uri;
			if (!requestedUri.StartsWith(AppImageScheme.Prefix, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			
			var relativePath = new Uri(requestedUri).Segments[1];
			var decodedPath = Uri.UnescapeDataString(relativePath).Replace('/', Path.DirectorySeparatorChar);
			if (decodedPath.StartsWith("..", StringComparison.Ordinal) || Path.IsPathRooted(decodedPath))
			{
				return;
			}

			var rootDirectory = Path.GetDirectoryName(CoreGlobals.CoversPath) ?? Path.GetFullPath(FileSystem.AppDataDirectory);
			var absolutePath = Path.GetFullPath(Path.Combine(rootDirectory, decodedPath));

			if (!absolutePath.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase) || !File.Exists(absolutePath))
			{
				args.Response = platformView.CoreWebView2.Environment.CreateWebResourceResponse(
					new MemoryStream([.. "Image not found."u8]).AsRandomAccessStream(),
					404,
					"Not Found",
					"Content-Type: text/plain;charset=utf-8");
				return;
			}

			var extension = Path.GetExtension(absolutePath).ToLowerInvariant();
			var contentType = extension switch
			{
				".jpg" or ".jpeg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".webp" => "image/webp",
				".bmp" => "image/bmp",
				".svg" => "image/svg+xml",
				_ => "application/octet-stream"
			};

			using var fileStream = File.OpenRead(absolutePath);
			var bytes = File.ReadAllBytes(absolutePath);
			args.Response = platformView.CoreWebView2.Environment.CreateWebResourceResponse(
				new MemoryStream(bytes).AsRandomAccessStream(),
				200,
				"OK",
				$"Content-Type: {contentType}");
		};
	}
}
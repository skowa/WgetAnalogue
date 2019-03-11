using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WgetAnalogue.Implementations;
using WgetAnalogue.Interfaces;

namespace WgetAnalogue
{
    /// <summary>
    /// The class that makes local copy of a website.
    /// </summary>
    public class WebSiteDownloader
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly IDataSaver _dataSaver;
        private readonly ISet<Uri> _visitedUris = new HashSet<Uri>();

        /// <summary>
        /// The ctor.
        /// </summary>
        /// <param name="dataSaver"> Determines where to save files.</param>
        /// <exception cref="ArgumentNullException"> Thrown, when <paramref name="dataSaver"/> is null.</exception>
        public WebSiteDownloader(IDataSaver dataSaver)
        {
            _dataSaver = dataSaver ?? throw new ArgumentNullException(nameof(dataSaver));
        }

        /// <summary>
        /// The method that makes copy of a website.
        /// </summary>
        /// <param name="url"> The url of the website.</param>
        /// <returns>The task.</returns>
        public async Task DownloadWebsiteAsync(string url) => await DownloadWebsiteAsync(url,
            new Constraint(LinkTransitionConstraintType.All, Array.Empty<string>()), 0);

        /// <summary>
        /// The method that makes copy of the website with some constraints.
        /// </summary>
        /// <param name="url"> The url of the website.</param>
        /// <param name="constraint"> The constrains on the files downloaded.</param>
        /// <param name="depth"> The depth of the downloading.</param>
        /// <returns> The task.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown, when <paramref name="url"/> is null.
        /// Thrown, when <paramref name="constraint"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown, when <paramref name="depth"/> is negative.
        /// Thrown, when <paramref name="url"/> is an invalid url.
        /// </exception>
        public async Task DownloadWebsiteAsync(string url, IConstraint constraint, int depth)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }

            if (depth < 0)
            {
                throw new ArgumentException($"{nameof(depth)} should be not negative.");
            }

            try
            {
                Uri uri = new Uri(url);
                await DownloadWebpageAsync(uri, constraint, depth);
            }
            catch (UriFormatException)
            {
                throw new ArgumentException($"{nameof(url)} is invalid url");
            }
        }

        private async Task DownloadWebpageAsync(Uri uri, IConstraint constraint, int depth)
        {
            if (depth < 0)
            {
                return;
            }
            
            HtmlDocument document = new HtmlWeb().Load(uri);
            await GetHtmlFileAndDownloadAsync(uri, GetHtmlTitle(document));

            await FindLinksAsync(uri, document, constraint, depth);
            await DownloadFilesAsync(uri, document, constraint);
        }

        private async Task FindLinksAsync(Uri parentUri, HtmlDocument document, IConstraint constraint, int depth)
        {
            IEnumerable<string> links = document.GetLinksFromTagsA();
            int lowerDepth = --depth;
            var tasks = new List<Task>();

            foreach (string link in links)
            {
                if (!FilterLink(link))
                {
                    continue;
                }

                Uri uri = CreateUri(parentUri, link);

                if (!_visitedUris.Contains(uri) && constraint.IsHtmlLinkPermissible(uri, parentUri))
                {
                    _visitedUris.Add(uri);
                    tasks.Add(DownloadWebpageAsync(uri, constraint, lowerDepth));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task DownloadFilesAsync(Uri parentUri, HtmlDocument document, IConstraint constraint)
        {
            IEnumerable<string> links = document.GetAllLinksExceptTagALinks();
            var tasks = new List<Task>();

            foreach (string link in links)
            {
                if (!FilterLink(link))
                {
                    continue;
                }

                Uri uri = CreateUri(parentUri, link);
                if (!_visitedUris.Contains(uri) && constraint.IsSourceLinkPermissible(uri))
                {
                    _visitedUris.Add(uri);
                    tasks.Add(GetSourceFileAndDownloadAsync(uri));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task GetSourceFileAndDownloadAsync(Uri uri)
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    _dataSaver.SaveSourceFile(uri, await response.Content.ReadAsStreamAsync());
                }
            }
        }

        private async Task GetHtmlFileAndDownloadAsync(Uri uri, string title)
        {
            using (HttpResponseMessage response = await HttpClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    _dataSaver.SaveHtmlFile(uri, title, await response.Content.ReadAsStreamAsync());
                }
            }
        }
        
        private Uri CreateUri(Uri parentUri, string url)
        {
            if (!url.StartsWith("http"))
            {
                string uri = parentUri.Scheme + "://" + parentUri.Host + url;
                return new Uri(uri);
            }

            return new Uri(url);
        }

        private bool FilterLink(string url) => !url.Contains("#");

        private string GetHtmlTitle(HtmlDocument document) =>
            document.DocumentNode.Descendants("title").FirstOrDefault()?.InnerText + ".html";
    }
}

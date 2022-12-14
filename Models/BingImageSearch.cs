using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Windows;

namespace ImageSearch.Models
{
    public class BingImageSearch
    {
        private readonly string _subscriptionKey;
        public readonly string _url = string.Empty;
        private string searchString;

        private List<ImageItem> _imageItems;

        // To page through the images, you'll need the next offset that Bing returns.
        private long _nextOffset = 0;

        public long NextOffset { get { return _nextOffset; } set { _nextOffset = value; } }
        public string SearchString { get { return searchString; } }

        // To get additional insights about the image, you'll need the image's
        // insights token (see Visual Search API).

        private static string _insightsToken = null;

        // Bing uses the X-MSEdge-ClientID header to provide users with consistent
        // behavior across Bing API calls. See the reference documentation
        // for usage.

        private static string _clientIdHeader = null;

        // Query
        private const string QUERY_PARAMETER = "?q=";  // Required
        private const string MKT_PARAMETER = "&mkt=";  // Strongly suggested
        private const string COUNT_PARAMETER = "&count=";
        private const string OFFSET_PARAMETER = "&offset=";
        private const string ID_PARAMETER = "&id=";
        private const string SAFE_SEARCH_PARAMETER = "&safeSearch=";

        //query parameters
        private const string ASPECT_PARAMETER = "&aspect=";
        private const string COLOR_PARAMETER = "&color=";
        private const string FRESHNESS_PARAMETER = "&freshness=";
        private const string HEIGHT_PARAMETER = "&height=";
        private const string WIDTH_PARAMETER = "&width=";
        private const string IMAGE_CONTENT_PARAMETER = "&imageContent=";
        private const string IMAGE_TYPE_PARAMETER = "&imageType=";
        private const string LICENSE_PARAMETER = "&license=";
        private const string MAX_FILE_SIZE_PARAMETER = "&maxFileSize=";
        private const string MIN_FILE_SIZE_PARAMETER = "&minFileSize=";
        private const string MAX_HEIGHT_PARAMETER = "&maxHeight=";
        private const string MIN_HEIGHT_PARAMETER = "&minHeight=";
        private const string MAX_WIDTH_PARAMETER = "&maxWidth=";
        private const string MIN_WIDTH_PARAMETER = "&minWidth=";
        private const string SIZE_PARAMETER = "&size=";

        public int retrievalCount = 50;

        public BingImageSearch()
        {
            _subscriptionKey = Environment.GetEnvironmentVariable("BING_SEARCH_V7_SUBSCRIPTION_KEY");
            _url = "https://api.bing.microsoft.com/v7.0/images/search";
            _imageItems = new List<ImageItem>();
        }

        public void SetSearchTerm(string term, int page = -1)
        {
            searchString = term;
            if (page > 1)
            {
                _nextOffset = page * retrievalCount;
            }
        }

        public async Task RunAsync()
        {
            try
            {
                var queryString = QUERY_PARAMETER + Uri.EscapeDataString(searchString);
                queryString += COUNT_PARAMETER + retrievalCount;
                queryString += OFFSET_PARAMETER + _nextOffset;
                queryString += MKT_PARAMETER + "en-us";

                HttpResponseMessage response = await MakeRequestAsync(queryString);
                _clientIdHeader = response.Headers.GetValues("X-MSEdge-ClientID").FirstOrDefault();

                var contentString = await response.Content.ReadAsStringAsync();
                Dictionary<string, object> SearchResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
                if (response.IsSuccessStatusCode)
                {
                    PrintImages(SearchResponse);
                }
                else
                {
                    MessageBox.Show("Error", "No results.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error", e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Prints the list of images in the JSON response.

        private void PrintImages(Dictionary<string, object> response)
        {
            // This example prints the first page of images but if you want to page
            // through the images, you need to capture the next offset that Bing returns.

            // _nextOffset = (long)response["nextOffset"];

            int count = 0;

            var images = response["value"] as Newtonsoft.Json.Linq.JToken;

            foreach (Newtonsoft.Json.Linq.JToken image in images)
            {
                count++;
                _imageItems.Add(new ImageItem
                {
                    Name = image["name"].ToString(),
                    ThumbnailUrl = image["thumbnailUrl"].ToString(),
                    ContentUrl = image["contentUrl"].ToString(),
                    Id = count
                });

                _insightsToken = (string)image["imageInsightsToken"];

            }

        }

        public int GetCurrentPage()
        {
            int page = 0;
            if (_nextOffset <= 0) { return 1; }
            page = ((int)_nextOffset / retrievalCount);
            return (int)page;
        }

        public List<ImageItem> GetSearchedData()
        {
            return _imageItems;
        }

        public void ResetSearchedData()
        {
            _imageItems.Clear();
        }
        private async Task<HttpResponseMessage> MakeRequestAsync(string queryString)
        {
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            return (await Client.GetAsync(_url + queryString));
        }
    }
}

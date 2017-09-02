using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wildmouse.Unearth.AliceUI.Contracts;

namespace Wildmouse.Unearth.AliceUI.Helpers
{
    public static class SearchHelper
    {
        private static SearchServiceClient _searchClient; 
        private static ISearchIndexClient _aliceIndexClient;
        //TODO: Change the search service name and query key if BYO index
        private static string _searchServiceName = "unearthdemo";
        private static string _queryKey = "4C38982C5B39BA43CFA64A63173C0C01";
        private static string _indexName = "alice-index";
        private static Dictionary<string, string> _fieldNames
            = new Dictionary<string, string>() {
            { "Standard Lucene", "standardText" },
            { "English Lucene", "englishText"},
            { "English Microsoft", "microsoftText"}};

        public static string ConstructorErrorMessage { get; set; }

        static SearchHelper()
        {
            try
            {
                _searchClient = new SearchServiceClient(_searchServiceName, new SearchCredentials(_queryKey));
                _aliceIndexClient = _searchClient.Indexes.GetClient(_indexName);
            }
            catch (Exception e)
            {
                ConstructorErrorMessage = e.Message.ToString();
            }
        }

        public static AliceSearchResult Search(string query)
        {
            AliceSearchResult result = new AliceSearchResult()
            {
                Msg = string.Empty,
                FieldResults = new Dictionary<string, List<string>>()
            };

            try
            {
                // One field-scoped search for each field
                foreach (var field in _fieldNames)
                {
                    var searchResult = SearchIndexField(query, field.Value);
                    result.FieldResults.Add(field.Key, searchResult);
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Exception in Alice Search: {0}", ex.Message.ToString());
                result.Msg = msg;
                Trace.TraceError(msg);
            }
            return result;
        }

        private static List<string> SearchIndexField(string query, string fieldName)
        {
            var result = new List<string>();
            SearchParameters sp = new SearchParameters()
            {
                QueryType = QueryType.Full,
                SearchMode = SearchMode.Any,
                HighlightFields = new List<string> { fieldName },
                HighlightPreTag = "<mark><b><i>",
                HighlightPostTag = "</i></b></mark>",
                Top = 1000
            };

            // Do a field-scoped search
            var fieldScopedQuery = fieldName + ":" + query;
            var dsr = _aliceIndexClient.Documents.Search(fieldScopedQuery, sp);

            // Extract the highlights
            if (dsr != null && dsr.Results != null)
            {
                foreach (var sr in dsr.Results)
                {
                    if (sr.Highlights != null)
                    {
                        result.AddRange(sr.Highlights[fieldName]);
                    }
                }
            }
            return result;
        }
    }
}

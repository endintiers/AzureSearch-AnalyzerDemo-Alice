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
        private static string _searchServiceName = "insummary";
        private static string _queryKey = "CEFF77C8B4316EF9C2934EE6AB734E12";
        private static string _indexName = "alice-index";
        private static string[] _fieldNames = new string[] { "standardText", "englishText", "microsoftText" };

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
            DocumentSearchResult dsr = null;
            AliceSearchResult result = new AliceSearchResult()
            {
                Msg = string.Empty,
                StandardLuceneHighLights = new List<string>(),
                EnglishLuceneHighLights = new List<string>(),
                EnglishMicrosoftHighLights = new List<string>()
            };
            try
            {
                SearchParameters sp = new SearchParameters()
                {
                    QueryType = QueryType.Simple,
                    SearchMode = SearchMode.Any,
                    IncludeTotalResultCount = true,
                    HighlightFields = _fieldNames.ToList(),
                    HighlightPreTag = "<mark><b><i>",
                    HighlightPostTag = "</i></b></mark>",
                    Top = 1000
                };
                // Do the search
                dsr = _aliceIndexClient.Documents.Search(query, sp);
                if (dsr != null && dsr.Count > 0)
                {
                    result = ConvertDocSRToAliceSR(dsr);
                }
                else
                {
                    result.Msg = "No Search results returned";
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("Exception in Search: {0}", ex.Message.ToString());
                result.Msg = msg;
                Trace.TraceError(msg);
            }
            return result;
        }

        private static AliceSearchResult ConvertDocSRToAliceSR(DocumentSearchResult dsr)
        {
            var result = new AliceSearchResult()
            {
                Msg = string.Empty,
                StandardLuceneHighLights = new List<string>(),
                EnglishLuceneHighLights = new List<string>(),
                EnglishMicrosoftHighLights = new List<string>()
            };

            foreach (var sr in dsr.Results)
            {
                if (sr.Highlights.ContainsKey(_fieldNames[0]))
                    result.StandardLuceneHighLights.AddRange(sr.Highlights[_fieldNames[0]]);
                if (sr.Highlights.ContainsKey(_fieldNames[1]))
                    result.EnglishLuceneHighLights.AddRange(sr.Highlights[_fieldNames[1]]);
                if (sr.Highlights.ContainsKey(_fieldNames[2]))
                    result.EnglishMicrosoftHighLights.AddRange(sr.Highlights[_fieldNames[2]]);
            }
            return result;
        }
    }
}

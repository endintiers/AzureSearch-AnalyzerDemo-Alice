using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WildMouse.Unearth.AliceLoadApp
{
    class Program
    {
        private static SearchServiceClient _searchClient;
        //TODO: put your search service name here
        private static string _searchServiceName = "REDACTED";
        //TODO put your search service admin key here
        private static string _adminKey = "REDACTED";
        private static string _indexName = "alice-index";

        static void Main(string[] args)
        {
            _searchClient = new SearchServiceClient(_searchServiceName, new SearchCredentials(_adminKey));

            Console.WriteLine("Getting sentences from Alice in Wonderland");
            List<string> sentences;
            using (var fs = File.OpenRead(@".\Documents\AliceInWonderLand.txt"))
            {
                string contents;
                using (var sr = new StreamReader(fs))
                {
                    contents = sr.ReadToEnd();
                }
                sentences = GetSentencesFromString(contents);
            }
            Console.WriteLine("Creating Index");
            DropandRecreateSearchIndex();
            AddSentencesToIndex(sentences.ToArray());
            Console.WriteLine("Mischief Managed!");
            Console.ReadLine();
        }

        public static List<string> GetSentencesFromString(String str)
        {
            //str = "First sentence. Second sentence! Third sentence? Yes.";
            string[] sentences = Regex.Split(str, @"(?<=[\.!\?])\s+");
            return sentences.ToList();
        }

        public static void DropandRecreateSearchIndex()
        {
            // Drop existing index
            if (_searchClient.Indexes.Exists(_indexName))
            {
                _searchClient.Indexes.Delete(_indexName);
            }

            // Create a new Document index
            var docIndex = new Index()
            {
                Name = _indexName,
                Fields = FieldBuilder.BuildForType<AliceDocument>()
            };
            _searchClient.Indexes.Create(docIndex);
        }

        static void AddSentencesToIndex(string[] sentences)
        {
            var indexClient = _searchClient.Indexes.GetClient(_indexName);
        var docActions = new List<IndexAction<AliceDocument>>();
            for(int i=0; i<sentences.Length; i++)
            {
                var doc = new AliceDocument()
                {
                    documentId = i.ToString(),
                    standardText = sentences[i],
                    englishText = sentences[i],
                    microsoftText = sentences[i]
                };
                docActions.Add(IndexAction.MergeOrUpload(doc));
            }
            var docBatch = IndexBatch.New(docActions);
            try
            {
                var result = indexClient.Documents.Index(docBatch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine("Failed to index some of the documents: " + string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }
    }
}

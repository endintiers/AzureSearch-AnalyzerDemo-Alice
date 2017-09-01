using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Wildmouse.Unearth.AliceUI.Contracts;
using Wildmouse.Unearth.AliceUI.Helpers;

namespace Wildmouse.Unearth.AliceUI.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string QueryText { get; set; }

        [BindProperty]
        public AliceSearchResult SearchResult { get; set; }

        [BindProperty]
        public string ResultsClass { get; set; }

        public void OnGet()
        {
            SearchResult = new AliceSearchResult()
            {
                Msg = "Demonstrates different types of search analyzer",
                StandardLuceneHighLights = new List<string>(),
                EnglishLuceneHighLights = new List<string>(),
                EnglishMicrosoftHighLights = new List<string>()
            };
            ResultsClass = "hidden";
        }

        public void OnPost()
        {
            SearchResult = SearchHelper.Search(QueryText);
            ResultsClass = "";
        }
    }
}

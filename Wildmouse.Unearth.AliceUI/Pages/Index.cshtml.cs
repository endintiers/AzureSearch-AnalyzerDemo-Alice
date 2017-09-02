using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public void OnGet()
        {
            SearchResult = new AliceSearchResult()
            {
                Msg = "Demonstrates different types of search analyzer",
                FieldResults = null
            };
        }

        public void OnPost()
        {
            SearchResult = SearchHelper.Search(QueryText);
        }
    }
}

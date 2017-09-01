using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.ComponentModel.DataAnnotations;

namespace WildMouse.Unearth.AliceLoadApp
{
    [SerializePropertyNamesAsCamelCase]
    public partial class AliceDocument
    {
        [Key]
        public string documentId { get; set; }

        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string standardText { get; set; }

        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.EnLucene)]
        public string englishText { get; set; }

        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        public string microsoftText { get; set; }
    }
}

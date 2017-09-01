using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wildmouse.Unearth.AliceUI.Contracts
{
    public class AliceSearchResult
    {
        public string Msg { get; set; }
        public List<string> StandardLuceneHighLights { get; set; }
        public List<string> EnglishLuceneHighLights { get; set; }
        public List<string> EnglishMicrosoftHighLights { get; set; }
    }
}

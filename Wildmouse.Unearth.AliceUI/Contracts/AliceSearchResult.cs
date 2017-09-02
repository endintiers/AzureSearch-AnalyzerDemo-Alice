using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wildmouse.Unearth.AliceUI.Contracts
{
    public class AliceSearchResult
    {
        public string Msg { get; set; }
        public Dictionary<string, List<string>> FieldResults { get; set; }
    }
}

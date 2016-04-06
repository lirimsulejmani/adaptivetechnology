using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AT.Repository.Model
{
    public class TextToReadHyperlink
    {
        public int Id { get; set; }
        public string HyperlinkText { get; set; }
        public int LanguageCode { get; set; }
    }
}

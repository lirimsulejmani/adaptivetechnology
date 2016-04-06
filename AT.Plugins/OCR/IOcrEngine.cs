using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AT.Plugins.OCR
{
   
        public interface IOcrEngine
        {
            string Recognize(Image image);
        }
    
}

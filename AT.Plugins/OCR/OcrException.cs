using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace AT.Plugins.OCR
{
    [Serializable]
    public class OcrException : Exception
    {
        public OcrException()
            : this("General error during OCR.")
        {

        }

        public OcrException(string message)
            : base(message)
        {
        }

        public OcrException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OcrException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

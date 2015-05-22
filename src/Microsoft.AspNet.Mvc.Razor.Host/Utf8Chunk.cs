using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Razor.Generator.Compiler;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class Utf8Chunk : Chunk
    {
        public Utf8Chunk([NotNull] string literal)
        {
            Bytes = Encoding.UTF8.GetBytes(literal);
        }

        public Utf8Chunk([NotNull] byte[] bytes)
        {
            Bytes = bytes;
        }

        public byte[] Bytes { get; set; }
    }
}

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
        public Utf8Chunk([NotNull] LiteralChunk literal)
        {
            Text = literal.Text;
            Bytes = Encoding.UTF8.GetBytes(literal.Text);
            Start = literal.Start;
            Association = literal.Association;
        }

        public byte[] Bytes { get; set; }

        public string Text { get; }

        public override string ToString()
        {
            return Start + " = " + Text;
        }
    }
}

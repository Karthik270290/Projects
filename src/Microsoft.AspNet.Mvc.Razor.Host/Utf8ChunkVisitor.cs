using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Razor.Generator;
using Microsoft.AspNet.Razor.Generator.Compiler;
using Microsoft.AspNet.Razor.Generator.Compiler.CSharp;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Razor.Host
{
    public class Utf8ChunkVisitor : MvcCSharpCodeVisitor
    {
        public Utf8ChunkVisitor([NotNull] CSharpCodeWriter writer,
                                [NotNull] CodeBuilderContext context)
            : base(writer, context)
        {

        }

        protected override void Visit(Utf8Chunk chunk)
        {
            //new byte[1] { 0x9D, }

            Writer.Write($"new byte[{chunk.Bytes.Length}]");
            Writer.Write(" { ");
            for (int i = 0; i < chunk.Bytes.Length; i++)
            {
                Writer.Write(chunk.Bytes[i].ToString());
                Writer.Write(", ");
            }
            Writer.Write(" }");
        }
    }
}

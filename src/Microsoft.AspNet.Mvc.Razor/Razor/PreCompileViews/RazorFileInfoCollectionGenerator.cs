// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RazorFileInfoCollectionGenerator
    {
        private string _fileFormat;

        protected IReadOnlyList<RazorFileInfo> FileInfos { get; private set; }
        protected CSharpParseOptions Options { get; private set; }

        public string CollectionFileName { get { return "__AUTO__GeneratedViewsCollection.cs"; } }
        public string ClassName { get; set; } = "__PreGeneratedViewCollection";

        public RazorFileInfoCollectionGenerator([NotNull] IReadOnlyList<RazorFileInfo> fileInfos,
                                                [NotNull] CSharpParseOptions options)
        {
            FileInfos = fileInfos;
            Options = options;
        }

        public virtual SyntaxTree GenerateCollection()
        {
            var sourceCode = GenerateCode();

            var syntaxTree = SyntaxTreeGenerator.Generate(sourceCode,
                                                          CollectionFileName,
                                                          Options);

            return syntaxTree;
        }

        internal string GenerateCode()
        {
            var builder = new StringBuilder();
            builder.Append(Top);

            foreach (var fileInfo in FileInfos)
            {
                var perFileEntry = GenerateFileEntry(fileInfo);
                builder.Append(perFileEntry);
            }

            builder.Append(Bottom);

            return builder.ToString();
        }

        protected virtual string GenerateFileEntry([NotNull] RazorFileInfo fileInfo)
        {
            var routeEntry = GenerateRouteCollection(fileInfo.Routes);

            return string.Format(FileFormat,
                                 fileInfo.LastModified.ToFileTimeUtc(),
                                 fileInfo.Length,
                                 fileInfo.RelativePath,
                                 fileInfo.FullTypeName,
                                 fileInfo.Hash,
                                 routeEntry)
                                 + "\r\n";
        }

        protected virtual string Top
        {
            get
            {
                return
@"using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.Razor;

namespace __ASP_ASSEMBLY
{
    public class " + ClassName + " : " + nameof(RazorFileInfoCollection) + @"
    {
        public __PreGeneratedViewCollection()
        {
            var fileInfos = new List<" + nameof(RazorFileInfo) + @">();
            " + nameof(RazorFileInfoCollection.FileInfos) + @" = fileInfos;
            " + nameof(RazorFileInfo) + @" info;

";
            }
        }

        protected virtual string Bottom
        {
            get
            {
                return
    @"        }
    }
}
";
            }
        }

        protected virtual string GenerateRouteCollection(IEnumerable<RazorRoute> routes)
        {
            // Generator code will look something like:
            // new RazorRoute[]
            //    {
            //        new RazorRoute("routes[0].RouteTemplate", "routes[0].Verb"),
            //        new RazorRoute("routes[1].RouteTemplate", "routes[1].Verb"),
            //    };

            if (routes == null || !routes.Any())
            {
                return "null";
            }

            var bldr = new StringBuilder();
            bldr.Append("new ").Append(nameof(RazorRoute)).AppendLine("[]").AppendLine("                {");

            foreach (var route in routes)
            {
                bldr.Append("                    new ").Append(nameof(RazorRoute)).Append("(");
                bldr.Append("@\"").Append(route.RouteTemplate).Append("\", ");
                bldr.Append("@\"").Append(route.Verb).AppendLine("\"),");
            }

            bldr.Append("                }");

            return bldr.ToString();
        }

        protected virtual string FileFormat
        {
            get
            {
                if (_fileFormat == null)
                {
                    _fileFormat =
                    "            info = new "
                    + nameof(RazorFileInfo) + @"
            {{
                " + nameof(RazorFileInfo.LastModified) + @" = DateTime.FromFileTimeUtc({0:D}),
                " + nameof(RazorFileInfo.Length) + @" = {1:D},
                " + nameof(RazorFileInfo.RelativePath) + @" = @""{2}"",
                " + nameof(RazorFileInfo.FullTypeName) + @" = @""{3}"",
                " + nameof(RazorFileInfo.Hash) + @" = @""{4}"",
                " + nameof(RazorFileInfo.Routes) + @" = {5},
            }};
            fileInfos.Add(info);
";
                }

                return _fileFormat;
            }
        }
    }
}

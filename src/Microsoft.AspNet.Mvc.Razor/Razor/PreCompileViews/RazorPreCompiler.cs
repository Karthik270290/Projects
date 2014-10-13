// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.FileSystems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RazorPreCompiler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFileSystem _fileSystem;
        private readonly IMvcRazorHost _host;

        public RazorPreCompiler([NotNull] IServiceProvider designTimeServiceProvider) :
            this(designTimeServiceProvider, 
                 designTimeServiceProvider.GetRequiredService<IMvcRazorHost>(),
                 designTimeServiceProvider.GetRequiredService<IOptions<RazorViewEngineOptions>>())
        {
        }

        public RazorPreCompiler([NotNull] IServiceProvider designTimeServiceProvider,
                                [NotNull] IMvcRazorHost host,
                                [NotNull] IOptions<RazorViewEngineOptions> optionsAccessor)
        {
            _serviceProvider = designTimeServiceProvider;
            _host = host;
            _host.EnableInstrumentation = true;

            var appEnv = _serviceProvider.GetRequiredService<IApplicationEnvironment>();
            _fileSystem = optionsAccessor.Options.FileSystem;
        }

        protected virtual string FileExtension { get; } = ".cshtml";

        public virtual void CompileViews([NotNull] IBeforeCompileContext context)
        {
            var descriptors = CreateCompilationDescriptors(context);

            if (descriptors.Count > 0)
            {
                var collectionGenerator = new RazorFileInfoCollectionGenerator(
                                                descriptors,
                                                SyntaxTreeGenerator.GetParseOptions(context.CSharpCompilation));

                var tree = collectionGenerator.GenerateCollection();
                context.CSharpCompilation = context.CSharpCompilation.AddSyntaxTrees(tree);
            }
        }

        protected virtual IReadOnlyList<RazorFileInfo> CreateCompilationDescriptors(
                                                            [NotNull] IBeforeCompileContext context)
        {
            var options = SyntaxTreeGenerator.GetParseOptions(context.CSharpCompilation);
            var list = new List<RazorFileInfo>();

            var directory = new RazorDirectory(_fileSystem, FileExtension);

            foreach (var info in directory.GetFileInfos(string.Empty))
            {
                var descriptor = ParseView(info,
                                           context,
                                           options);

                if (descriptor != null)
                {
                    list.Add(descriptor);
                }
            }

            return list;
        }

        protected virtual RazorFileInfo ParseView([NotNull] RelativeFileInfo fileInfo,
                                                  [NotNull] IBeforeCompileContext context,
                                                  [NotNull] CSharpParseOptions options)
        {
            using (var stream = fileInfo.FileInfo.CreateReadStream())
            {
                var results = _host.GenerateCode(fileInfo.RelativePath, stream);

                string route = RazorRoute.GetRoutes(results).FirstOrDefault();

                var document = results.Document;

                foreach (var parserError in results.ParserErrors)
                {
                    var diagnostic = parserError.ToDiagnostics(fileInfo.FileInfo.PhysicalPath);
                    context.Diagnostics.Add(diagnostic);
                }

                var generatedCode = results.GeneratedCode;

                if (generatedCode != null)
                {
                    var syntaxTree = SyntaxTreeGenerator.Generate(generatedCode, fileInfo.FileInfo.PhysicalPath, options);
                    var fullTypeName = results.GetMainClassName(_host, syntaxTree);

                    if (fullTypeName != null)
                    {
                        context.CSharpCompilation = context.CSharpCompilation.AddSyntaxTrees(syntaxTree);

                        var hash = RazorFileHash.GetHash(fileInfo.FileInfo);

                        return new RazorFileInfo()
                        {
                            FullTypeName = fullTypeName,
                            RelativePath = fileInfo.RelativePath,
                            LastModified = fileInfo.FileInfo.LastModified,
                            Length = fileInfo.FileInfo.Length,
                            Hash = hash,
                            Route = route,
                        };
                    }
                }
            }

            return null;
        }

    }
}

namespace Microsoft.Framework.Runtime
{
    [AssemblyNeutral]
    public interface IBeforeCompileContext
    {
        CSharpCompilation CSharpCompilation { get; set; }

        IList<ResourceDescription> Resources { get; }

        IList<Diagnostic> Diagnostics { get; }
    }
}

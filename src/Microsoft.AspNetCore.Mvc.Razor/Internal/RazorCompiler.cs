// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.Evolution;

namespace Microsoft.AspNetCore.Mvc.Razor.Internal
{
    public class RazorCompiler
    {
        private readonly ICompilationService _compilationService;
        private readonly ICompilerCacheProvider _compilerCacheProvider;
        private readonly MvcRazorTemplateEngine _templateEngine;
        private readonly Func<string, CompilerCacheContext> _getCacheContext;
        private readonly Func<CompilerCacheContext, CompilationResult> _getCompilationResultDelegate;

        private ICompilerCache _compilerCache;

        public RazorCompiler(
            ICompilationService compilationService,
            ICompilerCacheProvider compilerCacheProvider,
            MvcRazorTemplateEngine templateEngine)
        {
            _compilationService = compilationService;
            _compilerCacheProvider = compilerCacheProvider;
            _templateEngine = templateEngine;
            _getCacheContext = GetCacheContext;
            _getCompilationResultDelegate = GetCompilationResult;
        }

        private ICompilerCache CompilerCache
        {
            get
            {
                if (_compilerCache == null)
                {
                    _compilerCache = _compilerCacheProvider.Cache;
                }

                return _compilerCache;
            }
        }

        public CompilerCacheResult Compile(string relativePath)
        {
            return CompilerCache.GetOrAdd(relativePath, _getCacheContext);
        }

        private CompilerCacheContext GetCacheContext(string path)
        {
            var item = _templateEngine.Project.GetItem(path);
            var imports = _templateEngine.Project.FindHierarchicalItems(path, _templateEngine.Options.ImportsFileName);
            return new CompilerCacheContext(item, imports, GetCompilationResult);
        }

        private CompilationResult GetCompilationResult(CompilerCacheContext cacheContext)
        {
            var projectItem = cacheContext.ProjectItem;
            var codeDocument = _templateEngine.CreateCodeDocument(projectItem.Path);
            var cSharpDocument = _templateEngine.GenerateCode(codeDocument);

            CompilationResult compilationResult;
            if (cSharpDocument.Diagnostics.Count > 0)
            {
                compilationResult = GetCompilationFailedResult(
                    projectItem.Path,
                    cSharpDocument.Diagnostics);
            }
            else
            {
                compilationResult = _compilationService.Compile(codeDocument, cSharpDocument);
            }

            return compilationResult;
        }

        private CompilationResult GetCompilationFailedResult(
            string relativePath,
            IEnumerable<RazorDiagnostic> diagnostics)
        {
            // If a SourceLocation does not specify a file path, assume it is produced from parsing the current file.
            var messageGroups = diagnostics.GroupBy(razorError => razorError.Span.FilePath ?? relativePath, StringComparer.Ordinal);

            var failures = new List<CompilationFailure>();
            foreach (var group in messageGroups)
            {
                var filePath = group.Key;
                var fileContent = ReadFileContentsSafely(_templateEngine.Project.GetItem(filePath));
                var compilationFailure = new CompilationFailure(
                    filePath,
                    fileContent,
                    compiledContent: string.Empty,
                    messages: group.Select(parserError => CreateDiagnosticMessage(parserError, filePath)));
                failures.Add(compilationFailure);
            }

            return new CompilationResult(failures);
        }

        private static DiagnosticMessage CreateDiagnosticMessage(
            RazorDiagnostic error,
            string filePath)
        {
            var sourceSpan = error.Span;
            return new DiagnosticMessage(
                message: error.GetMessage(),
                formattedMessage: $"{error} ({sourceSpan.LineIndex},{sourceSpan.CharacterIndex}) {error.GetMessage()}",
                filePath: filePath,
                startLine: sourceSpan.LineIndex + 1,
                startColumn: sourceSpan.CharacterIndex,
                endLine: sourceSpan.LineIndex + 1,
                endColumn: sourceSpan.CharacterIndex + sourceSpan.Length);
        }

        private static string ReadFileContentsSafely(RazorProjectItem projectItem)
        {
            if (projectItem.Exists)
            {
                try
                {
                    using (var reader = new StreamReader(projectItem.Read()))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch
                {
                    // Ignore any failures
                }
            }

            return null;
        }
    }
}

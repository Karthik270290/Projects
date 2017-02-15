// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Host;
using Microsoft.AspNetCore.Razor.Evolution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Razor;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Razor
{
    public class RazorEngineTest
    {
        private static Assembly _assembly = typeof(RazorEngineTest).GetTypeInfo().Assembly;

        #region Runtime
        [Fact]
        public void RazorEngine_Basic_Runtime()
        {
            RunRuntimeTest("Basic");
        }

        [Fact]
        public void RazorEngine_ViewImports_Runtime()
        {
            RunRuntimeTest("_ViewImports");
        }

        [Fact]
        public void RazorEngine_Inject_Runtime()
        {
            RunRuntimeTest("Inject");
        }

        [Fact]
        public void RazorEngine_InjectWithModel_Runtime()
        {
            RunRuntimeTest("InjectWithModel");
        }

        [Fact]
        public void RazorEngine_InjectWithSemicolon_Runtime()
        {
            RunRuntimeTest("InjectWithSemicolon");
        }

        [Fact]
        public void RazorEngine_Model_Runtime()
        {
            RunRuntimeTest("Model");
        }

        [Fact]
        public void RazorEngine_ModelExpressionTagHelper_Runtime()
        {
            RunRuntimeTest("ModelExpressionTagHelper");
        }
        #endregion

        #region DesignTime
        [Fact]
        public void RazorEngine_Basic_DesignTime()
        {
            RunDesignTimeTest("Basic");
        }

        [Fact]
        public void RazorEngine_ViewImports_DesignTime()
        {
            RunDesignTimeTest("_ViewImports");
        }

        [Fact]
        public void RazorEngine_Inject_DesignTime()
        {
            RunDesignTimeTest("Inject");
        }

        [Fact]
        public void RazorEngine_InjectWithModel_DesignTime()
        {
            RunDesignTimeTest("InjectWithModel");
        }

        [Fact]
        public void RazorEngine_InjectWithSemicolon_DesignTime()
        {
            RunDesignTimeTest("InjectWithSemicolon");
        }

        [Fact]
        public void RazorEngine_Model_DesignTime()
        {
            RunDesignTimeTest("Model");
        }

        [Fact]
        public void RazorEngine_MultipleModels_DesignTime()
        {
            RunDesignTimeTest("MultipleModels");
        }

        [Fact]
        public void RazorEngine_ModelExpressionTagHelper_DesignTime()
        {
            RunDesignTimeTest("ModelExpressionTagHelper");
        }
        #endregion

        private static void RunRuntimeTest(string testName)
        {
            // Arrange
            var inputFile = "TestFiles/Input/" + testName + ".cshtml";
            var outputFile = "TestFiles/Output/Runtime/" + testName + ".cs";
            var expectedCode = ResourceFile.ReadResource(_assembly, outputFile, sourceFile: false);

            var engine = RazorEngine.Create(b =>
            {
                InjectDirective.Register(b);
                ModelDirective.Register(b);

                b.Features.Add(new ModelExpressionPass());
                b.Features.Add(new MvcViewDocumentClassifierPass());
                b.Features.Add(new DefaultInstrumentationPass());

                b.Features.Add(new DefaultTagHelperFeature());
                b.Features.Add(GetMetadataReferenceFeature());
            });

            // Act
            RazorCSharpDocument csharpDocument = null;
            using (var stream = ResourceFile.GetResourceStream(_assembly, inputFile, sourceFile: true))
            {
                var codeDocument = RazorCodeDocument.Create(RazorSourceDocument.ReadFrom(stream, inputFile));

                codeDocument.Items["SuppressUniqueIds"] = "test";
                engine.Process(codeDocument);
                csharpDocument = codeDocument.GetCSharpDocument();
            }

            // Assert
            Assert.Empty(csharpDocument.Diagnostics);

#if GENERATE_BASELINES
            ResourceFile.UpdateFile(_assembly, outputFile, expectedCode, csharpDocument.GeneratedCode);
#else
            Assert.Equal(expectedCode, csharpDocument.GeneratedCode, ignoreLineEndingDifferences: true);
#endif
        }

        private static void RunDesignTimeTest(string testName)
        {
            // Arrange
            var inputFile = "TestFiles/Input/" + testName + ".cshtml";
            var outputFile = "TestFiles/Output/DesignTime/" + testName + ".cs";
            var expectedCode = ResourceFile.ReadResource(_assembly, outputFile, sourceFile: false);

            var lineMappingOutputFile = "TestFiles/Output/DesignTime/" + testName + ".mappings.txt";
            var expectedMappings = ResourceFile.ReadResource(_assembly, lineMappingOutputFile, sourceFile: false);

            var engine = RazorEngine.CreateDesignTime(b =>
            {
                InjectDirective.Register(b);
                ModelDirective.Register(b);

                b.Features.Add(new ModelExpressionPass());
                b.Features.Add(new MvcViewDocumentClassifierPass());

                b.Features.Add(new DefaultTagHelperFeature());
                b.Features.Add(GetMetadataReferenceFeature());
            });

            // Act
            RazorCodeDocument codeDocument = null;
            using (var stream = ResourceFile.GetResourceStream(_assembly, inputFile, sourceFile: true))
            {
                // VS tooling passes in paths in all lower case. We'll mimic this behavior in our tests.
                codeDocument = RazorCodeDocument.Create(RazorSourceDocument.ReadFrom(stream, inputFile.ToLowerInvariant()));

                codeDocument.Items["SuppressUniqueIds"] = "test";
                engine.Process(codeDocument);
            }

            // Assert
            var csharpDocument = codeDocument.GetCSharpDocument();
            Assert.Empty(csharpDocument.Diagnostics);

            var serializedMappings = LineMappingsSerializer.Serialize(csharpDocument, codeDocument.Source);

#if GENERATE_BASELINES
            ResourceFile.UpdateFile(_assembly, outputFile, expectedCode, csharpDocument.GeneratedCode);
            ResourceFile.UpdateFile(_assembly, lineMappingOutputFile, expectedMappings, serializedMappings);
#else
            Assert.Equal(expectedCode, csharpDocument.GeneratedCode, ignoreLineEndingDifferences: true);
            Assert.Equal(expectedMappings, serializedMappings, ignoreLineEndingDifferences: true);
#endif
        }

        private static IRazorEngineFeature GetMetadataReferenceFeature()
        {
            var references = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                    .ToList<MetadataReference>();

            var feature = new DefaultMetadataReferenceFeature()
            {
                References = references,
            };

            return feature;
        }
    }
}
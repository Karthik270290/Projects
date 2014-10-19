// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RazorFileInfoCollectionGeneratorTest
    {
        private static readonly string _codeResultPath 
            = @"PreCompilation\TestFiles\collection._cs";
        private DateTime _now = new DateTime(2014, 10, 18);

        [Fact]
        public void GenerateCollectionSyntaxTreeFromInput()
        {
            // This test verifies there are no basic errors in the compilation.
            // It does not create a full assembly, because that will require more references
            // and will be a functional test.

            // Arrange
            var generator = new RazorFileInfoCollectionGenerator(
                Descriptors,
                SyntaxTreeGenerator.DefaultOptions);

            // Act
            var syntaxTree = generator.GenerateCollection();

            // Assert
            Assert.Empty(syntaxTree.GetDiagnostics());
        }

        [Fact]
        public void GenerateCollectionFromInput()
        {
            // Arrange
            var generator = new RazorFileInfoCollectionGenerator(
                Descriptors,
                SyntaxTreeGenerator.DefaultOptions);

            var expected = File.ReadAllText(_codeResultPath);

            // Act
            var sourceCode = generator.GenerateCode();

            // Assert
            Assert.Equal(expected, sourceCode);
        }

        public RazorFileInfo[] Descriptors
        {
            get
            {
                var descriptors = new List<RazorFileInfo>();

                var route1 = new RazorRoute("template1", "get");
                var route2 = new RazorRoute("template2", string.Empty);
                var route3 = new RazorRoute("template3", "post");
                var routes = new[] { route1, route2, route3 };

                var info1 = new RazorFileInfo()
                {
                    FullTypeName = "RazorPageDerivedType",
                    Hash = "Hash1",
                    LastModified = _now,
                    Length = 1000,
                    RelativePath = "relativePath1",
                    Routes = routes,
                };

                descriptors.Add(info1);

                var info2 = new RazorFileInfo()
                {
                    FullTypeName = "SecondType",
                    LastModified = _now,
                    Hash = "Hash2",
                    Length = 2000,
                    RelativePath = "relativePath2",
                    Routes = null,
                };

                descriptors.Add(info2);

                return descriptors.ToArray();
            }
        }
    }
}

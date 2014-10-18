// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNet.FileSystems;
using Microsoft.AspNet.Mvc.Razor;
using Xunit;

namespace Microsoft.AspNet.WebPages
{
    public class RazorRouteTest
    {
        public static IEnumerable<object[]> RouteTestData
        {
            get
            {
                var data = new[]
                {
                    new object [] { "WithRoute.cshtml", new[] { "myroute" } },
                    new object [] { "WithThreeRoutes.cshtml", new[] { "myroute1", "myroute2", "myroute3" } },
                    new object [] { "WithRouteToken.cshtml", new[] { "myroute/{foo}" } },
                    new object [] { "WithCatchAll.cshtml", new[] { "myroute/*foo" } },
                };

                return data;
            }
        }

        [Theory]
        [MemberData(nameof(RouteTestData))]
        public void RazorRouteFindsRoutesInFiles(string fileName, params string[] routes)
        {
            // Arrange
            var fileSystem = GetFileSystem();
            string path = fileName;

            IFileInfo fileInfo;
            Assert.True(fileSystem.TryGetFileInfo(path, out fileInfo));

            var relativeFileInfo = new RelativeFileInfo()
            {
                FileInfo = fileInfo,
                RelativePath = path,
            };

            // Act
            var foundRoutes = RazorRoute.GetRoutes(GetHost(), relativeFileInfo).ToArray();

            // Assert
            Assert.Equal(routes.Length, foundRoutes.Length);

            Array.Sort(foundRoutes);
            Array.Sort(routes);

            Assert.Equal(routes, foundRoutes);
        }

        private static IFileSystem GetFileSystem()
        {
            var currentFolder = Directory.GetCurrentDirectory();
            var fileSystem = new PhysicalFileSystem(currentFolder);

            return fileSystem;
        }

        private static IMvcRazorHost GetHost()
        {
            return new MvcRazorHost(GetFileSystem());
        }
    }
}
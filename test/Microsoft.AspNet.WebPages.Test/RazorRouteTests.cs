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
                    new object [] { "WithRoute.cshtml", new[] { "myroute" }, null },
                    new object [] { "WithThreeRoutes.cshtml",
                         new[] { "myroute1", "myroute2", "myroute3" }, null },
                    new object [] { "WithRouteToken.cshtml", new[] { "myroute/{foo}" }, null },
                    new object [] { "WithCatchAll.cshtml", new[] { "myroute/{*foo}" }, null },
                    new object [] { "WithGetPutPostDeletePatch.cshtml",
                        new[] { "getroute", "putroute", "postroute", "deleteroute", "patchroute" },
                        new[] { "GET", "PUT", "POST","DELETE", "PATCH"} },
                    new object [] { "WithGet.cshtml",
                        new [] { "route", "route/{foo}", "route/{*catchall}" },
                        new [] { "GET", "GET", "GET"}, },
                };

                return data;
            }
        }

        [Theory]
        [MemberData(nameof(RouteTestData))]
        public void RazorRouteFindsRoutesInFiles(string fileName, string[] routes, string[] verbs)
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

            var routesCopy = (string[])routes.Clone();
            Array.Sort(routesCopy);

            // Act
            var routesDictionary = RazorRoutes.GetRoutes(GetHost(), relativeFileInfo)
                                              .ToDictionary(r => r.RouteTemplate);

            var foundRoutes = routesDictionary.Keys
                                              .ToArray();

            // Assert
            Assert.Equal(routes.Length, foundRoutes.Length);

            Array.Sort(foundRoutes);

            Assert.Equal(routesCopy, foundRoutes);

            if (verbs != null)
            {
                Assert.Equal(routes.Length, verbs.Length);

                for (int i = 0; i < verbs.Length; i++)
                {
                    var routeKey = routes[i];
                    var route = routesDictionary[routeKey];

                    Assert.Equal(verbs[i], route.Verb);
                }
            }
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

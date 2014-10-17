// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Microsoft.AspNet.FileSystems;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RazorRouteTest
    {
        [Theory]
        [InlineData("WithCatchAll.cshtml", "myroute/*foo")]
        [InlineData("WithRoute.cshtml", "myroute")]
        [InlineData("WithRouteToken.cshtml", "myroute/{foo}")]
        public void RazorRouteFindsRoutesInFiles(string fileName, string[] routes)
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
// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.TestHost;
using Newtonsoft.Json;
using WebPagesWebSite;
using Xunit;

namespace Microsoft.AspNet.Mvc.FunctionalTests
{
    public class WebPagesControllerTests
    {
        private readonly IServiceProvider _provider = TestHelper.CreateServices("WebPagesWebSite");
        private readonly Action<IApplicationBuilder> _app = new Startup().Configure;

        [Fact]
        public async Task CanAccessController()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/plain; charset=utf-8");

            // Act

            var response = await client.GetAsync("http://localhost/Normal/NormalAction");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(NormalController.Response, responseContent);
        }

        [Fact]
        public async Task CanAccessAttributeRoutedController()
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/plain; charset=utf-8");

            // Act

            var response = await client.GetAsync("http://localhost" + AttributeRoutedController.Route);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(AttributeRoutedController.Response, responseContent);
        }

        [Theory]
        [InlineData("PagesWithoutRoutes/PageAtSubfolder", "Page at subfolder - PagesWithoutRoutes")]
        [InlineData("PageAtRoot", "Page at root")]
        [InlineData("routedpage/ValueFromTest", "routedpage - GET - ValueFromTest")]
        public async Task CanAccessPages(string urlSuffix, string content)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");

            // Act
            var response = await client.GetAsync("http://localhost/" + urlSuffix);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(content, responseContent);
        }

        [Theory]
        [InlineData("postedpage/ValueFromTest", "postedpage - POST - ValueFromTest")]
        public async Task CanAccessPagesWithPost(string urlSuffix, string content)
        {
            // Arrange
            var server = TestServer.Create(_provider, _app);
            var client = server.CreateClient();
            var expectedMediaType = MediaTypeHeaderValue.Parse("text/html; charset=utf-8");

            var postContent = new StringContent("input", Encoding.UTF8, "text/plain");

            // Act
            var response = await client.PostAsync("http://localhost/" + urlSuffix, postContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedMediaType, response.Content.Headers.ContentType);
            Assert.Equal(content, responseContent);
        }
    }
}

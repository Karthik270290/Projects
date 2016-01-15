// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.ViewComponents
{
    public class DefaultViewComponentActivatorTests
    {
        [Fact]
        public void DefaultViewComponentActivator_ActivatesViewComponentContext()
        {
            // Arrange
            var typeActivator = Mock.Of<ITypeActivatorCache>();
            var activator = new DefaultViewComponentActivator(typeActivator);

            var context = new ViewComponentContext();

            // Act
            var instance = activator.Create(context) as ViewComponent;

            // Assert
            Assert.NotNull(instance);
            Assert.Same(context, instance.ViewComponentContext);
        }

        [Fact]
        public void DefaultViewComponentActivator_ActivatesViewComponentContext_IgnoresNonPublic()
        {
            // Arrange
            var typeActivator = Mock.Of<ITypeActivatorCache>();
            var activator = new DefaultViewComponentActivator(typeActivator);

            var context = new ViewComponentContext();

            // Act
            var instance = activator.Create(context) as VisibilityViewComponent;

            // Assert
            Assert.NotNull(instance);
            Assert.Same(context, instance.ViewComponentContext);
            Assert.Null(instance.C);
        }

        private class TestViewComponent : ViewComponent
        {
            public Task ExecuteAsync()
            {
                throw new NotImplementedException();
            }
        }

        private class VisibilityViewComponent : ViewComponent
        {
            [ViewComponentContext]
            protected internal ViewComponentContext C { get; set; }
        }
    }
}

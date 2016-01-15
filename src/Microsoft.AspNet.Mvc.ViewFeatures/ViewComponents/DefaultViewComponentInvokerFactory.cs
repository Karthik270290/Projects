// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNet.Mvc.ViewComponents
{
    public class DefaultViewComponentInvokerFactory : IViewComponentInvokerFactory
    {
        private readonly IViewComponentActivator _viewComponentActivator;
        private readonly ILogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        public DefaultViewComponentInvokerFactory(
            IViewComponentActivator viewComponentActivator,
            DiagnosticSource diagnosticSource,
            ILoggerFactory loggerFactory)
        {
            if (viewComponentActivator == null)
            {
                throw new ArgumentNullException(nameof(viewComponentActivator));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _viewComponentActivator = viewComponentActivator;
            _diagnosticSource = diagnosticSource;

            _logger = loggerFactory.CreateLogger<DefaultViewComponentInvoker>();
        }

        /// <inheritdoc />
        // We don't currently make use of the descriptor or the arguments here (they are available on the context).
        // We might do this some day to cache which method we select, so resist the urge to 'clean' this without
        // considering that possibility.
        public IViewComponentInvoker CreateInstance(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new DefaultViewComponentInvoker(
                _viewComponentActivator,
                _diagnosticSource,
                _logger);
        }
    }
}

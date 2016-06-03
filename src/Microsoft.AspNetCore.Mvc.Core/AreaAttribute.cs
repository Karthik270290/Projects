// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// This attribute adds an 'area' Route Value to it's Controller or Action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AreaAttribute : RouteValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="AreaAttribute"/>.
        /// </summary>
        /// <param name="areaName">The name of the area.</param>
        public AreaAttribute(string areaName)
            : base("area", areaName)
        {
            if (string.IsNullOrEmpty(areaName))
            {
                throw new ArgumentException("Area name must not be empty", nameof(areaName));
            }
        }
    }
}

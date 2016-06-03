// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// This class or property is required when model binding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BindRequiredAttribute : BindingBehaviorAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="BindRequiredAttribute"/>
        /// </summary>
        public BindRequiredAttribute()
            : base(BindingBehavior.Required)
        {
        }
    }
}

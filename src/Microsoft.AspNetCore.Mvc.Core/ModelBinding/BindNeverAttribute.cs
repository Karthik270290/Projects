// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// Never model bind this class or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BindNeverAttribute : BindingBehaviorAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="BindNeverAttribute"/>.
        /// </summary>
        public BindNeverAttribute()
            : base(BindingBehavior.Never)
        {
        }
    }
}

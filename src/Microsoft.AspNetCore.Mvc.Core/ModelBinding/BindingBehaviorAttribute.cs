// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// Sets the binding behavior of a class or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BindingBehaviorAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="BindingBehaviorAttribute"/>
        /// </summary>
        /// <param name="behavior">The behavior to be enforced.</param>
        public BindingBehaviorAttribute(BindingBehavior behavior)
        {
            Behavior = behavior;
        }

        /// <summary>
        /// Get or sets the behavior to be enforced.
        /// </summary>
        public BindingBehavior Behavior { get; private set; }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Validation
{
    /// <summary>
    /// A common base class for <see cref="ModelValidationContext"/> and <see cref="ClientModelValidationContext"/>.
    /// </summary>
    public class ModelValidationContextBase
    {
        /// <summary>
        /// Instantiates a new <see cref="ModelValidationContextBase"/>.
        /// </summary>
        /// <param name="actionContext">The <see cref="ActionContext"/> for this context.</param>
        /// <param name="modelMetadata">The <see cref="ModelMetadata"/> for this model.</param>
        public ModelValidationContextBase(ActionContext actionContext, ModelMetadata modelMetadata)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (modelMetadata == null)
            {
                throw new ArgumentNullException(nameof(modelMetadata));
            }

            ActionContext = actionContext;
            ModelMetadata = modelMetadata;
        }

        /// <summary>
        /// Gets the <see cref="Mvc.ActionContext"/>.
        /// </summary>
        public ActionContext ActionContext { get; }

        /// <summary>
        /// Gets the <see cref="ModelBinding.ModelMetadata"/>.
        /// </summary>
        public ModelMetadata ModelMetadata { get; }
    }
}

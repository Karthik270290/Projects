// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    /// <summary>
    /// A default implementation of <see cref="IModelExpressionProvider"/>.
    /// </summary>
    public class ModelExpressionProvider : IModelExpressionProvider
    {
        private readonly ExpressionTextCache _expressionTextCache;

        /// <summary>
        /// Creates a  new <see cref="ModelExpressionProvider"/>.
        /// </summary>
        /// <param name="expressionTextCache">The <see cref="ExpressionTextCache"/>.</param>
        public ModelExpressionProvider(ExpressionTextCache expressionTextCache)
        {
            if (expressionTextCache == null)
            {
                throw new ArgumentNullException(nameof(expressionTextCache));
            }

            _expressionTextCache = expressionTextCache;
        }

        /// <inheritdoc />
        public ModelExpression CreateModelExpression<TModel, TValue>(
            ViewDataDictionary<TModel> viewData,
            Expression<Func<TModel, TValue>> expression)
        {
            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(viewData));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var name = ExpressionHelper.GetExpressionText(expression, _expressionTextCache);
            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, viewData);
            if (modelExplorer == null)
            {
                throw new InvalidOperationException(
                    Resources.FormatCreateModelExpression_NullModelMetadata(nameof(IModelMetadataProvider), name));
            }

            return new ModelExpression(name, modelExplorer);
        }
    }
}

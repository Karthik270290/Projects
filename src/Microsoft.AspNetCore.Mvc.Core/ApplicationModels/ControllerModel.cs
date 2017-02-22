// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Mvc.ApplicationModels
{
    [DebuggerDisplay("Name={ControllerName}, Type={ControllerType.Name}")]
    public class ControllerModel : ICommonModel, IFilterModel, IApiExplorerModel
    {
        private IList<PropertyModel> _controllerProperties = null;
        private readonly Lazy<List<PropertyModel>> _controllerPropertiesFactory = null;

        public ControllerModel(
            TypeInfo controllerType,
            IReadOnlyList<object> attributes)
        {
            if (controllerType == null)
            {
                throw new ArgumentNullException(nameof(controllerType));
            }

            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            ControllerType = controllerType;

            Actions = new List<ActionModel>();
            ApiExplorer = new ApiExplorerModel();
            Attributes = new List<object>(attributes);
            Filters = new List<IFilterMetadata>();
            Properties = new Dictionary<object, object>();
            RouteValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Selectors = new List<SelectorModel>();

            _controllerPropertiesFactory = new Lazy<List<PropertyModel>>(() => CreateProperties(controllerType.AsType(), this));
        }

        public ControllerModel(ControllerModel other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            ControllerName = other.ControllerName;
            ControllerType = other.ControllerType;

            // Still part of the same application
            Application = other.Application;

            // These are just metadata, safe to create new collections
            Attributes = new List<object>(other.Attributes);
            Filters = new List<IFilterMetadata>(other.Filters);
            RouteValues = new Dictionary<string, string>(other.RouteValues, StringComparer.OrdinalIgnoreCase);
            Properties = new Dictionary<object, object>(other.Properties);

            // Make a deep copy of other 'model' types.
            Actions = new List<ActionModel>(other.Actions.Select(a => new ActionModel(a)));
            ApiExplorer = new ApiExplorerModel(other.ApiExplorer);
            ControllerProperties =
                new List<PropertyModel>(other.ControllerProperties.Select(p => new PropertyModel(p)));
            Selectors = new List<SelectorModel>(other.Selectors.Select(s => new SelectorModel(s)));
        }

        public IList<ActionModel> Actions { get; }

        /// <summary>
        /// Gets or sets the <see cref="ApiExplorerModel"/> for this controller.
        /// </summary>
        /// <remarks>
        /// <see cref="ControllerModel.ApiExplorer"/> allows configuration of settings for ApiExplorer
        /// which apply to all actions in the controller unless overridden by <see cref="ActionModel.ApiExplorer"/>.
        ///
        /// Settings applied by <see cref="ControllerModel.ApiExplorer"/> override settings from
        /// <see cref="ApplicationModel.ApiExplorer"/>.
        /// </remarks>
        public ApiExplorerModel ApiExplorer { get; set; }

        public ApplicationModel Application { get; set; }

        public IReadOnlyList<object> Attributes { get; }

        MemberInfo ICommonModel.MemberInfo => ControllerType;

        string ICommonModel.Name => ControllerName;

        public string ControllerName { get; set; }

        public TypeInfo ControllerType { get; }

        public IList<PropertyModel> ControllerProperties
        {
            get
            {
                if (_controllerProperties == null)
                {
                    _controllerProperties = _controllerPropertiesFactory.Value;
                }
                return _controllerProperties;
            }
            private set
            {
                _controllerProperties = value;
            }
        }

        public IList<IFilterMetadata> Filters { get; }

        /// <summary>
        /// Gets a collection of route values that must be present in the 
        /// <see cref="RouteData.Values"/> for the corresponding action to be selected.
        /// </summary>
        /// <remarks>
        /// Entries in <see cref="RouteValues"/> can be overridden by entries in
        /// <see cref="ActionModel.RouteValues"/>.
        /// </remarks>
        public IDictionary<string, string> RouteValues { get; }

        /// <summary>
        /// Gets a set of properties associated with the controller.
        /// These properties will be copied to <see cref="Abstractions.ActionDescriptor.Properties"/>.
        /// </summary>
        /// <remarks>
        /// Entries will take precedence over entries with the same key
        /// in <see cref="ApplicationModel.Properties"/>.
        /// </remarks>
        public IDictionary<object, object> Properties { get; }

        public IList<SelectorModel> Selectors { get; }

        private static List<PropertyModel> CreateProperties(Type controllerType, ControllerModel controller)
        {
            var properties = new List<PropertyModel>();
            foreach (var propertyHelper in PropertyHelper.GetProperties(controllerType))
            {
                var propertyInfo = propertyHelper.Property;
                var propertyModel = CreatePropertyModel(propertyInfo);
                if (propertyModel != null)
                {
                    propertyModel.Controller = controller;
                    properties.Add(propertyModel);
                }
            }
            return properties;
        }

        private static PropertyModel CreatePropertyModel(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            // CoreCLR returns IEnumerable<Attribute> from GetCustomAttributes - the OfType<object>
            // is needed to so that the result of ToArray() is object
            var attributes = propertyInfo.GetCustomAttributes(inherit: true).OfType<object>().ToArray();
            var propertyModel = new PropertyModel(propertyInfo, attributes);
            var bindingInfo = BindingInfo.GetBindingInfo(attributes);

            propertyModel.BindingInfo = bindingInfo;
            propertyModel.PropertyName = propertyInfo.Name;

            return propertyModel;
        }
    }
}

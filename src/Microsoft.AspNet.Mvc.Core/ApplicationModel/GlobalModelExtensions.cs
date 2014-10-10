using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc.Description;
using Microsoft.AspNet.Mvc.Routing;

namespace Microsoft.AspNet.Mvc.ApplicationModel
{
    public static class GlobalModelExtensions
    {
        public static void AddControllerModel([NotNull] this GlobalModel applicationModel,
            [NotNull] TypeInfo controllerType,
            [NotNull] IActionDiscoveryConventions conventions)
        {
            var controllerModel = CreateControllerModel(applicationModel, controllerType);
            applicationModel.Controllers.Add(controllerModel);

            foreach (var methodInfo in controllerType.AsType().GetMethods())
            {
                var actionInfos = conventions.GetActions(methodInfo, controllerType);
                if (actionInfos == null)
                {
                    continue;
                }

                foreach (var actionInfo in actionInfos)
                {
                    var actionModel = CreateActionModel(controllerModel, methodInfo, actionInfo);
                    controllerModel.Actions.Add(actionModel);

                    foreach (var parameterInfo in methodInfo.GetParameters())
                    {
                        var parameterModel = CreateParameterModel(actionModel, parameterInfo);
                        actionModel.Parameters.Add(parameterModel);
                    }
                }
            }
        }

        private static ControllerModel CreateControllerModel(
            GlobalModel applicationModel,
            TypeInfo controllerType)
        {
            var controllerModel = new ControllerModel(controllerType)
            {
                Application = applicationModel,
            };

            controllerModel.ControllerName =
                controllerType.Name.EndsWith("Controller", StringComparison.Ordinal) ?
                    controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length) :
                    controllerType.Name;

            // CoreCLR returns IEnumerable<Attribute> from GetCustomAttributes - the OfType<object>
            // is needed to so that the result of ToList() is List<object>
            var attributes = controllerType.GetCustomAttributes(inherit: true).ToList();
            controllerModel.Attributes.AddRange(attributes);

            controllerModel.ActionConstraints.AddRange(attributes.OfType<IActionConstraintMetadata>());
            controllerModel.Filters.AddRange(attributes.OfType<IFilter>());
            controllerModel.RouteConstraints.AddRange(attributes.OfType<RouteConstraintAttribute>());

            controllerModel.AttributeRoutes.AddRange(
                attributes.OfType<IRouteTemplateProvider>().Select(rtp => new AttributeRouteModel(rtp)));

            var apiVisibility = attributes.OfType<IApiDescriptionVisibilityProvider>().FirstOrDefault();
            if (apiVisibility != null)
            {
                controllerModel.ApiExplorerIsVisible = !apiVisibility.IgnoreApi;
            }

            var apiGroupName = attributes.OfType<IApiDescriptionGroupNameProvider>().FirstOrDefault();
            if (apiGroupName != null)
            {
                controllerModel.ApiExplorerGroupName = apiGroupName.GroupName;
            }

            return controllerModel;
        }

        private static ActionModel CreateActionModel(
            ControllerModel controllerModel,
            MethodInfo methodInfo,
            ActionInfo actionInfo)
        {
            var actionModel = new ActionModel(methodInfo)
            {
                ActionName = actionInfo.ActionName,
                Controller = controllerModel,
                IsActionNameMatchRequired = actionInfo.RequireActionNameMatch,
            };

            var attributes = actionInfo.Attributes;

            actionModel.Attributes.AddRange(attributes);

            actionModel.ActionConstraints.AddRange(attributes.OfType<IActionConstraintMetadata>());
            actionModel.Filters.AddRange(attributes.OfType<IFilter>());

            var apiVisibility = attributes.OfType<IApiDescriptionVisibilityProvider>().FirstOrDefault();
            if (apiVisibility != null)
            {
                actionModel.ApiExplorerIsVisible = !apiVisibility.IgnoreApi;
            }

            var apiGroupName = attributes.OfType<IApiDescriptionGroupNameProvider>().FirstOrDefault();
            if (apiGroupName != null)
            {
                actionModel.ApiExplorerGroupName = apiGroupName.GroupName;
            }

            actionModel.HttpMethods.AddRange(actionInfo.HttpMethods ?? Enumerable.Empty<string>());

            if (actionInfo.AttributeRoute != null)
            {
                actionModel.AttributeRouteModel = new AttributeRouteModel(
                    actionInfo.AttributeRoute);
            }

            return actionModel;
        }

        private static ParameterModel CreateParameterModel(
            ActionModel actionModel,
            ParameterInfo parameterInfo)
        {
            var parameterModel = new ParameterModel(parameterInfo)
            {
                Action = actionModel,
            };

            // CoreCLR returns IEnumerable<Attribute> from GetCustomAttributes - the OfType<object>
            // is needed to so that the result of ToList() is List<object>
            var attributes = parameterInfo.GetCustomAttributes(inherit: true).OfType<object>().ToList();
            parameterModel.Attributes.AddRange(attributes);

            parameterModel.ParameterName = parameterInfo.Name;
            parameterModel.IsOptional = parameterInfo.HasDefaultValue;

            return parameterModel;
        }

        public static void ApplyConventions(this GlobalModel model, IEnumerable<IGlobalModelConvention> conventions)
        {
            // Conventions are applied from the outside-in to allow for scenarios where an action overrides
            // a controller, etc.
            foreach (var convention in conventions)
            {
                convention.Apply(model);
            }

            // First apply the conventions from attributes in decreasing order of scope.
            foreach (var controller in model.Controllers)
            {
                // ToArray is needed here to prevent issues with modifying the attributes collection
                // while iterating it.
                var controllerConventions =
                    controller.Attributes
                        .OfType<IControllerModelConvention>()
                        .ToArray();

                foreach (var controllerConvention in controllerConventions)
                {
                    controllerConvention.Apply(controller);
                }

                foreach (var action in controller.Actions)
                {
                    // ToArray is needed here to prevent issues with modifying the attributes collection
                    // while iterating it.
                    var actionConventions =
                        action.Attributes
                            .OfType<IActionModelConvention>()
                            .ToArray();

                    foreach (var actionConvention in actionConventions)
                    {
                        actionConvention.Apply(action);
                    }

                    foreach (var parameter in action.Parameters)
                    {
                        // ToArray is needed here to prevent issues with modifying the attributes collection
                        // while iterating it.
                        var parameterConventions =
                            parameter.Attributes
                                .OfType<IParameterModelConvention>()
                                .ToArray();

                        foreach (var parameterConvention in parameterConventions)
                        {
                            parameterConvention.Apply(parameter);
                        }
                    }
                }
            }
        }
    }
}

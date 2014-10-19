// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

public class AttributeRoutedController
{
    public const string Route = "/Attributed/Normal";
    public static string Response { get { return "Normal Attributed";} }

    [Route(Route)]
    public string NormalAttributedAction()
    {
        return Response;
    }
}

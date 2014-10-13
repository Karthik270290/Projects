// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using Microsoft.AspNet.FileSystems;
using Microsoft.CodeAnalysis;

namespace Microsoft.AspNet.Mvc.Razor
{
    public static class RazorRoute
    {
        public static string GetRoute(TypeInfo razorPage)
        {
            throw new NotImplementedException();
        }
        
        public static string GetRoute(IFileInfo fileInfo)
        {
            using (var stream = fileInfo.CreateReadStream())
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                for (int i = 0; i < 10 && !streamReader.EndOfStream; i++)
                {
                    var route = GetRoute(streamReader.ReadLine());
                    if (!string.IsNullOrEmpty(route))
                    {
                        return route;
                    }
                }
            }

            return null;
        }

        private static string GetRoute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            int index = input.IndexOf("@route ", StringComparison.Ordinal);

            if (index >= 0)
            {
                string route = input.Substring(7).Trim(new[] { ' ', '\t', '*', '@' });
                return route;
            }

            return null;
        }
    }
}

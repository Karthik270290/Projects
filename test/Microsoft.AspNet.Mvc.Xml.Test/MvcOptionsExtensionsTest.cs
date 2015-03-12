// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.AspNet.Mvc.Xml.Test
{
    public class MvcOptionsExtensionsTest
    {
        [Fact]
        public void AddXmlDataContractSerializerFormatter_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => MvcOptionsExtensions.AddXmlDataContractSerializerFormatter(null));
        }
    }
}
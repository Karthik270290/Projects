// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebSite.Models
{
    public class RemoteAttributeUser
    {
        public int Id { get; set; }

        // Controller in current area.
        [Required(ErrorMessage = "UserId1 is required")]
        public string UserId1 { get; set; }

        // Controller in root area.
        [Required(ErrorMessage = "UserId2 is required")]
        public string UserId2 { get; set; }

        [MinLength(6)]
        [Required(ErrorMessage = "UserId3 is required")]
        public string UserId3 { get; set; }

        [Required(ErrorMessage = "UserId4 is required")]
        public string UserId4 { get; set; }

        [Required(ErrorMessage = "UserId5 is required")]
        public string UserId5 { get; set; }
    }
}

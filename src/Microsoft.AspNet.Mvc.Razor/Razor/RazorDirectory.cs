// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNet.FileSystems;

namespace Microsoft.AspNet.Mvc.Razor
{
	public class RazorDirectory
	{
        private readonly string _extension;
        private readonly IFileSystem _fileSystem;

        public RazorDirectory([NotNull] IFileSystem fileSystem, string extension)
        {
            _fileSystem = fileSystem;
            _extension = extension;
        }

        public IEnumerable<RelativeFileInfo> GetFileInfos([NotNull] string relativePath)
		{
            IEnumerable<IFileInfo> fileInfos;
            string path = relativePath;

            if (!_fileSystem.TryGetDirectoryContents(path, out fileInfos))
            {
                yield break;
            }

            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.IsDirectory)
                {
                    var subPath = Path.Combine(path, fileInfo.Name);

                    foreach (var info in GetFileInfos(subPath))
                    {
                        yield return info;
                    }
                }
                else if (Path.GetExtension(fileInfo.Name)
                         .Equals(_extension, StringComparison.OrdinalIgnoreCase))
                {
                    var info = new RelativeFileInfo()
                    {
                        FileInfo = fileInfo,
                        RelativePath = Path.Combine(path, fileInfo.Name),
                    };

                    yield return info;
                }
            }
        }
	}
}

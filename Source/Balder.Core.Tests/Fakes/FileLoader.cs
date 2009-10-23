﻿using System;
using System.IO;
using Balder.Core.Content;
using Balder.Core.Interfaces;

namespace Balder.Core.Tests.Fakes
{
	public class FileLoader : IFileLoader
	{
		public IContentManager ContentManager { get; set; }
		public Game Game { get; set; }

		public Stream GetStream(string assetName)
		{
			return null;
		}
	}
}

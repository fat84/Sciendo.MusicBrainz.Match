﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Sciendo.FilesAnalyser;

namespace Sciendo.MusicBrainz.Match.Tests
{
    [TestFixture]
    public class FileSystemTests
    {
        [Test]
        public void GetFilesNoPath()
        {
            var fileLooper = new FileSystem();
            Assert.That(()=>fileLooper.GetFiles(default(string),null).ToArray(),Throws.ArgumentNullException);
        }

        [Test]
        public void GetFilesNoExtensions()
        {
            var fileLooper = new FileSystem();
            Assert.That(() => fileLooper.GetFiles("abc", null).ToArray(), Throws.ArgumentNullException);
        }
        [Test]
        public void GetFilesWrongPath()
        {
            var fileLooper = new FileSystem();
            Assert.That(() => fileLooper.GetFiles(@"..\abc", new [] {".mp3"}).ToArray(), Throws.ArgumentException);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Id3;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Rhino.Mocks;
using Sciendo.IOC;
using Sciendo.MusicBrainz.Match.Tests.Utils;

namespace Sciendo.MusicBrainz.Match.Tests
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void RunnerNoFileSystem()
        {
            Assert.That(() => new Runner(null, null, null,null,null), Throws.ArgumentNullException);
        }
        [Test]
        public void RunnerNoPath()
        {
            Assert.That(() => new Runner(new FileSystem(), null, null, null, null), Throws.ArgumentNullException);
        }
        [Test]
        public void RunnerNoExtensions()
        {
            Assert.That(() => new Runner(new FileSystem(), "abc", null, null, null), Throws.ArgumentNullException);
        }
        [Test]
        public void RunnerNoAnalyser()
        {
            Assert.That(() => new Runner(new FileSystem(), "abc", new [] {".mp3"}, null, null), Throws.ArgumentNullException);
        }

        [Test]
        public void RunnerWrongPath()
        {
            var mockAnalyser = MockRepository.GenerateStub<IAnalyser>();
            Assert.That(() => new Runner(new FileSystem(), "abc", new[] { ".mp3" }, mockAnalyser, "out.txt"), Throws.ArgumentException);
        }

        [Test]
        public void RunnerOk()
        {
            var mockAnalyser = MockRepository.GenerateStub<IAnalyser>();
            var runner = new Runner(new FileSystem(), "..", new[] { ".mp3" }, mockAnalyser, "out.txt");
            Assert.IsNotNull(runner.FilesAnalysed);
            Assert.IsEmpty(runner.FilesAnalysed);
        }

        [Test]
        public void StartNothingUnderPath()
        {
            var mockAnalyser = MockRepository.GenerateStub<IAnalyser>();
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            mockFileSystem.Expect(m => m.GetLeafDirectories("..")).Return(null);
            mockFileSystem.Expect(m => m.GetFiles("..", new[] {".mp3"})).Return(null);
            var runner = new Runner(mockFileSystem, "..", new[] { ".mp3" }, mockAnalyser);
            runner.Start();
            Assert.IsNotNull(runner.FilesAnalysed);
            Assert.IsEmpty(runner.FilesAnalysed);
        }
        [Test]
        public void StartOnlyFilesUnderPath()
        {
            var mockAnalyser = MockRepository.GenerateStub<IAnalyser>();
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            mockFileSystem.Expect(m => m.GetLeafDirectories("..")).Return(null);
            mockFileSystem.Expect(m => m.GetFiles("..", new[] { ".mp3" })).Return(new [] {"file1.mp3","file2.mp3"});
            mockFileSystem.Expect(m => m.FileExists("file1.mp3")).Return(true);
            mockFileSystem.Expect(m => m.FileExists("file2.mp3")).Return(true);
            mockAnalyser.Expect(m => m.Mp3IocKey).Return("Mp3IocKey");
            Container.GetInstance().Add(new RegisteredType().For<Mp3FileStub>().BasedOn<IMp3Stream>().With(LifeStyle.Transient).IdentifiedBy(mockAnalyser.Mp3IocKey));
            mockAnalyser.Expect(m => m.AnalyseFile(null, "file1.mp3")).Return(new FileAnalysed()).IgnoreArguments();
            mockAnalyser.Expect(m => m.AnalyseFile(null, "file2.mp3")).Return(new FileAnalysed()).IgnoreArguments();
            var runner = new Runner(mockFileSystem, "..", new[] { ".mp3" }, mockAnalyser);
            runner.Start();
            Assert.IsNotNull(runner.FilesAnalysed);
            Assert.IsNotEmpty(runner.FilesAnalysed);
            Assert.AreEqual(2,runner.FilesAnalysed.Count);

        }
        [Test]
        public void StartEmptyFoldersUnderPath()
        {
            var mockAnalyser = MockRepository.GenerateStub<IAnalyser>();
            var mockFileSystem = MockRepository.GenerateStub<IFileSystem>();
            mockFileSystem.Expect(m => m.GetLeafDirectories("..")).Return(new string[] {"f1","f2","f3"});
            mockFileSystem.Expect(m => m.GetFiles("..", new[] { ".mp3" })).Return(null).IgnoreArguments();

            var runner = new Runner(mockFileSystem, "..", new[] { ".mp3" }, mockAnalyser);
            runner.Start();
            Assert.IsNotNull(runner.FilesAnalysed);
            Assert.IsEmpty(runner.FilesAnalysed);
        }

    }
}

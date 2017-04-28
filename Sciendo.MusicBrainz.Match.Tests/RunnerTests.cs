using NUnit.Framework;
using Rhino.Mocks;
using Sciendo.FilesAnalyser;
using Sciendo.MusicMatch.Contracts;
using TagLib.Id3v2;

namespace Sciendo.MusicBrainz.Match.Tests
{
    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void RunnerNoFileSystem()
        {
            Assert.That(() => new Sciendo.FilesAnalyser.Runner(null, null, null,null,null), Throws.ArgumentNullException);
        }
        [Test]
        public void RunnerNoPath()
        {
            Assert.That(() => new Sciendo.FilesAnalyser.Runner(new FileSystem(), null, null, null, null), Throws.ArgumentNullException);
        }
        [Test]
        public void RunnerNoExtensions()
        {
            Assert.That(() => new Sciendo.FilesAnalyser.Runner(new FileSystem(), "abc", null, null, null), Throws.ArgumentNullException);
        }
        [Test]
        public void RunnerNoAnalyser()
        {
            Assert.That(() => new Sciendo.FilesAnalyser.Runner(new FileSystem(), "abc", new [] {".mp3"}, null, null), Throws.ArgumentNullException);
        }

        [Test]
        public void RunnerWrongPath()
        {
            var mockAnalyser = MockRepository.GenerateStub<IAnalyser>();
            Assert.That(() => new Sciendo.FilesAnalyser.Runner(new FileSystem(), "abc", new[] { ".mp3" }, mockAnalyser, "out.txt"), Throws.ArgumentException);
        }

        [Test]
        public void RunnerOk()
        {
            var mockAnalyser = MockRepository.GenerateStub<IAnalyser>();
            var runner = new Sciendo.FilesAnalyser.Runner(new FileSystem(), "..", new[] { ".mp3" }, mockAnalyser, "out.txt");
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
            var runner = new Sciendo.FilesAnalyser.Runner(mockFileSystem, "..", new[] { ".mp3" }, mockAnalyser);
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
            mockFileSystem.Expect(m => m.ReadTagFromFile("file1.mp3")).Return(new Tag());
            mockFileSystem.Expect(m => m.ReadTagFromFile("file2.mp3")).Return(new Tag());

            mockAnalyser.Expect(m => m.AnalyseFile("file1.mp3")).Return(new Music()).IgnoreArguments();
            mockAnalyser.Expect(m => m.AnalyseFile("file2.mp3")).Return(new Music()).IgnoreArguments();
            var runner = new Sciendo.FilesAnalyser.Runner(mockFileSystem, "..", new[] { ".mp3" }, mockAnalyser);
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

            var runner = new Sciendo.FilesAnalyser.Runner(mockFileSystem, "..", new[] { ".mp3" }, mockAnalyser);
            runner.Start();
            Assert.IsNotNull(runner.FilesAnalysed);
            Assert.IsEmpty(runner.FilesAnalysed);
        }

    }
}

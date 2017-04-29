using NUnit.Framework;
using Rhino.Mocks;
using Sciendo.FilesAnalyser;
using TagLib.Id3v2;

namespace Sciendo.FileAnalyser.Tests
{
    [TestFixture]
    public class FileAnalyserTests
    {
        [Test]
        public void FileAnalyserNoCollectionPaths()
        {
            Assert.That(() => new FilesAnalyser.FileAnalyser(null, null,null), Throws.ArgumentNullException);

        }

        [Test]
        public void FileAnalyserNoFileSystemInstance()
        {
            Assert.That(() => new FilesAnalyser.FileAnalyser(new [] {"abc"}, null,null), Throws.ArgumentNullException);

        }

        [Test]
        public void FileAnalyserNoCollectionMarker()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(false);
            Assert.That(() => new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock,null), Throws.ArgumentNullException);

        }

        [Test]
        public void FileAnalyserWrongCollectionPaths()
        {
            var collectionPaths = new string[] {"..", "abc"};
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(false);
            Assert.That(() => new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock,"some marker"), Throws.ArgumentException);
        }

        [Test]
        public void FileAnalyserCollectionPathsOk()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock,"some marker");
            for (int i = 0; i < collectionPaths.Length; i++)
            {
                Assert.AreEqual(collectionPaths[i],fileAnalyser.CollectionPaths[i]);
            }

        }

        [Test]
        public void FileAnalyserCollectionMarkerOk()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            Assert.AreEqual("some marker",fileAnalyser.CollectionMarker);
        }

        [Test]
        public void AnalyseFileNoMp3Stream()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock,"some marker");
            Assert.That(() => fileAnalyser.AnalyseFile(default(string)), Throws.ArgumentNullException);

        }

        [Test]
        public void AnalyseFileNoFilePath()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);

            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock,"some marker");
            Assert.That(() => fileAnalyser.AnalyseFile(default(string)), Throws.ArgumentNullException);

        }

        [Test]
        public void AnalyseFileWrongFilePath()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists("myownfile.mp3")).Return(false);

            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock, "some marker");
            Assert.That(() => fileAnalyser.AnalyseFile("myownfile.mp3"), Throws.ArgumentException);

        }

        [Test]
        public void AnalyseFileTagOkNoCollection()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists("myownfile.mp3")).Return(true);
            FileSystemMock.Expect(m => m.ReadTagFromFile("myownfile.mp3"))
                .Return(new Tag() {Title = "my song", Album = "my album", Artists = new[] {"my artist"}});
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock,"some marker");

            var fileAnalysed = fileAnalyser.AnalyseFile("myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Album.Name,"my album");
            Assert.AreEqual(fileAnalysed.Artist.Name,"my artist");
            Assert.AreEqual(fileAnalysed.Title.Name,"my song");
            Assert.AreEqual(fileAnalysed.FilePath,"myownfile.mp3");
            Assert.False(fileAnalysed.TagAnalysis.InCollectionPath);
            Assert.False(fileAnalysed.TagAnalysis.MarkedAsPartOfCollection);
            Assert.False(fileAnalysed.TagAnalysis.Id3TagIncomplete);
            
        }

        [Test]
        public void AnalyseFileTagIncomplete()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists("myownfile.mp3")).Return(true);
            FileSystemMock.Expect(m => m.ReadTagFromFile("myownfile.mp3"))
                .Return(new Tag() { Title = "my song"});
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var fileAnalysed = fileAnalyser.AnalyseFile( "myownfile.mp3");
            Assert.True(string.IsNullOrEmpty(fileAnalysed.Album.Name));
            Assert.True(string.IsNullOrEmpty(fileAnalysed.Artist.Name));
            Assert.AreEqual(fileAnalysed.Title.Name, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, "myownfile.mp3");
            Assert.False(fileAnalysed.TagAnalysis.InCollectionPath);
            Assert.False(fileAnalysed.TagAnalysis.MarkedAsPartOfCollection);
            Assert.True(fileAnalysed.TagAnalysis.Id3TagIncomplete);

        }

        [Test]
        public void AnalyseFileTagOkInCollection()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"..\myownfile.mp3")).Return(true);
            FileSystemMock.Expect(m => m.ReadTagFromFile(@"..\myownfile.mp3"))
    .Return(new Tag() { Title = "my song", Album = "my album", Artists = new[] { "my artist" },AlbumArtists = new []{"some marker"}});

            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var fileAnalysed = fileAnalyser.AnalyseFile( @"..\myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Album.Name, "my album");
            Assert.AreEqual(fileAnalysed.Artist.Name, "my artist");
            Assert.AreEqual(fileAnalysed.Title.Name, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"..\myownfile.mp3");
            Assert.True(fileAnalysed.TagAnalysis.InCollectionPath);
            Assert.True(fileAnalysed.TagAnalysis.MarkedAsPartOfCollection);
            Assert.False(fileAnalysed.TagAnalysis.Id3TagIncomplete);

        }

        [Test]
        public void AnalyseFileTagInCollectionShouldntBe()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"myownfile.mp3")).Return(true);
            FileSystemMock.Expect(m => m.ReadTagFromFile(@"myownfile.mp3"))
    .Return(new Tag() { Title = "my song", Album = "my album", Artists = new[] { "my artist" }, AlbumArtists = new[] { "some marker" } });
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var fileAnalysed = fileAnalyser.AnalyseFile(@"myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Album.Name, "my album");
            Assert.AreEqual(fileAnalysed.Artist.Name, "my artist");
            Assert.AreEqual(fileAnalysed.Title.Name, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"myownfile.mp3");
            Assert.False(fileAnalysed.TagAnalysis.InCollectionPath);
            Assert.True(fileAnalysed.TagAnalysis.MarkedAsPartOfCollection);
            Assert.False(fileAnalysed.TagAnalysis.Id3TagIncomplete);

        }

        [Test]
        public void AnalyseFileTagShouldbeInCollectionMarkedWrong()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"..\myownfile.mp3")).Return(true);
            FileSystemMock.Expect(m => m.ReadTagFromFile(@"..\myownfile.mp3"))
    .Return(new Tag() { Title = "my song", Album = "my album", Artists = new[] { "my artist" }, AlbumArtists = new[] { "some other marker" } });
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var fileAnalysed = fileAnalyser.AnalyseFile( @"..\myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Album.Name, "my album");
            Assert.AreEqual(fileAnalysed.Artist.Name, "my artist");
            Assert.AreEqual(fileAnalysed.Title.Name, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"..\myownfile.mp3");
            Assert.True(fileAnalysed.TagAnalysis.InCollectionPath);
            Assert.False(fileAnalysed.TagAnalysis.MarkedAsPartOfCollection);
            Assert.False(fileAnalysed.TagAnalysis.Id3TagIncomplete);

        }

        [Test]
        public void AnalyseFileTagShouldBeInCollectionNotMarked()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"..\myownfile.mp3")).Return(true);
            FileSystemMock.Expect(m => m.ReadTagFromFile(@"..\myownfile.mp3"))
    .Return(new Tag() { Title = "my song", Album = "my album", Artists = new[] { "my artist" }});
            var fileAnalyser = new FilesAnalyser.FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var fileAnalysed = fileAnalyser.AnalyseFile(@"..\myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Album.Name, "my album");
            Assert.AreEqual(fileAnalysed.Artist.Name, "my artist");
            Assert.AreEqual(fileAnalysed.Title.Name, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"..\myownfile.mp3");
            Assert.True(fileAnalysed.TagAnalysis.InCollectionPath);
            Assert.False(fileAnalysed.TagAnalysis.MarkedAsPartOfCollection);
            Assert.False(fileAnalysed.TagAnalysis.Id3TagIncomplete);

        }

    }
}

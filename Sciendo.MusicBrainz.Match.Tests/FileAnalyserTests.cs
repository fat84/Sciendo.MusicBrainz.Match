using System;
using Id3;
using Id3.Frames;
using Id3.Id3;
using NUnit.Framework;
using Rhino.Mocks;

namespace Sciendo.MusicBrainz.Match.Tests
{
    [TestFixture]
    public class FileAnalyserTests
    {
        [Test]
        public void FileAnalyserNoCollectionPaths()
        {
            Assert.That(() => new FileAnalyser(null, null,null), Throws.ArgumentNullException);

        }

        [Test]
        public void FileAnalyserNoFileSystemInstance()
        {
            Assert.That(() => new FileAnalyser(new [] {"abc"}, null,null), Throws.ArgumentNullException);

        }

        [Test]
        public void FileAnalyserNoCollectionMarker()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(false);
            Assert.That(() => new FileAnalyser(collectionPaths, FileSystemMock,null), Throws.ArgumentNullException);

        }

        [Test]
        public void FileAnalyserWrongCollectionPaths()
        {
            var collectionPaths = new string[] {"..", "abc"};
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(false);
            Assert.That(() => new FileAnalyser(collectionPaths, FileSystemMock,"some marker"), Throws.ArgumentException);
        }

        [Test]
        public void FileAnalyserCollectionPathsOk()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock,"some marker");
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
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            Assert.AreEqual("some marker",fileAnalyser.CollectionMarker);
        }

        [Test]
        public void AnalyseFileNoMp3Stream()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock,"some marker");
            Assert.That(() => fileAnalyser.AnalyseFile(null,default(string)), Throws.ArgumentNullException);

        }

        [Test]
        public void AnalyseFileNoFilePath()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            var mp3FileMock = MockRepository.GenerateStub<IMp3Stream>();

            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock,"some marker");
            Assert.That(() => fileAnalyser.AnalyseFile(mp3FileMock, default(string)), Throws.ArgumentNullException);

        }

        [Test]
        public void AnalyseFileWrongFilePath()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists("myownfile.mp3")).Return(false);
            var mp3FileMock = MockRepository.GenerateStub<IMp3Stream>();

            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock, "some marker");
            Assert.That(() => fileAnalyser.AnalyseFile(mp3FileMock, "myownfile.mp3"), Throws.ArgumentException);

        }

        [Test]
        public void AnalyseFileTagOkNoCollection()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists("myownfile.mp3")).Return(true);
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock,"some marker");

            var mp3FileMock = MockRepository.GenerateStub<IMp3Stream>();


            var mockTitleFrame = MockRepository.GenerateStub<TitleFrame>();
            mockTitleFrame.TextValue = "my song";
            var mockAlbumFrame = MockRepository.GenerateStub<AlbumFrame>();
            mockAlbumFrame.TextValue = "my album";
            var mockId3Tag = MockRepository.GenerateStub<IId3Tag>();
            mockId3Tag.Stub(m => m.Artists).Return(new ArtistsFrame {Value = { "my artist"}});
            mockId3Tag.Stub(m => m.Title).Return(mockTitleFrame);
            mockId3Tag.Stub(m => m.Album).Return(mockAlbumFrame);
            mp3FileMock.Expect(m => m.HasTags).Return(true);
            mp3FileMock.Expect(m => m.AvailableTagVersions).Return(new Version[] {new Version(3, 1)});
            mp3FileMock.Expect(m => m.GetTag(3,1)).Return(mockId3Tag);
            var fileAnalysed = fileAnalyser.AnalyseFile(mp3FileMock,"myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Id3Tag.Album.TextValue,"my album");
            Assert.AreEqual(fileAnalysed.Id3Tag.Artists.TextValue,"my artist");
            Assert.AreEqual(fileAnalysed.Id3Tag.Title.TextValue,"my song");
            Assert.AreEqual(fileAnalysed.FilePath,"myownfile.mp3");
            Assert.False(fileAnalysed.InCollectionPath);
            Assert.False(fileAnalysed.MarkedAsPartOfCollection);
            Assert.True(fileAnalysed.Id3TagComplete);
            
        }

        [Test]
        public void AnalyseFileTagOkInCollection()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"..\myownfile.mp3")).Return(true);
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var mp3FileMock = MockRepository.GenerateStub<IMp3Stream>();


            var mockTitleFrame = MockRepository.GenerateStub<TitleFrame>();
            mockTitleFrame.TextValue = "my song";
            var mockAlbumFrame = MockRepository.GenerateStub<AlbumFrame>();
            mockAlbumFrame.TextValue = "my album";
            var mockId3Tag = MockRepository.GenerateStub<IId3Tag>();
            mockId3Tag.Stub(m => m.Artists).Return(new ArtistsFrame { Value = { "my artist" } });
            mockId3Tag.Stub(m => m.Title).Return(mockTitleFrame);
            mockId3Tag.Stub(m => m.Album).Return(mockAlbumFrame);
            mockId3Tag.Stub(m => m.Band).Return(new BandFrame {Value = "some marker"});
            mp3FileMock.Expect(m => m.HasTags).Return(true);
            mp3FileMock.Expect(m => m.AvailableTagVersions).Return(new Version[] { new Version(3, 1) });
            mp3FileMock.Expect(m => m.GetTag(3, 1)).Return(mockId3Tag);
            var fileAnalysed = fileAnalyser.AnalyseFile(mp3FileMock, @"..\myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Id3Tag.Album.TextValue, "my album");
            Assert.AreEqual(fileAnalysed.Id3Tag.Artists.TextValue, "my artist");
            Assert.AreEqual(fileAnalysed.Id3Tag.Title.TextValue, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"..\myownfile.mp3");
            Assert.True(fileAnalysed.InCollectionPath);
            Assert.True(fileAnalysed.MarkedAsPartOfCollection);
            Assert.True(fileAnalysed.Id3TagComplete);

        }

        [Test]
        public void AnalyseFileTagInCollectionShouldntBe()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"myownfile.mp3")).Return(true);
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var mp3FileMock = MockRepository.GenerateStub<IMp3Stream>();


            var mockTitleFrame = MockRepository.GenerateStub<TitleFrame>();
            mockTitleFrame.TextValue = "my song";
            var mockAlbumFrame = MockRepository.GenerateStub<AlbumFrame>();
            mockAlbumFrame.TextValue = "my album";
            var mockId3Tag = MockRepository.GenerateStub<IId3Tag>();
            mockId3Tag.Stub(m => m.Artists).Return(new ArtistsFrame { Value = { "my artist" } });
            mockId3Tag.Stub(m => m.Title).Return(mockTitleFrame);
            mockId3Tag.Stub(m => m.Album).Return(mockAlbumFrame);
            mockId3Tag.Stub(m => m.Band).Return(new BandFrame { Value = "some marker" });
            mp3FileMock.Expect(m => m.HasTags).Return(true);
            mp3FileMock.Expect(m => m.AvailableTagVersions).Return(new Version[] { new Version(3, 1) });
            mp3FileMock.Expect(m => m.GetTag(3, 1)).Return(mockId3Tag);
            var fileAnalysed = fileAnalyser.AnalyseFile(mp3FileMock, @"myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Id3Tag.Album.TextValue, "my album");
            Assert.AreEqual(fileAnalysed.Id3Tag.Artists.TextValue, "my artist");
            Assert.AreEqual(fileAnalysed.Id3Tag.Title.TextValue, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"myownfile.mp3");
            Assert.False(fileAnalysed.InCollectionPath);
            Assert.True(fileAnalysed.MarkedAsPartOfCollection);
            Assert.True(fileAnalysed.Id3TagComplete);

        }

        [Test]
        public void AnalyseFileTagShouldbeInCollectionMarkedWrong()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"..\myownfile.mp3")).Return(true);
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var mp3FileMock = MockRepository.GenerateStub<IMp3Stream>();


            var mockTitleFrame = MockRepository.GenerateStub<TitleFrame>();
            mockTitleFrame.TextValue = "my song";
            var mockAlbumFrame = MockRepository.GenerateStub<AlbumFrame>();
            mockAlbumFrame.TextValue = "my album";
            var mockId3Tag = MockRepository.GenerateStub<IId3Tag>();
            mockId3Tag.Stub(m => m.Artists).Return(new ArtistsFrame { Value = { "my artist" } });
            mockId3Tag.Stub(m => m.Title).Return(mockTitleFrame);
            mockId3Tag.Stub(m => m.Album).Return(mockAlbumFrame);
            mockId3Tag.Stub(m => m.Band).Return(new BandFrame { Value = "some other marker" });
            mp3FileMock.Expect(m => m.HasTags).Return(true);
            mp3FileMock.Expect(m => m.AvailableTagVersions).Return(new Version[] { new Version(3, 1) });
            mp3FileMock.Expect(m => m.GetTag(3, 1)).Return(mockId3Tag);
            var fileAnalysed = fileAnalyser.AnalyseFile(mp3FileMock, @"..\myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Id3Tag.Album.TextValue, "my album");
            Assert.AreEqual(fileAnalysed.Id3Tag.Artists.TextValue, "my artist");
            Assert.AreEqual(fileAnalysed.Id3Tag.Title.TextValue, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"..\myownfile.mp3");
            Assert.True(fileAnalysed.InCollectionPath);
            Assert.False(fileAnalysed.MarkedAsPartOfCollection);
            Assert.True(fileAnalysed.Id3TagComplete);

        }

        [Test]
        public void AnalyseFileTagShouldBeInCollectionNotMarked()
        {
            var collectionPaths = new string[] { "..", "abc" };
            var FileSystemMock = MockRepository.GenerateStub<IFileSystem>();
            FileSystemMock.Expect(m => m.DirectoriesExist(collectionPaths)).Return(true);
            FileSystemMock.Expect(m => m.FileExists(@"..\myownfile.mp3")).Return(true);
            var fileAnalyser = new FileAnalyser(collectionPaths, FileSystemMock, "some marker");

            var mp3FileMock = MockRepository.GenerateStub<IMp3Stream>();


            var mockTitleFrame = MockRepository.GenerateStub<TitleFrame>();
            mockTitleFrame.TextValue = "my song";
            var mockAlbumFrame = MockRepository.GenerateStub<AlbumFrame>();
            mockAlbumFrame.TextValue = "my album";
            var mockId3Tag = MockRepository.GenerateStub<IId3Tag>();
            mockId3Tag.Stub(m => m.Artists).Return(new ArtistsFrame { Value = { "my artist" } });
            mockId3Tag.Stub(m => m.Title).Return(mockTitleFrame);
            mockId3Tag.Stub(m => m.Album).Return(mockAlbumFrame);
            mockId3Tag.Stub(m => m.Band).Return(null);
            mp3FileMock.Expect(m => m.HasTags).Return(true);
            mp3FileMock.Expect(m => m.AvailableTagVersions).Return(new Version[] { new Version(3, 1) });
            mp3FileMock.Expect(m => m.GetTag(3, 1)).Return(mockId3Tag);
            var fileAnalysed = fileAnalyser.AnalyseFile(mp3FileMock, @"..\myownfile.mp3");
            Assert.AreEqual(fileAnalysed.Id3Tag.Album.TextValue, "my album");
            Assert.AreEqual(fileAnalysed.Id3Tag.Artists.TextValue, "my artist");
            Assert.AreEqual(fileAnalysed.Id3Tag.Title.TextValue, "my song");
            Assert.AreEqual(fileAnalysed.FilePath, @"..\myownfile.mp3");
            Assert.True(fileAnalysed.InCollectionPath);
            Assert.False(fileAnalysed.MarkedAsPartOfCollection);
            Assert.True(fileAnalysed.Id3TagComplete);

        }

    }
}

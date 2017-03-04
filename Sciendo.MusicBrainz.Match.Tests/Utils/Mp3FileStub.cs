using System;
using System.Collections.Generic;
using Id3;
using Id3.Id3;

namespace Sciendo.MusicBrainz.Match.Tests.Utils
{
    public class Mp3FileStub:IMp3Stream
    {
        public Mp3FileStub(string file)
        {
            
        }
        public void DeleteAllTags()
        {
        }

        public void DeleteTag(Id3TagFamily family)
        {
            throw new NotImplementedException();
        }

        public void DeleteTag(int majorVersion, int minorVersion)
        {
        }

        public void Dispose()
        {
        }

        public IEnumerable<Id3Tag> GetAllTags()
        {
            return null;
        }

        public byte[] GetAudioStream()
        {
            return null;
        }

        public Id3Tag GetTag(Id3TagFamily family)
        {
            return null;
        }

        public IId3Tag GetTag(int majorVersion, int minorVersion)
        {
            return null;
        }

        public byte[] GetTagBytes(int majorVersion, int minorVersion)
        {
            return null;
        }

        public bool HasTagOfFamily(Id3TagFamily family)
        {
            return false;
        }

        public bool HasTagOfVersion(int majorVersion, int minorVersion)
        {
            return false;
        }

        public bool WriteTag(Id3Tag tag, WriteConflictAction conflictAction = WriteConflictAction.NoAction)
        {
            return false;
        }

        public bool WriteTag(Id3Tag tag, int majorVersion, int minorVersion, WriteConflictAction conflictAction = WriteConflictAction.NoAction)
        {
            return false;
        }

        public AudioStreamProperties Audio { get; }
        public IEnumerable<Version> AvailableTagVersions { get; }
        public bool HasTags { get; }
    }
}
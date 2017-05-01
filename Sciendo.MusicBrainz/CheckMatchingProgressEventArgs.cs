using System;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz
{
    public class CheckMatchingProgressEventArgs:EventArgs
    {
        public string File { get; private set; }

        public ExecutionStatus ExecutionStatus { get; private set; }

        public QueryType QueryType { get; private set; }

        public CheckMatchingProgressEventArgs(string file, ExecutionStatus executionStatus, QueryType queryType)
        {
            File = file;
            ExecutionStatus = executionStatus;
            QueryType = queryType;
        }
    }
}
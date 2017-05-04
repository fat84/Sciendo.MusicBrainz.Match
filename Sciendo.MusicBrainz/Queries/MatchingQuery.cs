using System;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public abstract class MatchingQuery
    {
        protected GraphClient GraphClient { get; }

        public abstract QueryType QueryType { get; }

        protected abstract string MatchingVersion { get;}

        protected abstract Guid ParentId { get;}

        protected abstract Guid SecondParentId { get; }
        public Music Music { get; private set; }
        protected MatchingQuery(GraphClient graphClient)
        {
            GraphClient = graphClient;
        }

        public MatchingQuery UsingMusic(Music music)
        {
            if(music==null)
                throw new ArgumentNullException(nameof(music));
            Music = music;
            return this;
        }

        protected abstract ICypherFluentQuery GetQuery();

        public abstract string GetQueryForDisplay();

        public abstract MBEntry Execute(string exactMatch);

        public void RecordExecutionStatus(ExecutionStatus executionStatus)
        {
            Music.Neo4jQuerries.ForEach(q =>
            {
                if (q.QueryType == QueryType) q.ExecutionStatus = executionStatus;
            });
        }

        public ExecutionStatus GetExecutionStatus()
        {
            var query = Music.Neo4jQuerries.FirstOrDefault(q => q.QueryType == QueryType);
            return query?.ExecutionStatus ?? ExecutionStatus.ExecutionError;
        }
    }
}

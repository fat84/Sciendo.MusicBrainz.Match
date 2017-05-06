using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using Sciendo.MusicBrainz.Queries.Matching;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public abstract class MusicBrainzQuery
    {
        protected GraphClient GraphClient { get; }

        public abstract QueryType QueryType { get; }

        protected abstract string ProcessedParameter { get; }

        protected abstract Guid ParentId { get; }

        protected abstract Guid SecondParentId { get; }
        public Music Music { get; protected set; }
        protected MusicBrainzQuery(GraphClient graphClient)
        {
            GraphClient = graphClient;
        }

        protected abstract ICypherFluentQuery<MBEntry> GetQuery();

        public string QueryText => GetQuery().Query.DebugQueryText;

        public abstract MBEntry Execute();

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

        public abstract Item CurrentItem { get; }

    }
}

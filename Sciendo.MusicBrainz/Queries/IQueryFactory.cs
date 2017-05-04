using System;
using Neo4jClient;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public interface IQueryFactory
    {
        MatchingQuery Get(QueryType queryType, GraphClient graphClient);
    }
}
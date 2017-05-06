using Neo4jClient;
using Sciendo.MusicBrainz.Queries.Matching;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries
{
    public interface IQueryFactory<out T> where T: MusicBrainzQuery
    {
        T Get(QueryType queryType, GraphClient graphClient);
    }
}
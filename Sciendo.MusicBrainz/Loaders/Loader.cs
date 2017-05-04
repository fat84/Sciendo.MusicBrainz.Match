using System;
using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicBrainz.Queries;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    public abstract class Loader
    {
        protected MatchingQuery MatchingQuery { get; private set; }
        protected ItemMemoryCache Cache { get; }

        protected abstract string CacheKey { get; }

        public Loader UsingMatchingQuery(MatchingQuery matchingQuery)
        {
            MatchingQuery = matchingQuery;
            return this;

        }
        protected Loader(ItemMemoryCache cache)
        {
            Cache = cache;
        }

        public ExecutionStatus Load()
        {
            MatchingQuery.Music.Neo4jQuerries.Add(new Neo4jQuery
            {
                ExecutionStatus = ExecutionStatus.NotExecuted,
                DebugQuery = MatchingQuery.GetQueryForDisplay(),
                QueryType = MatchingQuery.QueryType
            });
            try
            {
                if(Cache==null)
                    return LoadItemFromStore();
                return TryLoadItemFromCache()
                    ? MatchingQuery.GetExecutionStatus()
                    : LoadItemFromStore();
            }
            catch (Exception e)
            {
                return ExecutionStatus.ExecutionError;
            }

        }

        protected abstract Item CurrentItem { get; set; }

        private bool TryLoadItemFromCache()
        {
            var cachedItem = Cache.Get(CacheKey);
            if (cachedItem == null)
                return false;
            CurrentItem = cachedItem;
            MatchingQuery.RecordExecutionStatus(
                CurrentItem.Id != Guid.Empty ? ExecutionStatus.Found : ExecutionStatus.NotFound);
            return true;
        }

        private ExecutionStatus LoadItemFromStore()
        {
            var result = MatchingQuery.Execute(CurrentItem.Name);
            var executionStatus = ExecutionStatus.NotFound;
            if (result != null)
            {
                CurrentItem.Id = new Guid(result.mbid);
                CurrentItem.Name = result.name;
                CurrentItem.FixSuggestion = "No Fix needed.";
                executionStatus = ExecutionStatus.Found;
            }
            Cache?.Put(CacheKey, CurrentItem);
            MatchingQuery.RecordExecutionStatus(executionStatus);
            return executionStatus;
        }

    }
}

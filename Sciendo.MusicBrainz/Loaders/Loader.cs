using System;
using Sciendo.MusicBrainz.Cache;
using Sciendo.MusicBrainz.Queries;
using Sciendo.MusicBrainz.Queries.Matching;
using Sciendo.MusicBrainz.Queries.Merging;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Loaders
{
    public abstract class Loader
    {
        protected MatchingMusicBrainzQuery MatchingQuery { get; private set; }
        protected ItemMemoryCache Cache { get; }

        protected abstract string CacheKey { get; }

        public Loader UsingMatchingQuery(MatchingMusicBrainzQuery matchingQuery)
        {
            MatchingQuery = matchingQuery;
            return this;

        }

        public Loader UsingMergingQuery(MergingMusicBrainzQuery mergingQuery)
        {
            MergingQuery = mergingQuery;
            return this;
        }

        public MergingMusicBrainzQuery MergingQuery { get; private set; }

        protected Loader(ItemMemoryCache cache)
        {
            Cache = cache;
        }

        public ExecutionStatus Load()
        {
            var result = TryMatching();
            if (MergingQuery != null && result!=ExecutionStatus.Found)
                result = TryCreating();
            return result;
        }

        private ExecutionStatus TryCreating()
        {
            var result = ExecutionStatus.NotExecuted;

            MergingQuery.Music.Neo4jQuerries.Add(new Neo4jQuery
            {
                ExecutionStatus = result,
                DebugQuery = MergingQuery.QueryText,
                QueryType = MergingQuery.QueryType
            });
            try
            {
                result = LoadItemFromStore(MergingQuery, ExecutionStatus.ExecutionError);
            }
            catch (Exception e)
            {
                result = ExecutionStatus.ExecutionError;
            }
            return result;
        }

        private ExecutionStatus TryMatching()
        {
            var result = ExecutionStatus.NotExecuted;

            MatchingQuery.Music.Neo4jQuerries.Add(new Neo4jQuery
            {
                ExecutionStatus = result,
                DebugQuery = MatchingQuery.QueryText,
                QueryType = MatchingQuery.QueryType
            });
            try
            {
                if (Cache == null)
                    result = LoadItemFromStore(MatchingQuery,ExecutionStatus.NotFound);
                else
                    result = TryLoadItemFromCache()
                        ? MatchingQuery.GetExecutionStatus()
                        : LoadItemFromStore(MatchingQuery, ExecutionStatus.NotFound);
            }
            catch (Exception e)
            {
                result = ExecutionStatus.ExecutionError;
            }
            return result;
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

        private ExecutionStatus LoadItemFromStore(MusicBrainzQuery currentQuery, ExecutionStatus defaultExecutionStatus)
        {
            var result = currentQuery.Execute();
            var executionStatus = defaultExecutionStatus;
            if (result != null)
            {
                CurrentItem.Id = new Guid(result.mbid);
                CurrentItem.Name = result.name;
                CurrentItem.FixSuggestion = "No Fix needed.";
                executionStatus = ExecutionStatus.Found;
            }
            Cache?.Put(CacheKey, CurrentItem);
            currentQuery.RecordExecutionStatus(executionStatus);
            return executionStatus;
        }

    }
}

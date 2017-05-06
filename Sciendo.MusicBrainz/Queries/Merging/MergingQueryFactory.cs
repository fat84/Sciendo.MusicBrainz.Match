﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Sciendo.MusicMatch.Contracts;

namespace Sciendo.MusicBrainz.Queries.Merging
{
    public class MergingQueryFactory:IQueryFactory<MergingMusicBrainzQuery>
    {
        public MergingMusicBrainzQuery Get(QueryType queryType, GraphClient graphClient)
        {
            switch (queryType)
            {
                case QueryType.ArtistMerging:
                    return new MergingArtistQuery(graphClient,false);
                default:
                    return new MergingArtistQuery(graphClient,false);
            }

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.MusicMatch.Contracts
{
    public enum ApplyStatus
    {
        None,
        ApplyedToExistingInCollection,
        ApplyedToExistingNotInCollection,
        CreatedAndApplyedInCollection,
        CreatedAndApplyedNotInCollection,
        ErrorApplying
    }
}

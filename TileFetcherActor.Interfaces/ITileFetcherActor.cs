using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using System.IO;

namespace TileFetcherActor.Interfaces
{
    public interface ITileFetcherActor : IActor
    {
        Task<MemoryStream> FetchTile(string name, int x, int y, int zoom);
    }
}

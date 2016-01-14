using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace VerySimpleActor.Interfaces
{
    public interface IVerySimpleActor : IActor
    {
        Task<string> GetRandomValue();
    }
}

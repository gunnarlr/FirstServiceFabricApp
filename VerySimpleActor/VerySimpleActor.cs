using VerySimpleActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VerySimpleActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IVerySimpleActor interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by VerySimpleActor objects.
    /// </remarks>
    internal class VerySimpleActor : StatelessActor, IVerySimpleActor
    {
        Task<string> IVerySimpleActor.GetRandomValue()
        {
            // TODO: Replace the following with your own logic.
            ActorEventSource.Current.ActorMessage(this, "Doing Work");

            var dill = new Random();
            return Task.FromResult(dill.NextDouble().ToString());
        }
    }
}

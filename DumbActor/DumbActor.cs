using DumbActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DumbActor
{
    /// <remarks>
    /// Each ActorID maps to an instance of this class.
    /// The IDumbActor interface (in a separate DLL that client code can
    /// reference) defines the operations exposed by DumbActor objects.
    /// </remarks>
    internal class DumbActor : StatelessActor, IActor, IDumbActor
    {
        Task<string> IDumbActor.GetRandomValue()
        {
            // TODO: Replace the following with your own logic.
            ActorEventSource.Current.ActorMessage(this, "Doing Work");

            var dill = new Random();
            return Task.FromResult(dill.NextDouble().ToString());
        }
    }
}

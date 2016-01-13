using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstStatefulService.Interface
{
    using Microsoft.ServiceFabric.Services.Remoting;

    public interface ICounter: IService
    {
        Task<long> GetCountAsync();
    }
}

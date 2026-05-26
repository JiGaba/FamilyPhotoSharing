using ServiceLayer.BackgroundServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundInterfaces
{
    public interface IJobProcessor
    {
        Task ProcessAsync(BackgroundJob job, CancellationToken token);
    }
}

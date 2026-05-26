using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundServices
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly IBackgroundQueue _queue;
        private readonly BackgroundJobDispatcher _dispatcher;

        public BackgroundWorker(IBackgroundQueue queue, BackgroundJobDispatcher dispatcher)
        {
            _queue = queue;
            _dispatcher = dispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _queue.DequeueAsync(stoppingToken);
                var prosessor = _dispatcher.ResolveProcessor(job.JobType);
                
                await prosessor.ProcessAsync(job, stoppingToken);
            }
        }
    }

}

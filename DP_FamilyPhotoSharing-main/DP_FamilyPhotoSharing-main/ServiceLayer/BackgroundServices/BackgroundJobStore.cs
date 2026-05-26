using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundServices
{
    public interface IBackgroundJobStore
    {
        void Add(BackgroundJob job);
        BackgroundJob? Get(Guid id);
    }

    public class BackgroundJobStore : IBackgroundJobStore
    {
        private readonly ConcurrentDictionary<Guid, BackgroundJob> _jobs = new();

        public void Add(BackgroundJob job)
            => _jobs[job.JobId] = job;

        public BackgroundJob? Get(Guid id)
        {
            _jobs.TryGetValue(id, out var job);
            return job;
        }
    }

}

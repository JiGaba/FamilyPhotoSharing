using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundServices
{
    public interface IBackgroundQueue
    {
        void Enqueue(BackgroundJob job);
        Task<BackgroundJob> DequeueAsync(CancellationToken token);
    }

    public class BackgroundQueue : IBackgroundQueue
    {
        private readonly Channel<BackgroundJob> _channel = Channel.CreateUnbounded<BackgroundJob>();

        public void Enqueue(BackgroundJob job)
            => _channel.Writer.TryWrite(job);

        public async Task<BackgroundJob> DequeueAsync(CancellationToken token)
            => await _channel.Reader.ReadAsync(token);
    }

}

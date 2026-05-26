using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.BackgroundServices
{
    public static class JobStatus
    {
        public const string COMPLETED = "Completed";
        public const string FAILED = "Failed";
        public const string RUNNING = "Running";
        public const string PENDING = "Pending";
    }
}

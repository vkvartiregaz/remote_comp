using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Entities
{
    public enum Status
    {
        UNKNOWN, PROCESSING, QUEUED, RUNNING, COMPLETED, ABORTED, FAILED
    }
}

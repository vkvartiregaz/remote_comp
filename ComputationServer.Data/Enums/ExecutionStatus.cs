using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Enums
{
    public enum ExecutionStatus
    {
        UNKNOWN, PROCESSING, QUEUED, RUNNING, COMPLETED, ABORTED, FAILED
    }
}

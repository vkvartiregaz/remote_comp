using ComputationServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Nodes.Interfaces
{
    public interface IDataStorage
    {
        bool EnqueueTransfer(DataTransfer transfer);
        bool CancelTransfer(DataTransfer transfer);
        double TimeEstimate(DataTransfer transfer);
    }
}

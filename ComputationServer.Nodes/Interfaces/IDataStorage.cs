using ComputationServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Nodes.Interfaces
{
    public interface IDataStorage
    {
        bool RequestData(string id, DataType type);
        bool TransferData(string id, string uri);
        bool UploadData(string id, DataType type, byte[] data);
    }
}

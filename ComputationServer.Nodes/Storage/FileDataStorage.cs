using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Nodes
{
    public class FileDataStorage : IDataStorage
    {
        public bool RequestData(string id, DataType type)
        {
            throw new NotImplementedException();
        }

        public bool TransferData(string id, string uri)
        {
            throw new NotImplementedException();
        }

        public bool UploadData(string id, DataType type, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}

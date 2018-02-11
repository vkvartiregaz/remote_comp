using ComputationServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Interfaces
{
    public interface IOperationRepository
    {
        bool Add(Operation operation);
        bool Update(Operation operation);
        Operation Find(string opId);
        bool Delete(Operation operation);
    }
}

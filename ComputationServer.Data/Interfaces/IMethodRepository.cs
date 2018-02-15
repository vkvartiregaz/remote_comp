using ComputationServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Interfaces
{
    public interface IMethodRepository
    {
        Method Find(string name);
    }
}

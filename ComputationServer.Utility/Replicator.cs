using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Utility
{
    public class Replicator
    {
        public static List<T> CopyAll<T>(params List<T>[] data) where T : class, ICloneable
        {
            var result = new List<T>();
            
            foreach(var list in data)
                foreach(var item in list)
                    result.Add(item.Clone() as T);

            return result;                    
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader
{
    using System.Data;
    using LightInject;

    public static class ReaderCache<T>
    {
        private static ImmutableHashTree<string, Func<IDataRecord, int[], T>> tree =
            ImmutableHashTree<string, Func<IDataRecord, int[], T>>.Empty;

        public static Func<IDataRecord, int[], T> Get(string sql)
        {
            return tree.Search(sql);
        }

        public static void Add(Func<IDataRecord, int[], T> method, string sql)
        {
            tree = tree.Add(sql, method);
        }
    }



    
}

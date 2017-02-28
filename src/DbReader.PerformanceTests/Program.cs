using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader.PerformanceTests
{
    using Tests;

    class Program
    {
        static void Main(string[] args)
        {
            IntegrationTests integrationTests = new IntegrationTests();
            //integrationTests.DbReaderVsDapper();
            Console.ReadKey();
        }
    }
}

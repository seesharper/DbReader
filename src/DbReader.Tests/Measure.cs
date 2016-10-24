using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbReader.Tests
{
    using System.Diagnostics;

    public class Measure
    {
        public static Report Run(Action action, int numberOfRuns ,string name, bool warmup = true)
        {
            if (warmup)
            {
                action();
            }


            double[] times = new double[numberOfRuns];    
            

            var result = new Report();
            result.MemoryStart = GC.GetTotalMemory(true);
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < numberOfRuns; i++)
            {
                action();
                times[i] = sw.Elapsed.TotalMilliseconds;    
                sw.Restart();
                
                
            }            
            sw.Stop();

            result.Name = name;
            result.NumberOfRuns = numberOfRuns;
            result.MemoryEnd = GC.GetTotalMemory(false);
            result.MemoryAfterCollect = GC.GetTotalMemory(true);            
            result.StandardDeviation = CalculateStandardDeviation(times);
            result.Total = CalculateTotal(times);
            result.Longest = CalculateLongest(times);
            result.Shortest = CalculateShortest(times);
            result.Mean = CalculateMean(times);
            result.Percentiles = new Percentile[9];
            result.Percentiles[0] = new Percentile() {Value = Percentile(times, 0.5), Percent = 0.5};
            result.Percentiles[1] = new Percentile() { Value = Percentile(times, 0.66), Percent = 0.66 };
            result.Percentiles[2] = new Percentile() { Value = Percentile(times, 0.75), Percent = 0.75 };
            result.Percentiles[3] = new Percentile() { Value = Percentile(times, 0.80), Percent = 0.80 };
            result.Percentiles[4] = new Percentile() { Value = Percentile(times, 0.90), Percent = 0.90 };
            result.Percentiles[5] = new Percentile() { Value = Percentile(times, 0.95), Percent = 0.95 };
            result.Percentiles[6] = new Percentile() { Value = Percentile(times, 0.98), Percent = 0.98 };
            result.Percentiles[7] = new Percentile() { Value = Percentile(times, 0.99), Percent = 0.99 };
            result.Percentiles[8] = new Percentile() { Value = Percentile(times, 1), Percent = 1.0 };
            
            

            return result;

        }

        private static double CalculateTotal(double[] numbers)
        {
            return numbers.Sum();
        }

        private static double CalculateLongest(double[] numbers)
        {
            return numbers.Max();
        }

        private static double CalculateShortest(double[] numbers)
        {
            return numbers.Min();
        }

        private static double CalculateStandardDeviation(double[] numbers)
        {
            double average = numbers.Average();
            double sumOfSquaresOfDifferences = numbers.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / numbers.Length);
            return sd;
        }

        private static double CalculateMean(double[] numbers)
        {
            return numbers.Average();
        }

        private static double Percentile(double[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;            
            if (n == 1d) return sequence[0];
            if (n == N) return sequence[N - 1];
            int k = (int)n;
            double d = n - k;
            return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
        }
    }

    public struct Percentile
    {
        public double Percent;
        public double Value;

        public override string ToString()
        {
            return $"{Percent*100:0}%\t\t{Value:0.00}";
        }
    }


    public class Report
    {
        public double Total;
        public int NumberOfRuns;
        public double StandardDeviation;
        public Percentile[] Percentiles;
        public double Longest;
        public double Shortest;
        public double Mean;
        public string Name;
        public long MemoryStart;
        public long MemoryEnd;
        public long MemoryAfterCollect;
        



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"****** {Name} ******");
            sb.AppendLine($"Number of runs: {NumberOfRuns}");
            sb.AppendLine($"Total time: {Total:0.00} ms");            
            sb.AppendLine($"Standard Deviation: {StandardDeviation:0.00} ms");
            sb.AppendLine($"Mean: {Mean:0.00} ms");
            sb.AppendLine($"Longest: {Longest:0.00} ms");
            sb.AppendLine($"Shortest: {Shortest:0.00} ms");

            sb.AppendLine($"------ Memory ------");
            sb.AppendLine($"Memory (start): {MemoryStart} bytes");
            sb.AppendLine($"Memory (end): {MemoryEnd} bytes");
            sb.AppendLine($"Memory (allocated) {MemoryEnd - MemoryStart} bytes");
            sb.AppendLine($"Memory (after collect): {MemoryAfterCollect} bytes");
            sb.AppendLine();
            sb.AppendLine("Percentage of the requests served within a certain time (ms)");
            foreach (var percentile in Percentiles)
            {
                sb.AppendLine(percentile.ToString());
            }


            return sb.ToString();
        }
    }

    
}

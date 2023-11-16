using BenchmarkDotNet.Running;

namespace EntityMerger.Benchmark;

public class Program
{
    // open console and run
    //  dotnet run -c Release
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<LoadNoNavigation>();
        //var summary = BenchmarkRunner.Run<EntityComparer>();
    }
}

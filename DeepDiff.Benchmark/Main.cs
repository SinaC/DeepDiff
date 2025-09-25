using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace DeepDiff.Benchmark;

public class Program
{
    // open console and run
    //  dotnet run -c Release compare|hash|navigation|nonavigation
    public static void Main(string[] args)
    {
        //https://stackoverflow.com/questions/73475521/benchmarkdotnet-inprocessemittoolchain-complete-sample
        var config = DefaultConfig.Instance
            //.AddJob(
            //    Job
            //    .LongRun
            //    .WithLaunchCount(1)
            //    .WithToolchain(InProcessNoEmitToolchain.Instance));
            .AddJob(Job.Default);

        var summary = args[1].ToLowerInvariant() switch
        {
            "compare" => BenchmarkRunner.Run<Compare>(config),
            "hash" => BenchmarkRunner.Run<Hash>(config),
            "navigation" => BenchmarkRunner.Run<LoadNavigation>(config),
            "nonavigation" => BenchmarkRunner.Run<LoadNoNavigation>(config),
            _ => throw new ArgumentOutOfRangeException($"Unknown benchmark: {args[1]}")
        };
    }
}

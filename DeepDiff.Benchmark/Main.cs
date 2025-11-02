using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace DeepDiff.Benchmark;

public class Program
{
    // open console and run
    //  dotnet run -c Release hashthreshold|navigation|nonavigation
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

        var summary = args[0].ToLowerInvariant() switch
        {
            "hashthreshold" => BenchmarkRunner.Run<HashThreshold>(config),
            "navigation" => BenchmarkRunner.Run<LoadNavigation>(config),
            "nonavigation" => BenchmarkRunner.Run<LoadNoNavigation>(config),
            _ => throw new ArgumentOutOfRangeException($"Unknown benchmark: {args[0]}")
        };
    }
}

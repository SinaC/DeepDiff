using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace DeepDiff.POC.Benchmark;

public class Program
{
    // open console and run
    //  dotnet run -c Release compare|hash|value
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
            "compare" => BenchmarkRunner.Run<Compare>(config),
            "hash" => BenchmarkRunner.Run<Hash>(config),
            "value" => BenchmarkRunner.Run<GetAndSetValue>(config),
            _ => throw new ArgumentOutOfRangeException($"Unknown benchmark: {args[0]}")
        };
    }
}

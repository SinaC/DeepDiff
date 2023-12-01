using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace DeepDiff.Benchmark;

public class Program
{
    // open console and run
    //  dotnet run -c Release
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

        var summary = BenchmarkRunner.Run<LoadNoNavigation>(config);
        //var summary = BenchmarkRunner.Run<DeepDiff>(config);
    }
}

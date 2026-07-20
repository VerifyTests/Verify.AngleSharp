using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Legacy).Assembly).Run(args);

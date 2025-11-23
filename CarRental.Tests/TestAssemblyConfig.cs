using Xunit;

// Disable parallel execution for the whole test assembly to keep SQLite integration tests isolated.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

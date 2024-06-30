using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

/*
the most optimized methods for checking the number is prime is IsPrimeOptimized and IsPrimeOptimized3

the result of benchmarking is at the end of this file
*/

BenchmarkRunner.Run<Benchmarker>();

public class Benchmarker
{
    private static readonly int[] _steps = [4, 2, 4, 2, 4, 6, 2, 6];

    [ParamsSource(nameof(Values))] public long ValueToTest;

    public IEnumerable<long> Values => new[] { 100, 200, long.MaxValue, 107, 2147483647, 98867, 98893, 13171 * 13171 };

    [Benchmark(Baseline = true)]
    public bool IsPrimeStd()
    {
        var min = (long)(Math.Sqrt(ValueToTest) + 0.1f);

        for (var i = 2L; i <= min; i++)
            if (ValueToTest % i == 0)
                return false;

        return true;
    }

    [Benchmark]
    public bool IsPrimeOptimized()
    {
        if (ValueToTest is 0 or 2 or 3 or 5)
            return true;

        if (ValueToTest % 2 == 0 || ValueToTest % 3 == 0 || ValueToTest % 5 == 0)
            return false;

        var min = (long)(Math.Sqrt(ValueToTest) + 0.1f);
        var steps = _steps.AsSpan();
        var ind = 0;

        for (var i = 7L; i <= min; i += steps[ind], ind = (ind + 1) % steps.Length)
            if (ValueToTest % i == 0)
                return false;

        return true;
    }

    [Benchmark]
    public bool IsPrimeOptimized2()
    {
        if (ValueToTest is 0 or 2 or 3 or 5)
            return true;

        if (ValueToTest % 2 == 0 || ValueToTest % 3 == 0 || ValueToTest % 5 == 0)
            return false;

        var min = (long)(Math.Sqrt(ValueToTest) + 0.1f);
        var steps = _steps.AsSpan();

        for (var i = 7L; i <= min;)
            foreach (var step in steps)
            {
                if (ValueToTest % i == 0)
                    return false;
                i += step;
            }

        return true;
    }

    [Benchmark]
    public bool IsPrimeOptimized3()
    {
        if (ValueToTest is 0 or 2 or 3 or 5)
            return true;

        if (ValueToTest % 2 == 0 || ValueToTest % 3 == 0 || ValueToTest % 5 == 0)
            return false;

        var min = (long)(Math.Sqrt(ValueToTest) + 0.1f);
        var steps = _steps.AsSpan();

        var i = 7L;
        while (true)
            foreach (var step in steps)
            {
                if (i <= min)
                {
                    if (ValueToTest % i == 0)
                        return false;
                }
                else
                {
                    return true;
                }

                i += step;
            }
    }
}

/*
| Method            | ValueToTest         | Mean            | Error         | StdDev      | Ratio |
|------------------ |-------------------- |----------------:|--------------:|------------:|------:|
| IsPrimeStd        | 100                 |       7.7339 ns |     0.0443 ns |   0.0393 ns |  1.00 |
| IsPrimeOptimized  | 100                 |       0.3439 ns |     0.0109 ns |   0.0102 ns |  0.04 |
| IsPrimeOptimized2 | 100                 |       0.4834 ns |     0.0091 ns |   0.0085 ns |  0.06 |
| IsPrimeOptimized3 | 100                 |       0.3501 ns |     0.0042 ns |   0.0037 ns |  0.05 |
|                   |                     |                 |               |             |       |
| IsPrimeStd        | 107                 |      74.2137 ns |     0.3802 ns |   0.3556 ns |  1.00 |
| IsPrimeOptimized  | 107                 |      14.8403 ns |     0.0631 ns |   0.0591 ns |  0.20 |
| IsPrimeOptimized2 | 107                 |      70.2376 ns |     0.2285 ns |   0.2138 ns |  0.95 |
| IsPrimeOptimized3 | 107                 |      13.7058 ns |     0.0872 ns |   0.0773 ns |  0.18 |
|                   |                     |                 |               |             |       |
| IsPrimeStd        | 200                 |       7.6287 ns |     0.0235 ns |   0.0220 ns |  1.00 |
| IsPrimeOptimized  | 200                 |       0.3450 ns |     0.0080 ns |   0.0071 ns |  0.05 |
| IsPrimeOptimized2 | 200                 |       0.4770 ns |     0.0086 ns |   0.0072 ns |  0.06 |
| IsPrimeOptimized3 | 200                 |       0.3409 ns |     0.0124 ns |   0.0116 ns |  0.04 |
|                   |                     |                 |               |             |       |
| IsPrimeStd        | 98867               |   2,653.4416 ns |    10.8306 ns |  10.1309 ns |  1.00 |
| IsPrimeOptimized  | 98867               |     904.7079 ns |     2.8636 ns |   2.6786 ns |  0.34 |
| IsPrimeOptimized2 | 98867               |     758.3989 ns |     4.6504 ns |   4.3500 ns |  0.29 |
| IsPrimeOptimized3 | 98867               |     726.9455 ns |    14.0209 ns |  15.0022 ns |  0.27 |
|                   |                     |                 |               |             |       |
| IsPrimeStd        | 98893               |   2,665.8223 ns |    13.3977 ns |  12.5322 ns |  1.00 |
| IsPrimeOptimized  | 98893               |     907.4732 ns |     3.0338 ns |   2.6894 ns |  0.34 |
| IsPrimeOptimized2 | 98893               |     759.7903 ns |     0.7812 ns |   0.6099 ns |  0.29 |
| IsPrimeOptimized3 | 98893               |     705.7275 ns |     3.6823 ns |   3.0749 ns |  0.26 |
|                   |                     |                 |               |             |       |
| IsPrimeStd        | 173475241           | 111,858.9429 ns |   304.3450 ns | 284.6845 ns |  1.00 |
| IsPrimeOptimized  | 173475241           |  38,139.5721 ns |   163.1800 ns | 152.6387 ns |  0.34 |
| IsPrimeOptimized2 | 173475241           |  30,328.2440 ns |    64.8192 ns |  50.6066 ns |  0.27 |
| IsPrimeOptimized3 | 173475241           |  30,519.9634 ns |    86.4494 ns |  80.8648 ns |  0.27 |
|                   |                     |                 |               |             |       |
| IsPrimeStd        | 2147483647          | 394,463.5874 ns | 1,078.4851 ns | 900.5839 ns |  1.00 |
| IsPrimeOptimized  | 2147483647          | 133,101.1572 ns |   522.9547 ns | 489.1722 ns |  0.34 |
| IsPrimeOptimized2 | 2147483647          | 106,476.4858 ns |   431.3626 ns | 382.3917 ns |  0.27 |
| IsPrimeOptimized3 | 2147483647          | 110,768.1230 ns |   321.1848 ns | 300.4364 ns |  0.28 |
|                   |                     |                 |               |             |       |
| IsPrimeStd        | 9223372036854775807 |      50.5011 ns |     0.2616 ns |   0.2447 ns |  1.00 |
| IsPrimeOptimized  | 9223372036854775807 |      12.7551 ns |     0.0584 ns |   0.0546 ns |  0.25 |
| IsPrimeOptimized2 | 9223372036854775807 |      13.0894 ns |     0.0529 ns |   0.0442 ns |  0.26 |
| IsPrimeOptimized3 | 9223372036854775807 |      13.2510 ns |     0.0405 ns |   0.0379 ns |  0.26 |
*/
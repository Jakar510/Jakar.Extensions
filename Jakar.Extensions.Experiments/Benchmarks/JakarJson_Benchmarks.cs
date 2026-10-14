// Jakar.Extensions :: Experiments
// 10/07/2026

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using JJ = Jakar.Json;
using NewtonsoftSerializer = Newtonsoft.Json.JsonSerializer;



namespace Experiments.Benchmarks;
/*

Mean = 1.507 ms, StdErr = 0.007 ms (0.48%), N = 18, StdDev = 0.031 ms
Min = 1.425 ms, Q1 = 1.508 ms, Median = 1.515 ms, Q3 = 1.523 ms, Max = 1.537 ms
IQR = 0.015 ms, LowerFence = 1.486 ms, UpperFence = 1.545 ms
ConfidenceInterval = [1.478 ms; 1.536 ms] (CI 99.9%), Margin = 0.029 ms (1.91% of Mean)
Skewness = -1.53, Kurtosis = 4.35, MValue = 2

// ** Remained 0 (0.0%) benchmark(s) to run. Estimated finish 2026-10-13 21:41 (0h 0m from now) **
Successfully reverted power plan (GUID: 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c FriendlyName: High performance)
// ***** BenchmarkRunner: Finish  *****

// * Export *
  BenchmarkDotNet.Artifacts\results\Experiments.Benchmarks.JakarJson_Benchmarks-report.csv
  BenchmarkDotNet.Artifacts\results\Experiments.Benchmarks.JakarJson_Benchmarks-report-github.md
  BenchmarkDotNet.Artifacts\results\Experiments.Benchmarks.JakarJson_Benchmarks-report.html

// * Detailed results *
JakarJson_Benchmarks.StjNode_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.485 us, StdErr = 0.003 us (0.18%), N = 14, StdDev = 0.010 us
Min = 1.466 us, Q1 = 1.477 us, Median = 1.487 us, Q3 = 1.491 us, Max = 1.500 us
IQR = 0.014 us, LowerFence = 1.456 us, UpperFence = 1.511 us
ConfidenceInterval = [1.473 us; 1.496 us] (CI 99.9%), Margin = 0.012 us (0.78% of Mean)
Skewness = -0.27, Kurtosis = 1.85, MValue = 2
-------------------- Histogram --------------------
[1.460 us ; 1.506 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJsonNode_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.648 us, StdErr = 0.003 us (0.19%), N = 15, StdDev = 0.012 us
Min = 1.631 us, Q1 = 1.639 us, Median = 1.646 us, Q3 = 1.655 us, Max = 1.678 us
IQR = 0.016 us, LowerFence = 1.615 us, UpperFence = 1.680 us
ConfidenceInterval = [1.635 us; 1.661 us] (CI 99.9%), Margin = 0.013 us (0.79% of Mean)
Skewness = 0.84, Kurtosis = 3.03, MValue = 2
-------------------- Histogram --------------------
[1.627 us ; 1.684 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.NewtonsoftJObject_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 3.140 us, StdErr = 0.012 us (0.38%), N = 13, StdDev = 0.044 us
Min = 3.016 us, Q1 = 3.135 us, Median = 3.146 us, Q3 = 3.158 us, Max = 3.196 us
IQR = 0.023 us, LowerFence = 3.100 us, UpperFence = 3.193 us
ConfidenceInterval = [3.088 us; 3.192 us] (CI 99.9%), Margin = 0.052 us (1.66% of Mean)
Skewness = -1.53, Kurtosis = 5.49, MValue = 2
-------------------- Histogram --------------------
[2.991 us ; 3.094 us) | @
[3.094 us ; 3.220 us) | @@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.StjNode_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 56.317 us, StdErr = 0.126 us (0.22%), N = 13, StdDev = 0.455 us
Min = 55.328 us, Q1 = 56.136 us, Median = 56.275 us, Q3 = 56.488 us, Max = 57.288 us
IQR = 0.352 us, LowerFence = 55.607 us, UpperFence = 57.017 us
ConfidenceInterval = [55.772 us; 56.862 us] (CI 99.9%), Margin = 0.545 us (0.97% of Mean)
Skewness = -0.03, Kurtosis = 3.42, MValue = 2
-------------------- Histogram --------------------
[55.074 us ; 57.542 us) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJsonNode_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 64.183 us, StdErr = 0.073 us (0.11%), N = 15, StdDev = 0.281 us
Min = 63.759 us, Q1 = 63.998 us, Median = 64.189 us, Q3 = 64.401 us, Max = 64.729 us
IQR = 0.404 us, LowerFence = 63.392 us, UpperFence = 65.007 us
ConfidenceInterval = [63.882 us; 64.483 us] (CI 99.9%), Margin = 0.300 us (0.47% of Mean)
Skewness = 0.11, Kurtosis = 1.84, MValue = 2
-------------------- Histogram --------------------
[63.610 us ; 64.878 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.NewtonsoftJObject_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 112.767 us, StdErr = 0.103 us (0.09%), N = 13, StdDev = 0.372 us
Min = 111.878 us, Q1 = 112.605 us, Median = 112.792 us, Q3 = 112.951 us, Max = 113.423 us
IQR = 0.346 us, LowerFence = 112.086 us, UpperFence = 113.471 us
ConfidenceInterval = [112.321 us; 113.212 us] (CI 99.9%), Margin = 0.446 us (0.40% of Mean)
Skewness = -0.62, Kurtosis = 3.42, MValue = 2
-------------------- Histogram --------------------
[111.670 us ; 113.630 us) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.StjNode_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 7.762 ms, StdErr = 0.023 ms (0.30%), N = 15, StdDev = 0.090 ms
Min = 7.632 ms, Q1 = 7.711 ms, Median = 7.759 ms, Q3 = 7.817 ms, Max = 7.937 ms
IQR = 0.106 ms, LowerFence = 7.552 ms, UpperFence = 7.977 ms
ConfidenceInterval = [7.665 ms; 7.858 ms] (CI 99.9%), Margin = 0.097 ms (1.24% of Mean)
Skewness = 0.16, Kurtosis = 2, MValue = 2
-------------------- Histogram --------------------
[7.584 ms ; 7.985 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJsonNode_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 8.855 ms, StdErr = 0.025 ms (0.28%), N = 13, StdDev = 0.089 ms
Min = 8.708 ms, Q1 = 8.801 ms, Median = 8.850 ms, Q3 = 8.899 ms, Max = 9.032 ms
IQR = 0.098 ms, LowerFence = 8.655 ms, UpperFence = 9.045 ms
ConfidenceInterval = [8.748 ms; 8.962 ms] (CI 99.9%), Margin = 0.107 ms (1.21% of Mean)
Skewness = 0.5, Kurtosis = 2.42, MValue = 2
-------------------- Histogram --------------------
[8.658 ms ; 9.082 ms) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.NewtonsoftJObject_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 43.053 ms, StdErr = 0.223 ms (0.52%), N = 25, StdDev = 1.113 ms
Min = 41.252 ms, Q1 = 42.433 ms, Median = 42.571 ms, Q3 = 43.712 ms, Max = 45.475 ms
IQR = 1.279 ms, LowerFence = 40.515 ms, UpperFence = 45.631 ms
ConfidenceInterval = [42.219 ms; 43.887 ms] (CI 99.9%), Margin = 0.834 ms (1.94% of Mean)
Skewness = 0.32, Kurtosis = 2.17, MValue = 2
-------------------- Histogram --------------------
[40.881 ms ; 41.957 ms) | @@@
[41.957 ms ; 42.956 ms) | @@@@@@@@@@@
[42.956 ms ; 44.409 ms) | @@@@@@@@
[44.409 ms ; 45.497 ms) | @@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.128 us, StdErr = 0.001 us (0.08%), N = 14, StdDev = 0.003 us
Min = 1.119 us, Q1 = 1.127 us, Median = 1.128 us, Q3 = 1.130 us, Max = 1.134 us
IQR = 0.003 us, LowerFence = 1.122 us, UpperFence = 1.135 us
ConfidenceInterval = [1.124 us; 1.132 us] (CI 99.9%), Margin = 0.004 us (0.35% of Mean)
Skewness = -0.92, Kurtosis = 4, MValue = 2
-------------------- Histogram --------------------
[1.117 us ; 1.135 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.301 us, StdErr = 0.001 us (0.06%), N = 13, StdDev = 0.003 us
Min = 1.298 us, Q1 = 1.299 us, Median = 1.302 us, Q3 = 1.303 us, Max = 1.307 us
IQR = 0.004 us, LowerFence = 1.293 us, UpperFence = 1.310 us
ConfidenceInterval = [1.298 us; 1.305 us] (CI 99.9%), Margin = 0.003 us (0.26% of Mean)
Skewness = 0.17, Kurtosis = 1.75, MValue = 2
-------------------- Histogram --------------------
[1.296 us ; 1.308 us) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.358 us, StdErr = 0.001 us (0.10%), N = 14, StdDev = 0.005 us
Min = 1.349 us, Q1 = 1.354 us, Median = 1.358 us, Q3 = 1.361 us, Max = 1.367 us
IQR = 0.007 us, LowerFence = 1.343 us, UpperFence = 1.372 us
ConfidenceInterval = [1.352 us; 1.363 us] (CI 99.9%), Margin = 0.006 us (0.42% of Mean)
Skewness = 0.08, Kurtosis = 1.78, MValue = 2
-------------------- Histogram --------------------
[1.347 us ; 1.370 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 2.691 us, StdErr = 0.007 us (0.25%), N = 15, StdDev = 0.026 us
Min = 2.626 us, Q1 = 2.677 us, Median = 2.696 us, Q3 = 2.704 us, Max = 2.739 us
IQR = 0.027 us, LowerFence = 2.637 us, UpperFence = 2.744 us
ConfidenceInterval = [2.663 us; 2.719 us] (CI 99.9%), Margin = 0.028 us (1.04% of Mean)
Skewness = -0.6, Kurtosis = 3.48, MValue = 2
-------------------- Histogram --------------------
[2.612 us ; 2.665 us) | @
[2.665 us ; 2.753 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 39.633 us, StdErr = 0.014 us (0.04%), N = 14, StdDev = 0.052 us
Min = 39.533 us, Q1 = 39.606 us, Median = 39.633 us, Q3 = 39.647 us, Max = 39.737 us
IQR = 0.041 us, LowerFence = 39.544 us, UpperFence = 39.708 us
ConfidenceInterval = [39.574 us; 39.692 us] (CI 99.9%), Margin = 0.059 us (0.15% of Mean)
Skewness = 0.17, Kurtosis = 2.52, MValue = 2
-------------------- Histogram --------------------
[39.504 us ; 39.765 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 39.813 us, StdErr = 0.024 us (0.06%), N = 15, StdDev = 0.092 us
Min = 39.695 us, Q1 = 39.733 us, Median = 39.797 us, Q3 = 39.878 us, Max = 39.999 us
IQR = 0.146 us, LowerFence = 39.514 us, UpperFence = 40.097 us
ConfidenceInterval = [39.715 us; 39.911 us] (CI 99.9%), Margin = 0.098 us (0.25% of Mean)
Skewness = 0.35, Kurtosis = 1.92, MValue = 2
-------------------- Histogram --------------------
[39.646 us ; 40.048 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 41.912 us, StdErr = 0.026 us (0.06%), N = 15, StdDev = 0.101 us
Min = 41.746 us, Q1 = 41.859 us, Median = 41.894 us, Q3 = 41.970 us, Max = 42.111 us
IQR = 0.111 us, LowerFence = 41.692 us, UpperFence = 42.137 us
ConfidenceInterval = [41.804 us; 42.020 us] (CI 99.9%), Margin = 0.108 us (0.26% of Mean)
Skewness = 0.29, Kurtosis = 2.21, MValue = 2
-------------------- Histogram --------------------
[41.692 us ; 42.165 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 67.840 us, StdErr = 0.043 us (0.06%), N = 14, StdDev = 0.162 us
Min = 67.594 us, Q1 = 67.750 us, Median = 67.809 us, Q3 = 67.927 us, Max = 68.186 us
IQR = 0.177 us, LowerFence = 67.484 us, UpperFence = 68.193 us
ConfidenceInterval = [67.658 us; 68.023 us] (CI 99.9%), Margin = 0.182 us (0.27% of Mean)
Skewness = 0.5, Kurtosis = 2.35, MValue = 2
-------------------- Histogram --------------------
[67.506 us ; 68.274 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 3.962 ms, StdErr = 0.004 ms (0.11%), N = 15, StdDev = 0.016 ms
Min = 3.942 ms, Q1 = 3.950 ms, Median = 3.958 ms, Q3 = 3.977 ms, Max = 3.990 ms
IQR = 0.026 ms, LowerFence = 3.911 ms, UpperFence = 4.016 ms
ConfidenceInterval = [3.945 ms; 3.979 ms] (CI 99.9%), Margin = 0.017 ms (0.44% of Mean)
Skewness = 0.46, Kurtosis = 1.63, MValue = 2
-------------------- Histogram --------------------
[3.934 ms ; 3.999 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 3.984 ms, StdErr = 0.004 ms (0.09%), N = 13, StdDev = 0.013 ms
Min = 3.971 ms, Q1 = 3.977 ms, Median = 3.980 ms, Q3 = 3.985 ms, Max = 4.019 ms
IQR = 0.008 ms, LowerFence = 3.965 ms, UpperFence = 3.997 ms
ConfidenceInterval = [3.968 ms; 3.999 ms] (CI 99.9%), Margin = 0.015 ms (0.39% of Mean)
Skewness = 1.5, Kurtosis = 4.57, MValue = 2
-------------------- Histogram --------------------
[3.964 ms ; 4.026 ms) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 4.176 ms, StdErr = 0.002 ms (0.06%), N = 14, StdDev = 0.009 ms
Min = 4.163 ms, Q1 = 4.171 ms, Median = 4.175 ms, Q3 = 4.182 ms, Max = 4.190 ms
IQR = 0.011 ms, LowerFence = 4.154 ms, UpperFence = 4.199 ms
ConfidenceInterval = [4.166 ms; 4.186 ms] (CI 99.9%), Margin = 0.010 ms (0.24% of Mean)
Skewness = 0.14, Kurtosis = 1.62, MValue = 2
-------------------- Histogram --------------------
[4.158 ms ; 4.195 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_FromUtf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 6.807 ms, StdErr = 0.006 ms (0.09%), N = 14, StdDev = 0.022 ms
Min = 6.784 ms, Q1 = 6.790 ms, Median = 6.800 ms, Q3 = 6.815 ms, Max = 6.851 ms
IQR = 0.025 ms, LowerFence = 6.752 ms, UpperFence = 6.853 ms
ConfidenceInterval = [6.782 ms; 6.831 ms] (CI 99.9%), Margin = 0.025 ms (0.36% of Mean)
Skewness = 0.84, Kurtosis = 2.3, MValue = 2
-------------------- Histogram --------------------
[6.772 ms ; 6.863 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.209 us, StdErr = 0.001 us (0.04%), N = 14, StdDev = 0.002 us
Min = 1.206 us, Q1 = 1.208 us, Median = 1.209 us, Q3 = 1.210 us, Max = 1.213 us
IQR = 0.002 us, LowerFence = 1.205 us, UpperFence = 1.213 us
ConfidenceInterval = [1.207 us; 1.211 us] (CI 99.9%), Margin = 0.002 us (0.18% of Mean)
Skewness = 0.37, Kurtosis = 2.42, MValue = 2
-------------------- Histogram --------------------
[1.205 us ; 1.214 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.219 us, StdErr = 0.001 us (0.05%), N = 15, StdDev = 0.002 us
Min = 1.215 us, Q1 = 1.217 us, Median = 1.218 us, Q3 = 1.220 us, Max = 1.224 us
IQR = 0.003 us, LowerFence = 1.213 us, UpperFence = 1.225 us
ConfidenceInterval = [1.216 us; 1.221 us] (CI 99.9%), Margin = 0.003 us (0.21% of Mean)
Skewness = 0.4, Kurtosis = 2.14, MValue = 2
-------------------- Histogram --------------------
[1.214 us ; 1.225 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.219 us, StdErr = 0.002 us (0.12%), N = 13, StdDev = 0.005 us
Min = 1.213 us, Q1 = 1.215 us, Median = 1.218 us, Q3 = 1.223 us, Max = 1.232 us
IQR = 0.008 us, LowerFence = 1.204 us, UpperFence = 1.234 us
ConfidenceInterval = [1.213 us; 1.226 us] (CI 99.9%), Margin = 0.007 us (0.54% of Mean)
Skewness = 0.99, Kurtosis = 3.01, MValue = 2
-------------------- Histogram --------------------
[1.210 us ; 1.236 us) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 2.463 us, StdErr = 0.012 us (0.49%), N = 18, StdDev = 0.051 us
Min = 2.385 us, Q1 = 2.417 us, Median = 2.471 us, Q3 = 2.486 us, Max = 2.551 us
IQR = 0.069 us, LowerFence = 2.313 us, UpperFence = 2.590 us
ConfidenceInterval = [2.415 us; 2.511 us] (CI 99.9%), Margin = 0.048 us (1.95% of Mean)
Skewness = 0.17, Kurtosis = 1.89, MValue = 2
-------------------- Histogram --------------------
[2.381 us ; 2.432 us) | @@@@@@
[2.432 us ; 2.505 us) | @@@@@@@@@
[2.505 us ; 2.577 us) | @@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 37.494 us, StdErr = 0.012 us (0.03%), N = 15, StdDev = 0.045 us
Min = 37.430 us, Q1 = 37.454 us, Median = 37.502 us, Q3 = 37.523 us, Max = 37.578 us
IQR = 0.069 us, LowerFence = 37.350 us, UpperFence = 37.627 us
ConfidenceInterval = [37.446 us; 37.542 us] (CI 99.9%), Margin = 0.048 us (0.13% of Mean)
Skewness = 0.23, Kurtosis = 1.74, MValue = 2
-------------------- Histogram --------------------
[37.406 us ; 37.602 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 37.750 us, StdErr = 0.011 us (0.03%), N = 12, StdDev = 0.038 us
Min = 37.711 us, Q1 = 37.720 us, Median = 37.743 us, Q3 = 37.769 us, Max = 37.830 us
IQR = 0.049 us, LowerFence = 37.647 us, UpperFence = 37.842 us
ConfidenceInterval = [37.701 us; 37.798 us] (CI 99.9%), Margin = 0.049 us (0.13% of Mean)
Skewness = 0.72, Kurtosis = 2.22, MValue = 2
-------------------- Histogram --------------------
[37.689 us ; 37.852 us) | @@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 41.350 us, StdErr = 0.022 us (0.05%), N = 14, StdDev = 0.083 us
Min = 41.220 us, Q1 = 41.286 us, Median = 41.342 us, Q3 = 41.393 us, Max = 41.504 us
IQR = 0.106 us, LowerFence = 41.127 us, UpperFence = 41.552 us
ConfidenceInterval = [41.256 us; 41.444 us] (CI 99.9%), Margin = 0.094 us (0.23% of Mean)
Skewness = 0.14, Kurtosis = 1.93, MValue = 2
-------------------- Histogram --------------------
[41.174 us ; 41.550 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 65.518 us, StdErr = 0.078 us (0.12%), N = 14, StdDev = 0.293 us
Min = 65.240 us, Q1 = 65.329 us, Median = 65.373 us, Q3 = 65.758 us, Max = 66.063 us
IQR = 0.429 us, LowerFence = 64.685 us, UpperFence = 66.402 us
ConfidenceInterval = [65.188 us; 65.848 us] (CI 99.9%), Margin = 0.330 us (0.50% of Mean)
Skewness = 0.78, Kurtosis = 1.81, MValue = 2
-------------------- Histogram --------------------
[65.080 us ; 66.222 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 3.752 ms, StdErr = 0.001 ms (0.04%), N = 14, StdDev = 0.005 ms
Min = 3.744 ms, Q1 = 3.748 ms, Median = 3.752 ms, Q3 = 3.754 ms, Max = 3.760 ms
IQR = 0.006 ms, LowerFence = 3.739 ms, UpperFence = 3.764 ms
ConfidenceInterval = [3.746 ms; 3.757 ms] (CI 99.9%), Margin = 0.006 ms (0.15% of Mean)
Skewness = 0.08, Kurtosis = 1.92, MValue = 2
-------------------- Histogram --------------------
[3.741 ms ; 3.763 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 3.756 ms, StdErr = 0.001 ms (0.03%), N = 13, StdDev = 0.005 ms
Min = 3.748 ms, Q1 = 3.753 ms, Median = 3.758 ms, Q3 = 3.761 ms, Max = 3.762 ms
IQR = 0.007 ms, LowerFence = 3.742 ms, UpperFence = 3.772 ms
ConfidenceInterval = [3.751 ms; 3.762 ms] (CI 99.9%), Margin = 0.006 ms (0.15% of Mean)
Skewness = -0.37, Kurtosis = 1.74, MValue = 2
-------------------- Histogram --------------------
[3.745 ms ; 3.765 ms) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 4.179 ms, StdErr = 0.004 ms (0.10%), N = 15, StdDev = 0.016 ms
Min = 4.153 ms, Q1 = 4.168 ms, Median = 4.184 ms, Q3 = 4.191 ms, Max = 4.206 ms
IQR = 0.023 ms, LowerFence = 4.133 ms, UpperFence = 4.226 ms
ConfidenceInterval = [4.162 ms; 4.196 ms] (CI 99.9%), Margin = 0.017 ms (0.41% of Mean)
Skewness = -0.1, Kurtosis = 1.6, MValue = 2
-------------------- Histogram --------------------
[4.145 ms ; 4.214 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_FromString: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 6.485 ms, StdErr = 0.003 ms (0.05%), N = 14, StdDev = 0.011 ms
Min = 6.466 ms, Q1 = 6.478 ms, Median = 6.482 ms, Q3 = 6.495 ms, Max = 6.502 ms
IQR = 0.017 ms, LowerFence = 6.452 ms, UpperFence = 6.520 ms
ConfidenceInterval = [6.472 ms; 6.497 ms] (CI 99.9%), Margin = 0.013 ms (0.19% of Mean)
Skewness = 0.16, Kurtosis = 1.72, MValue = 2
-------------------- Histogram --------------------
[6.460 ms ; 6.508 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.StjDocument_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 756.945 ns, StdErr = 1.181 ns (0.16%), N = 14, StdDev = 4.418 ns
Min = 752.130 ns, Q1 = 753.544 ns, Median = 755.797 ns, Q3 = 759.292 ns, Max = 767.390 ns
IQR = 5.748 ns, LowerFence = 744.922 ns, UpperFence = 767.914 ns
ConfidenceInterval = [751.961 ns; 761.928 ns] (CI 99.9%), Margin = 4.983 ns (0.66% of Mean)
Skewness = 0.93, Kurtosis = 2.77, MValue = 2
-------------------- Histogram --------------------
[749.724 ns ; 769.796 ns) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJsonTape_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 794.109 ns, StdErr = 0.299 ns (0.04%), N = 13, StdDev = 1.078 ns
Min = 792.555 ns, Q1 = 793.514 ns, Median = 794.099 ns, Q3 = 794.385 ns, Max = 796.291 ns
IQR = 0.871 ns, LowerFence = 792.207 ns, UpperFence = 795.693 ns
ConfidenceInterval = [792.818 ns; 795.400 ns] (CI 99.9%), Margin = 1.291 ns (0.16% of Mean)
Skewness = 0.47, Kurtosis = 2.16, MValue = 2
-------------------- Histogram --------------------
[791.953 ns ; 796.893 ns) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.StjDocument_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 28.456 us, StdErr = 0.011 us (0.04%), N = 15, StdDev = 0.044 us
Min = 28.392 us, Q1 = 28.427 us, Median = 28.435 us, Q3 = 28.485 us, Max = 28.557 us
IQR = 0.059 us, LowerFence = 28.339 us, UpperFence = 28.573 us
ConfidenceInterval = [28.409 us; 28.503 us] (CI 99.9%), Margin = 0.047 us (0.17% of Mean)
Skewness = 0.7, Kurtosis = 2.5, MValue = 2
-------------------- Histogram --------------------
[28.369 us ; 28.580 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJsonTape_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 29.068 us, StdErr = 0.005 us (0.02%), N = 15, StdDev = 0.020 us
Min = 29.036 us, Q1 = 29.056 us, Median = 29.066 us, Q3 = 29.083 us, Max = 29.100 us
IQR = 0.027 us, LowerFence = 29.016 us, UpperFence = 29.123 us
ConfidenceInterval = [29.047 us; 29.089 us] (CI 99.9%), Margin = 0.021 us (0.07% of Mean)
Skewness = 0.03, Kurtosis = 1.84, MValue = 2
-------------------- Histogram --------------------
[29.026 us ; 29.110 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.StjDocument_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 3.595 ms, StdErr = 0.018 ms (0.50%), N = 20, StdDev = 0.080 ms
Min = 3.353 ms, Q1 = 3.558 ms, Median = 3.609 ms, Q3 = 3.645 ms, Max = 3.724 ms
IQR = 0.087 ms, LowerFence = 3.428 ms, UpperFence = 3.775 ms
ConfidenceInterval = [3.526 ms; 3.664 ms] (CI 99.9%), Margin = 0.069 ms (1.92% of Mean)
Skewness = -1.08, Kurtosis = 4.92, MValue = 2
-------------------- Histogram --------------------
[3.314 ms ; 3.391 ms) | @
[3.391 ms ; 3.468 ms) |
[3.468 ms ; 3.557 ms) | @@@
[3.557 ms ; 3.634 ms) | @@@@@@@@@@
[3.634 ms ; 3.763 ms) | @@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJsonTape_SumPrices: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 3.723 ms, StdErr = 0.020 ms (0.52%), N = 23, StdDev = 0.094 ms
Min = 3.568 ms, Q1 = 3.655 ms, Median = 3.717 ms, Q3 = 3.804 ms, Max = 3.876 ms
IQR = 0.149 ms, LowerFence = 3.431 ms, UpperFence = 4.028 ms
ConfidenceInterval = [3.649 ms; 3.796 ms] (CI 99.9%), Margin = 0.074 ms (1.99% of Mean)
Skewness = -0.11, Kurtosis = 1.74, MValue = 2
-------------------- Histogram --------------------
[3.540 ms ; 3.634 ms) | @@@@
[3.634 ms ; 3.720 ms) | @@@@@@@@
[3.720 ms ; 3.832 ms) | @@@@@@@@@
[3.832 ms ; 3.919 ms) | @@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 564.588 ns, StdErr = 2.624 ns (0.46%), N = 15, StdDev = 10.161 ns
Min = 545.620 ns, Q1 = 558.169 ns, Median = 563.716 ns, Q3 = 572.350 ns, Max = 577.862 ns
IQR = 14.181 ns, LowerFence = 536.898 ns, UpperFence = 593.621 ns
ConfidenceInterval = [553.725 ns; 575.451 ns] (CI 99.9%), Margin = 10.863 ns (1.92% of Mean)
Skewness = -0.33, Kurtosis = 1.87, MValue = 2
-------------------- Histogram --------------------
[545.291 ns ; 559.931 ns) | @@@@
[559.931 ns ; 583.270 ns) | @@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 618.992 ns, StdErr = 1.631 ns (0.26%), N = 15, StdDev = 6.315 ns
Min = 608.887 ns, Q1 = 615.413 ns, Median = 618.439 ns, Q3 = 622.059 ns, Max = 630.032 ns
IQR = 6.646 ns, LowerFence = 605.444 ns, UpperFence = 632.028 ns
ConfidenceInterval = [612.241 ns; 625.743 ns] (CI 99.9%), Margin = 6.751 ns (1.09% of Mean)
Skewness = 0.17, Kurtosis = 2.08, MValue = 2
-------------------- Histogram --------------------
[605.526 ns ; 633.393 ns) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 656.752 ns, StdErr = 3.576 ns (0.54%), N = 33, StdDev = 20.540 ns
Min = 600.933 ns, Q1 = 646.319 ns, Median = 662.535 ns, Q3 = 671.816 ns, Max = 684.577 ns
IQR = 25.497 ns, LowerFence = 608.074 ns, UpperFence = 710.061 ns
ConfidenceInterval = [643.802 ns; 669.702 ns] (CI 99.9%), Margin = 12.950 ns (1.97% of Mean)
Skewness = -0.81, Kurtosis = 2.92, MValue = 2
-------------------- Histogram --------------------
[600.541 ns ; 622.799 ns) | @@
[622.799 ns ; 639.609 ns) | @@@@@
[639.609 ns ; 660.572 ns) | @@@@@@@@@
[660.572 ns ; 692.982 ns) | @@@@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 2.787 us, StdErr = 0.014 us (0.49%), N = 20, StdDev = 0.061 us
Min = 2.617 us, Q1 = 2.760 us, Median = 2.796 us, Q3 = 2.822 us, Max = 2.869 us
IQR = 0.061 us, LowerFence = 2.669 us, UpperFence = 2.913 us
ConfidenceInterval = [2.734 us; 2.840 us] (CI 99.9%), Margin = 0.053 us (1.90% of Mean)
Skewness = -0.95, Kurtosis = 3.77, MValue = 2
-------------------- Histogram --------------------
[2.587 us ; 2.646 us) | @
[2.646 us ; 2.704 us) |
[2.704 us ; 2.771 us) | @@@@@
[2.771 us ; 2.830 us) | @@@@@@@@@@
[2.830 us ; 2.898 us) | @@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 20.347 us, StdErr = 0.106 us (0.52%), N = 22, StdDev = 0.497 us
Min = 19.155 us, Q1 = 20.170 us, Median = 20.393 us, Q3 = 20.635 us, Max = 21.190 us
IQR = 0.465 us, LowerFence = 19.472 us, UpperFence = 21.333 us
ConfidenceInterval = [19.942 us; 20.752 us] (CI 99.9%), Margin = 0.405 us (1.99% of Mean)
Skewness = -0.92, Kurtosis = 3.48, MValue = 2
-------------------- Histogram --------------------
[18.922 us ; 19.411 us) | @@
[19.411 us ; 20.218 us) | @@@@
[20.218 us ; 20.684 us) | @@@@@@@@@@@@
[20.684 us ; 21.206 us) | @@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 21.034 us, StdErr = 0.078 us (0.37%), N = 15, StdDev = 0.303 us
Min = 20.443 us, Q1 = 20.849 us, Median = 20.941 us, Q3 = 21.244 us, Max = 21.551 us
IQR = 0.394 us, LowerFence = 20.258 us, UpperFence = 21.836 us
ConfidenceInterval = [20.710 us; 21.358 us] (CI 99.9%), Margin = 0.324 us (1.54% of Mean)
Skewness = 0.07, Kurtosis = 2.14, MValue = 2
-------------------- Histogram --------------------
[20.282 us ; 21.085 us) | @@@@@@@@@
[21.085 us ; 21.607 us) | @@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 22.846 us, StdErr = 0.065 us (0.28%), N = 14, StdDev = 0.243 us
Min = 22.157 us, Q1 = 22.829 us, Median = 22.897 us, Q3 = 22.986 us, Max = 23.109 us
IQR = 0.157 us, LowerFence = 22.593 us, UpperFence = 23.222 us
ConfidenceInterval = [22.572 us; 23.120 us] (CI 99.9%), Margin = 0.274 us (1.20% of Mean)
Skewness = -1.51, Kurtosis = 4.94, MValue = 2
-------------------- Histogram --------------------
[22.024 us ; 23.143 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 68.092 us, StdErr = 0.153 us (0.22%), N = 13, StdDev = 0.550 us
Min = 67.479 us, Q1 = 67.669 us, Median = 68.124 us, Q3 = 68.321 us, Max = 69.440 us
IQR = 0.652 us, LowerFence = 66.691 us, UpperFence = 69.299 us
ConfidenceInterval = [67.433 us; 68.751 us] (CI 99.9%), Margin = 0.659 us (0.97% of Mean)
Skewness = 0.87, Kurtosis = 3.21, MValue = 2
-------------------- Histogram --------------------
[67.172 us ; 69.747 us) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 2.020 ms, StdErr = 0.010 ms (0.50%), N = 17, StdDev = 0.041 ms
Min = 1.920 ms, Q1 = 2.006 ms, Median = 2.033 ms, Q3 = 2.046 ms, Max = 2.086 ms
IQR = 0.039 ms, LowerFence = 1.947 ms, UpperFence = 2.105 ms
ConfidenceInterval = [1.979 ms; 2.060 ms] (CI 99.9%), Margin = 0.040 ms (1.99% of Mean)
Skewness = -0.77, Kurtosis = 2.99, MValue = 2
-------------------- Histogram --------------------
[1.899 ms ; 1.952 ms) | @
[1.952 ms ; 1.994 ms) | @@@
[1.994 ms ; 2.048 ms) | @@@@@@@@@@
[2.048 ms ; 2.091 ms) | @@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 2.020 ms, StdErr = 0.009 ms (0.47%), N = 17, StdDev = 0.039 ms
Min = 1.933 ms, Q1 = 1.990 ms, Median = 2.018 ms, Q3 = 2.048 ms, Max = 2.079 ms
IQR = 0.058 ms, LowerFence = 1.903 ms, UpperFence = 2.134 ms
ConfidenceInterval = [1.982 ms; 2.058 ms] (CI 99.9%), Margin = 0.038 ms (1.88% of Mean)
Skewness = -0.27, Kurtosis = 2.39, MValue = 2
-------------------- Histogram --------------------
[1.913 ms ; 1.982 ms) | @@
[1.982 ms ; 2.099 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 3.170 ms, StdErr = 0.010 ms (0.31%), N = 15, StdDev = 0.038 ms
Min = 3.091 ms, Q1 = 3.150 ms, Median = 3.166 ms, Q3 = 3.200 ms, Max = 3.241 ms
IQR = 0.050 ms, LowerFence = 3.075 ms, UpperFence = 3.275 ms
ConfidenceInterval = [3.129 ms; 3.211 ms] (CI 99.9%), Margin = 0.041 ms (1.29% of Mean)
Skewness = -0.07, Kurtosis = 2.43, MValue = 2
-------------------- Histogram --------------------
[3.070 ms ; 3.181 ms) | @@@@@@@@@@
[3.181 ms ; 3.262 ms) | @@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_Stream: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 6.396 ms, StdErr = 0.024 ms (0.37%), N = 15, StdDev = 0.092 ms
Min = 6.197 ms, Q1 = 6.368 ms, Median = 6.408 ms, Q3 = 6.465 ms, Max = 6.501 ms
IQR = 0.097 ms, LowerFence = 6.222 ms, UpperFence = 6.611 ms
ConfidenceInterval = [6.298 ms; 6.495 ms] (CI 99.9%), Margin = 0.099 ms (1.54% of Mean)
Skewness = -0.89, Kurtosis = 2.68, MValue = 2
-------------------- Histogram --------------------
[6.148 ms ; 6.296 ms) | @@
[6.296 ms ; 6.519 ms) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 567.586 ns, StdErr = 2.668 ns (0.47%), N = 17, StdDev = 11.001 ns
Min = 544.640 ns, Q1 = 558.006 ns, Median = 572.260 ns, Q3 = 574.259 ns, Max = 586.310 ns
IQR = 16.253 ns, LowerFence = 533.627 ns, UpperFence = 598.638 ns
ConfidenceInterval = [556.873 ns; 578.299 ns] (CI 99.9%), Margin = 10.713 ns (1.89% of Mean)
Skewness = -0.31, Kurtosis = 2.04, MValue = 2
-------------------- Histogram --------------------
[539.025 ns ; 555.282 ns) | @
[555.282 ns ; 569.999 ns) | @@@@@@@
[569.999 ns ; 591.925 ns) | @@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Utf8_IntoBuffer: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 574.328 ns, StdErr = 2.304 ns (0.40%), N = 13, StdDev = 8.306 ns
Min = 552.873 ns, Q1 = 571.913 ns, Median = 573.818 ns, Q3 = 578.879 ns, Max = 587.107 ns
IQR = 6.966 ns, LowerFence = 561.463 ns, UpperFence = 589.329 ns
ConfidenceInterval = [564.380 ns; 584.275 ns] (CI 99.9%), Margin = 9.947 ns (1.73% of Mean)
Skewness = -0.89, Kurtosis = 4.05, MValue = 2
-------------------- Histogram --------------------
[548.237 ns ; 563.597 ns) | @
[563.597 ns ; 591.744 ns) | @@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 579.081 ns, StdErr = 1.032 ns (0.18%), N = 14, StdDev = 3.863 ns
Min = 574.850 ns, Q1 = 575.613 ns, Median = 578.835 ns, Q3 = 580.324 ns, Max = 587.505 ns
IQR = 4.711 ns, LowerFence = 568.546 ns, UpperFence = 587.390 ns
ConfidenceInterval = [574.723 ns; 583.438 ns] (CI 99.9%), Margin = 4.357 ns (0.75% of Mean)
Skewness = 0.73, Kurtosis = 2.45, MValue = 2
-------------------- Histogram --------------------
[572.747 ns ; 589.609 ns) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 625.816 ns, StdErr = 1.351 ns (0.22%), N = 13, StdDev = 4.871 ns
Min = 618.774 ns, Q1 = 621.201 ns, Median = 627.060 ns, Q3 = 628.022 ns, Max = 633.675 ns
IQR = 6.821 ns, LowerFence = 610.969 ns, UpperFence = 638.253 ns
ConfidenceInterval = [619.982 ns; 631.650 ns] (CI 99.9%), Margin = 5.834 ns (0.93% of Mean)
Skewness = 0.03, Kurtosis = 1.59, MValue = 2
-------------------- Histogram --------------------
[616.055 ns ; 636.394 ns) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 2.923 us, StdErr = 0.015 us (0.51%), N = 25, StdDev = 0.075 us
Min = 2.757 us, Q1 = 2.876 us, Median = 2.926 us, Q3 = 2.954 us, Max = 3.082 us
IQR = 0.078 us, LowerFence = 2.759 us, UpperFence = 3.072 us
ConfidenceInterval = [2.867 us; 2.979 us] (CI 99.9%), Margin = 0.056 us (1.91% of Mean)
Skewness = 0.05, Kurtosis = 2.98, MValue = 2
-------------------- Histogram --------------------
[2.724 us ; 2.791 us) | @
[2.791 us ; 2.877 us) | @@@@@@
[2.877 us ; 2.972 us) | @@@@@@@@@@@@@
[2.972 us ; 3.044 us) | @@@
[3.044 us ; 3.115 us) | @@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Utf8_IntoBuffer: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 19.947 us, StdErr = 0.093 us (0.47%), N = 16, StdDev = 0.373 us
Min = 19.324 us, Q1 = 19.687 us, Median = 20.042 us, Q3 = 20.213 us, Max = 20.577 us
IQR = 0.526 us, LowerFence = 18.899 us, UpperFence = 21.002 us
ConfidenceInterval = [19.567 us; 20.327 us] (CI 99.9%), Margin = 0.380 us (1.91% of Mean)
Skewness = -0.26, Kurtosis = 1.79, MValue = 2
-------------------- Histogram --------------------
[19.253 us ; 19.971 us) | @@@@@@@
[19.971 us ; 20.771 us) | @@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 21.660 us, StdErr = 0.066 us (0.30%), N = 15, StdDev = 0.254 us
Min = 21.094 us, Q1 = 21.585 us, Median = 21.703 us, Q3 = 21.818 us, Max = 22.006 us
IQR = 0.233 us, LowerFence = 21.235 us, UpperFence = 22.167 us
ConfidenceInterval = [21.389 us; 21.932 us] (CI 99.9%), Margin = 0.272 us (1.25% of Mean)
Skewness = -0.73, Kurtosis = 2.54, MValue = 2
-------------------- Histogram --------------------
[20.997 us ; 21.456 us) | @@@
[21.456 us ; 22.141 us) | @@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 21.829 us, StdErr = 0.065 us (0.30%), N = 15, StdDev = 0.253 us
Min = 21.416 us, Q1 = 21.658 us, Median = 21.855 us, Q3 = 21.945 us, Max = 22.334 us
IQR = 0.287 us, LowerFence = 21.228 us, UpperFence = 22.376 us
ConfidenceInterval = [21.559 us; 22.099 us] (CI 99.9%), Margin = 0.270 us (1.24% of Mean)
Skewness = 0.19, Kurtosis = 2.21, MValue = 2
-------------------- Histogram --------------------
[21.281 us ; 22.468 us) | @@@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 23.310 us, StdErr = 0.121 us (0.52%), N = 21, StdDev = 0.554 us
Min = 21.598 us, Q1 = 23.064 us, Median = 23.362 us, Q3 = 23.676 us, Max = 24.127 us
IQR = 0.612 us, LowerFence = 22.146 us, UpperFence = 24.595 us
ConfidenceInterval = [22.844 us; 23.775 us] (CI 99.9%), Margin = 0.465 us (2.00% of Mean)
Skewness = -1.22, Kurtosis = 4.94, MValue = 2
-------------------- Histogram --------------------
[21.334 us ; 21.861 us) | @
[21.861 us ; 22.717 us) | @
[22.717 us ; 23.233 us) | @@@@@@
[23.233 us ; 23.760 us) | @@@@@@@@@@
[23.760 us ; 24.390 us) | @@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 70.945 us, StdErr = 0.343 us (0.48%), N = 19, StdDev = 1.494 us
Min = 68.406 us, Q1 = 70.107 us, Median = 70.877 us, Q3 = 71.874 us, Max = 73.290 us
IQR = 1.767 us, LowerFence = 67.455 us, UpperFence = 74.525 us
ConfidenceInterval = [69.601 us; 72.289 us] (CI 99.9%), Margin = 1.344 us (1.89% of Mean)
Skewness = -0.12, Kurtosis = 1.85, MValue = 2
-------------------- Histogram --------------------
[68.009 us ; 69.479 us) | @@@@
[69.479 us ; 71.739 us) | @@@@@@@@@@
[71.739 us ; 73.386 us) | @@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Utf8_IntoBuffer: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 1.965 ms, StdErr = 0.007 ms (0.34%), N = 15, StdDev = 0.026 ms
Min = 1.918 ms, Q1 = 1.950 ms, Median = 1.956 ms, Q3 = 1.993 ms, Max = 2.003 ms
IQR = 0.043 ms, LowerFence = 1.886 ms, UpperFence = 2.058 ms
ConfidenceInterval = [1.937 ms; 1.992 ms] (CI 99.9%), Margin = 0.027 ms (1.39% of Mean)
Skewness = 0.15, Kurtosis = 1.73, MValue = 2
-------------------- Histogram --------------------
[1.904 ms ; 1.971 ms) | @@@@@@@@@@
[1.971 ms ; 2.017 ms) | @@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 2.127 ms, StdErr = 0.008 ms (0.38%), N = 14, StdDev = 0.030 ms
Min = 2.074 ms, Q1 = 2.110 ms, Median = 2.125 ms, Q3 = 2.150 ms, Max = 2.177 ms
IQR = 0.041 ms, LowerFence = 2.049 ms, UpperFence = 2.211 ms
ConfidenceInterval = [2.093 ms; 2.161 ms] (CI 99.9%), Margin = 0.034 ms (1.59% of Mean)
Skewness = -0.15, Kurtosis = 1.83, MValue = 2
-------------------- Histogram --------------------
[2.063 ms ; 2.110 ms) | @@@@
[2.110 ms ; 2.193 ms) | @@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 2.139 ms, StdErr = 0.009 ms (0.42%), N = 15, StdDev = 0.035 ms
Min = 2.076 ms, Q1 = 2.128 ms, Median = 2.141 ms, Q3 = 2.166 ms, Max = 2.196 ms
IQR = 0.038 ms, LowerFence = 2.071 ms, UpperFence = 2.223 ms
ConfidenceInterval = [2.102 ms; 2.176 ms] (CI 99.9%), Margin = 0.037 ms (1.74% of Mean)
Skewness = -0.38, Kurtosis = 2.13, MValue = 2
-------------------- Histogram --------------------
[2.065 ms ; 2.130 ms) | @@@@
[2.130 ms ; 2.215 ms) | @@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 2.377 ms, StdErr = 0.011 ms (0.45%), N = 15, StdDev = 0.041 ms
Min = 2.290 ms, Q1 = 2.347 ms, Median = 2.388 ms, Q3 = 2.406 ms, Max = 2.437 ms
IQR = 0.059 ms, LowerFence = 2.258 ms, UpperFence = 2.495 ms
ConfidenceInterval = [2.333 ms; 2.421 ms] (CI 99.9%), Margin = 0.044 ms (1.86% of Mean)
Skewness = -0.5, Kurtosis = 2.13, MValue = 2
-------------------- Histogram --------------------
[2.268 ms ; 2.358 ms) | @@@@@
[2.358 ms ; 2.459 ms) | @@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_Utf8: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 6.685 ms, StdErr = 0.028 ms (0.41%), N = 15, StdDev = 0.107 ms
Min = 6.495 ms, Q1 = 6.605 ms, Median = 6.705 ms, Q3 = 6.744 ms, Max = 6.868 ms
IQR = 0.139 ms, LowerFence = 6.397 ms, UpperFence = 6.953 ms
ConfidenceInterval = [6.570 ms; 6.800 ms] (CI 99.9%), Margin = 0.115 ms (1.72% of Mean)
Skewness = 0.04, Kurtosis = 2.06, MValue = 2
-------------------- Histogram --------------------
[6.438 ms ; 6.649 ms) | @@@@@@
[6.649 ms ; 6.925 ms) | @@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Chars_IntoBuffer: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 507.260 ns, StdErr = 2.421 ns (0.48%), N = 17, StdDev = 9.983 ns
Min = 487.925 ns, Q1 = 499.982 ns, Median = 508.279 ns, Q3 = 514.500 ns, Max = 520.982 ns
IQR = 14.517 ns, LowerFence = 478.206 ns, UpperFence = 536.276 ns
ConfidenceInterval = [497.539 ns; 516.981 ns] (CI 99.9%), Margin = 9.721 ns (1.92% of Mean)
Skewness = -0.53, Kurtosis = 1.9, MValue = 2
-------------------- Histogram --------------------
[482.829 ns ; 500.845 ns) | @@@@@
[500.845 ns ; 526.077 ns) | @@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 523.952 ns, StdErr = 2.722 ns (0.52%), N = 23, StdDev = 13.056 ns
Min = 497.982 ns, Q1 = 515.890 ns, Median = 521.513 ns, Q3 = 533.184 ns, Max = 544.261 ns
IQR = 17.294 ns, LowerFence = 489.948 ns, UpperFence = 559.125 ns
ConfidenceInterval = [513.629 ns; 534.276 ns] (CI 99.9%), Margin = 10.323 ns (1.97% of Mean)
Skewness = -0.19, Kurtosis = 2.05, MValue = 2
-------------------- Histogram --------------------
[496.036 ns ; 510.119 ns) | @@@
[510.119 ns ; 522.170 ns) | @@@@@@@@@
[522.170 ns ; 536.540 ns) | @@@@@@@
[536.540 ns ; 550.287 ns) | @@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 531.570 ns, StdErr = 2.345 ns (0.44%), N = 15, StdDev = 9.083 ns
Min = 515.444 ns, Q1 = 526.090 ns, Median = 529.710 ns, Q3 = 539.761 ns, Max = 545.438 ns
IQR = 13.671 ns, LowerFence = 505.584 ns, UpperFence = 560.268 ns
ConfidenceInterval = [521.860 ns; 541.280 ns] (CI 99.9%), Margin = 9.710 ns (1.83% of Mean)
Skewness = -0.07, Kurtosis = 1.65, MValue = 2
-------------------- Histogram --------------------
[510.610 ns ; 521.868 ns) | @@
[521.868 ns ; 546.948 ns) | @@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 686.321 ns, StdErr = 2.885 ns (0.42%), N = 15, StdDev = 11.173 ns
Min = 663.123 ns, Q1 = 683.417 ns, Median = 687.891 ns, Q3 = 694.655 ns, Max = 701.075 ns
IQR = 11.238 ns, LowerFence = 666.561 ns, UpperFence = 711.511 ns
ConfidenceInterval = [674.376 ns; 698.265 ns] (CI 99.9%), Margin = 11.945 ns (1.74% of Mean)
Skewness = -0.65, Kurtosis = 2.3, MValue = 2
-------------------- Histogram --------------------
[660.815 ns ; 680.688 ns) | @@@
[680.688 ns ; 707.021 ns) | @@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=1]
Runtime = ; GC =
Mean = 1.927 us, StdErr = 0.010 us (0.53%), N = 29, StdDev = 0.055 us
Min = 1.830 us, Q1 = 1.893 us, Median = 1.924 us, Q3 = 1.975 us, Max = 2.025 us
IQR = 0.082 us, LowerFence = 1.769 us, UpperFence = 2.098 us
ConfidenceInterval = [1.889 us; 1.964 us] (CI 99.9%), Margin = 0.038 us (1.96% of Mean)
Skewness = -0.03, Kurtosis = 1.94, MValue = 2
-------------------- Histogram --------------------
[1.825 us ; 1.873 us) | @@@@@
[1.873 us ; 1.939 us) | @@@@@@@@@@@@@
[1.939 us ; 1.999 us) | @@@@@@@@@
[1.999 us ; 2.049 us) | @@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Chars_IntoBuffer: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 15.501 us, StdErr = 0.082 us (0.53%), N = 26, StdDev = 0.418 us
Min = 14.678 us, Q1 = 15.221 us, Median = 15.557 us, Q3 = 15.841 us, Max = 16.235 us
IQR = 0.620 us, LowerFence = 14.290 us, UpperFence = 16.772 us
ConfidenceInterval = [15.196 us; 15.806 us] (CI 99.9%), Margin = 0.305 us (1.97% of Mean)
Skewness = -0.4, Kurtosis = 2.18, MValue = 2
-------------------- Histogram --------------------
[14.648 us ; 15.119 us) | @@@@@
[15.119 us ; 15.519 us) | @@@@@@
[15.519 us ; 15.889 us) | @@@@@@@@@@@
[15.889 us ; 16.269 us) | @@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 17.513 us, StdErr = 0.082 us (0.47%), N = 16, StdDev = 0.329 us
Min = 16.848 us, Q1 = 17.363 us, Median = 17.519 us, Q3 = 17.754 us, Max = 18.024 us
IQR = 0.391 us, LowerFence = 16.776 us, UpperFence = 18.340 us
ConfidenceInterval = [17.178 us; 17.848 us] (CI 99.9%), Margin = 0.335 us (1.91% of Mean)
Skewness = -0.31, Kurtosis = 2.36, MValue = 2
-------------------- Histogram --------------------
[16.726 us ; 17.283 us) | @@
[17.283 us ; 18.056 us) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 17.702 us, StdErr = 0.096 us (0.54%), N = 30, StdDev = 0.527 us
Min = 16.186 us, Q1 = 17.331 us, Median = 17.786 us, Q3 = 18.018 us, Max = 18.594 us
IQR = 0.687 us, LowerFence = 16.301 us, UpperFence = 19.048 us
ConfidenceInterval = [17.350 us; 18.054 us] (CI 99.9%), Margin = 0.352 us (1.99% of Mean)
Skewness = -0.62, Kurtosis = 3.32, MValue = 2
-------------------- Histogram --------------------
[15.963 us ; 16.408 us) | @
[16.408 us ; 17.102 us) | @@
[17.102 us ; 17.591 us) | @@@@@@@@
[17.591 us ; 18.036 us) | @@@@@@@@@@@@
[18.036 us ; 18.610 us) | @@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 24.461 us, StdErr = 0.123 us (0.50%), N = 19, StdDev = 0.537 us
Min = 23.530 us, Q1 = 24.035 us, Median = 24.594 us, Q3 = 24.855 us, Max = 25.416 us
IQR = 0.820 us, LowerFence = 22.805 us, UpperFence = 26.084 us
ConfidenceInterval = [23.977 us; 24.944 us] (CI 99.9%), Margin = 0.484 us (1.98% of Mean)
Skewness = -0.01, Kurtosis = 1.81, MValue = 2
-------------------- Histogram --------------------
[23.266 us ; 23.756 us) | @@
[23.756 us ; 24.285 us) | @@@@@@
[24.285 us ; 25.087 us) | @@@@@@@@@
[25.087 us ; 25.680 us) | @@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=75]
Runtime = ; GC =
Mean = 55.321 us, StdErr = 0.277 us (0.50%), N = 19, StdDev = 1.209 us
Min = 53.107 us, Q1 = 54.517 us, Median = 55.386 us, Q3 = 56.035 us, Max = 57.530 us
IQR = 1.518 us, LowerFence = 52.240 us, UpperFence = 58.312 us
ConfidenceInterval = [54.233 us; 56.409 us] (CI 99.9%), Margin = 1.088 us (1.97% of Mean)
Skewness = 0.15, Kurtosis = 2.06, MValue = 2
-------------------- Histogram --------------------
[53.028 us ; 54.316 us) | @@@@
[54.316 us ; 55.506 us) | @@@@@@@@@
[55.506 us ; 57.736 us) | @@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_Chars_IntoBuffer: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 1.507 ms, StdErr = 0.007 ms (0.48%), N = 18, StdDev = 0.031 ms
Min = 1.425 ms, Q1 = 1.508 ms, Median = 1.515 ms, Q3 = 1.523 ms, Max = 1.537 ms
IQR = 0.015 ms, LowerFence = 1.486 ms, UpperFence = 1.545 ms
ConfidenceInterval = [1.478 ms; 1.536 ms] (CI 99.9%), Margin = 0.029 ms (1.91% of Mean)
Skewness = -1.53, Kurtosis = 4.35, MValue = 2
-------------------- Histogram --------------------
[1.416 ms ; 1.471 ms) | @@
[1.471 ms ; 1.506 ms) | @@
[1.506 ms ; 1.537 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

JakarJson_Benchmarks.JakarJson_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 1.826 ms, StdErr = 0.009 ms (0.48%), N = 16, StdDev = 0.035 ms
Min = 1.768 ms, Q1 = 1.811 ms, Median = 1.833 ms, Q3 = 1.847 ms, Max = 1.880 ms
IQR = 0.035 ms, LowerFence = 1.758 ms, UpperFence = 1.900 ms
ConfidenceInterval = [1.790 ms; 1.862 ms] (CI 99.9%), Margin = 0.036 ms (1.96% of Mean)
Skewness = -0.41, Kurtosis = 2.02, MValue = 2
-------------------- Histogram --------------------
[1.762 ms ; 1.817 ms) | @@@@
[1.817 ms ; 1.859 ms) | @@@@@@@@@@
[1.859 ms ; 1.899 ms) | @@
---------------------------------------------------

JakarJson_Benchmarks.JsonModelHybrid_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 1.973 ms, StdErr = 0.010 ms (0.51%), N = 24, StdDev = 0.049 ms
Min = 1.879 ms, Q1 = 1.940 ms, Median = 1.980 ms, Q3 = 2.009 ms, Max = 2.049 ms
IQR = 0.070 ms, LowerFence = 1.835 ms, UpperFence = 2.114 ms
ConfidenceInterval = [1.935 ms; 2.011 ms] (CI 99.9%), Margin = 0.038 ms (1.92% of Mean)
Skewness = -0.29, Kurtosis = 1.95, MValue = 2
-------------------- Histogram --------------------
[1.857 ms ; 1.896 ms) | @
[1.896 ms ; 1.977 ms) | @@@@@@@@@
[1.977 ms ; 2.022 ms) | @@@@@@@@@@@
[2.022 ms ; 2.071 ms) | @@@
---------------------------------------------------

JakarJson_Benchmarks.SystemTextJson_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 2.542 ms, StdErr = 0.011 ms (0.43%), N = 15, StdDev = 0.042 ms
Min = 2.470 ms, Q1 = 2.521 ms, Median = 2.532 ms, Q3 = 2.572 ms, Max = 2.608 ms
IQR = 0.051 ms, LowerFence = 2.444 ms, UpperFence = 2.649 ms
ConfidenceInterval = [2.498 ms; 2.587 ms] (CI 99.9%), Margin = 0.045 ms (1.77% of Mean)
Skewness = 0.21, Kurtosis = 1.83, MValue = 2
-------------------- Histogram --------------------
[2.461 ms ; 2.562 ms) | @@@@@@@@@@@
[2.562 ms ; 2.630 ms) | @@@@
---------------------------------------------------

JakarJson_Benchmarks.Newtonsoft_String: Job-JWQWGO(Toolchain=InProcessEmitToolchain) [Lines=7500]
Runtime = ; GC =
Mean = 5.566 ms, StdErr = 0.015 ms (0.26%), N = 13, StdDev = 0.052 ms
Min = 5.428 ms, Q1 = 5.547 ms, Median = 5.574 ms, Q3 = 5.591 ms, Max = 5.632 ms
IQR = 0.044 ms, LowerFence = 5.481 ms, UpperFence = 5.657 ms
ConfidenceInterval = [5.503 ms; 5.629 ms] (CI 99.9%), Margin = 0.063 ms (1.13% of Mean)
Skewness = -1.15, Kurtosis = 4.11, MValue = 2
-------------------- Histogram --------------------
[5.398 ms ; 5.511 ms) | @
[5.511 ms ; 5.661 ms) | @@@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 3900X 3.80GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.401
  [Host] : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3

Toolchain=InProcessEmitToolchain

| Method                      | Categories           | Lines | Mean            | Error         | StdDev          | Ratio | RatioSD | Gen0      | Gen1      | Gen2     | Allocated  | Alloc Ratio |
|---------------------------- |--------------------- |------ |----------------:|--------------:|----------------:|------:|--------:|----------:|----------:|---------:|-----------:|------------:|
| StjNode_SumPrices           | DOM                  | 1     |      1,484.7 ns |      11.51 ns |        10.21 ns |  0.90 |    0.01 |    0.3147 |         - |        - |     2640 B |        1.19 |
| JakarJsonNode_SumPrices     | DOM                  | 1     |      1,648.0 ns |      12.97 ns |        12.13 ns |  1.00 |    0.01 |    0.2651 |    0.0019 |        - |     2224 B |        1.00 |
| NewtonsoftJObject_SumPrices | DOM                  | 1     |      3,140.2 ns |      52.18 ns |        43.57 ns |  1.91 |    0.03 |    1.1253 |    0.0267 |        - |     9440 B |        4.24 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| StjNode_SumPrices           | DOM                  | 75    |     56,317.3 ns |     545.08 ns |       455.17 ns |  0.88 |    0.01 |   10.8032 |    2.1362 |        - |    90720 B |        1.17 |
| JakarJsonNode_SumPrices     | DOM                  | 75    |     64,182.6 ns |     300.39 ns |       280.99 ns |  1.00 |    0.01 |    9.1553 |    1.5869 |        - |    77337 B |        1.00 |
| NewtonsoftJObject_SumPrices | DOM                  | 75    |    112,766.6 ns |     445.56 ns |       372.06 ns |  1.76 |    0.01 |   22.2168 |    6.3477 |        - |   185993 B |        2.40 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| StjNode_SumPrices           | DOM                  | 7500  |  7,761,500.6 ns |  96,618.68 ns |    90,377.17 ns |  0.88 |    0.01 |  843.7500 |  781.2500 |        - |  8942702 B |        1.17 |
| JakarJsonNode_SumPrices     | DOM                  | 7500  |  8,854,730.5 ns | 106,823.61 ns |    89,202.55 ns |  1.00 |    0.01 |  906.2500 |  828.1250 |        - |  7674886 B |        1.00 |
| NewtonsoftJObject_SumPrices | DOM                  | 7500  | 43,052,801.4 ns | 833,783.71 ns | 1,113,077.40 ns |  4.86 |    0.13 | 3062.5000 | 3000.0000 | 937.5000 | 17888628 B |        2.33 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| SystemTextJson_FromUtf8     | Deserialize UTF-8    | 1     |      1,128.3 ns |       3.93 ns |         3.48 ns |  0.87 |    0.00 |    0.1030 |         - |        - |      864 B |        2.35 |
| JakarJson_FromUtf8          | Deserialize UTF-8    | 1     |      1,301.4 ns |       3.34 ns |         2.79 ns |  1.00 |    0.00 |    0.0439 |         - |        - |      368 B |        1.00 |
| JsonModelHybrid_FromUtf8    | Deserialize UTF-8    | 1     |      1,357.7 ns |       5.72 ns |         5.07 ns |  1.04 |    0.00 |    0.0439 |         - |        - |      368 B |        1.00 |
| Newtonsoft_FromUtf8         | Deserialize UTF-8    | 1     |      2,691.0 ns |      28.00 ns |        26.19 ns |  2.07 |    0.02 |    1.0529 |    0.0153 |        - |     8832 B |       24.00 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| SystemTextJson_FromUtf8     | Deserialize UTF-8    | 75    |     39,633.2 ns |      58.83 ns |        52.15 ns |  0.95 |    0.00 |    1.6479 |    0.0610 |        - |    14216 B |        1.16 |
| JsonModelHybrid_FromUtf8    | Deserialize UTF-8    | 75    |     39,813.0 ns |      98.32 ns |        91.97 ns |  0.95 |    0.00 |    1.4038 |         - |        - |    12208 B |        1.00 |
| JakarJson_FromUtf8          | Deserialize UTF-8    | 75    |     41,912.2 ns |     107.91 ns |       100.94 ns |  1.00 |    0.00 |    1.4038 |         - |        - |    12208 B |        1.00 |
| Newtonsoft_FromUtf8         | Deserialize UTF-8    | 75    |     67,840.4 ns |     182.50 ns |       161.78 ns |  1.62 |    0.01 |    3.0518 |    0.1221 |        - |    26137 B |        2.14 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JsonModelHybrid_FromUtf8    | Deserialize UTF-8    | 7500  |  3,962,130.1 ns |  17,277.98 ns |    16,161.83 ns |  0.95 |    0.00 |  148.4375 |   78.1250 |        - |  1259455 B |        1.00 |
| SystemTextJson_FromUtf8     | Deserialize UTF-8    | 7500  |  3,983,544.8 ns |  15,399.02 ns |    12,858.88 ns |  0.95 |    0.00 |  156.2500 |  109.3750 |        - |  1331231 B |        1.06 |
| JakarJson_FromUtf8          | Deserialize UTF-8    | 7500  |  4,176,249.1 ns |  10,059.26 ns |     8,917.28 ns |  1.00 |    0.00 |  148.4375 |   78.1250 |        - |  1259455 B |        1.00 |
| Newtonsoft_FromUtf8         | Deserialize UTF-8    | 7500  |  6,806,619.4 ns |  24,597.16 ns |    21,804.74 ns |  1.63 |    0.01 |  203.1250 |  117.1875 |        - |  1744308 B |        1.38 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_FromString        | Deserialize string   | 1     |      1,209.0 ns |       2.23 ns |         1.97 ns |  1.00 |    0.00 |    0.0439 |         - |        - |      368 B |        1.00 |
| JsonModelHybrid_FromString  | Deserialize string   | 1     |      1,218.9 ns |       2.56 ns |         2.40 ns |  1.01 |    0.00 |    0.0439 |         - |        - |      368 B |        1.00 |
| SystemTextJson_FromString   | Deserialize string   | 1     |      1,219.2 ns |       6.56 ns |         5.48 ns |  1.01 |    0.00 |    0.1030 |         - |        - |      864 B |        2.35 |
| Newtonsoft_FromString       | Deserialize string   | 1     |      2,463.1 ns |      48.05 ns |        51.41 ns |  2.04 |    0.04 |    0.6981 |    0.0076 |        - |     5856 B |       15.91 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_FromString        | Deserialize string   | 75    |     37,494.2 ns |      48.21 ns |        45.09 ns |  1.00 |    0.00 |    1.4038 |         - |        - |    12208 B |        1.00 |
| JsonModelHybrid_FromString  | Deserialize string   | 75    |     37,749.6 ns |      48.55 ns |        37.91 ns |  1.01 |    0.00 |    1.4038 |         - |        - |    12208 B |        1.00 |
| SystemTextJson_FromString   | Deserialize string   | 75    |     41,350.0 ns |      94.07 ns |        83.39 ns |  1.10 |    0.00 |    1.6479 |    0.0610 |        - |    14216 B |        1.16 |
| Newtonsoft_FromString       | Deserialize string   | 75    |     65,517.8 ns |     330.19 ns |       292.70 ns |  1.75 |    0.01 |    2.6855 |    0.1221 |        - |    23161 B |        1.90 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JsonModelHybrid_FromString  | Deserialize string   | 7500  |  3,751,741.7 ns |   5,682.34 ns |     5,037.25 ns |  1.00 |    0.00 |  148.4375 |   78.1250 |        - |  1259431 B |        1.00 |
| JakarJson_FromString        | Deserialize string   | 7500  |  3,756,448.2 ns |   5,528.84 ns |     4,616.84 ns |  1.00 |    0.00 |  148.4375 |   78.1250 |        - |  1259431 B |        1.00 |
| SystemTextJson_FromString   | Deserialize string   | 7500  |  4,179,005.2 ns |  16,968.78 ns |    15,872.61 ns |  1.11 |    0.00 |  156.2500 |  109.3750 |        - |  1331231 B |        1.06 |
| Newtonsoft_FromString       | Deserialize string   | 7500  |  6,484,745.9 ns |  12,531.57 ns |    11,108.91 ns |  1.73 |    0.00 |  203.1250 |  117.1875 |        - |  1741260 B |        1.38 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| StjDocument_SumPrices       | Navigate (read-only) | 1     |        756.9 ns |       4.98 ns |         4.42 ns |  0.95 |    0.01 |    0.0086 |         - |        - |       72 B |        1.12 |
| JakarJsonTape_SumPrices     | Navigate (read-only) | 1     |        794.1 ns |       1.29 ns |         1.08 ns |  1.00 |    0.00 |    0.0076 |         - |        - |       64 B |        1.00 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| StjDocument_SumPrices       | Navigate (read-only) | 75    |     28,456.1 ns |      47.15 ns |        44.11 ns |  0.98 |    0.00 |         - |         - |        - |       72 B |        1.12 |
| JakarJsonTape_SumPrices     | Navigate (read-only) | 75    |     29,067.8 ns |      20.90 ns |        19.55 ns |  1.00 |    0.00 |         - |         - |        - |       64 B |        1.00 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| StjDocument_SumPrices       | Navigate (read-only) | 7500  |  3,595,262.7 ns |  69,179.24 ns |    79,666.92 ns |  0.97 |    0.03 |         - |         - |        - |       95 B |        1.09 |
| JakarJsonTape_SumPrices     | Navigate (read-only) | 7500  |  3,722,510.3 ns |  73,975.38 ns |    93,555.17 ns |  1.00 |    0.03 |         - |         - |        - |       87 B |        1.00 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_Stream            | Serialize Stream     | 1     |        564.6 ns |      10.86 ns |        10.16 ns |  1.00 |    0.02 |         - |         - |        - |          - |          NA |
| JsonModelHybrid_Stream      | Serialize Stream     | 1     |        619.0 ns |       6.75 ns |         6.31 ns |  1.10 |    0.02 |         - |         - |        - |          - |          NA |
| SystemTextJson_Stream       | Serialize Stream     | 1     |        656.8 ns |      12.95 ns |        20.54 ns |  1.16 |    0.04 |    0.0067 |         - |        - |       56 B |          NA |
| Newtonsoft_Stream           | Serialize Stream     | 1     |      2,787.3 ns |      52.97 ns |        61.00 ns |  4.94 |    0.14 |    1.1368 |    0.0305 |        - |     9528 B |          NA |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_Stream            | Serialize Stream     | 75    |     20,347.2 ns |     405.06 ns |       497.45 ns |  1.00 |    0.03 |         - |         - |        - |          - |          NA |
| JsonModelHybrid_Stream      | Serialize Stream     | 75    |     21,033.9 ns |     323.57 ns |       302.67 ns |  1.03 |    0.03 |         - |         - |        - |          - |          NA |
| SystemTextJson_Stream       | Serialize Stream     | 75    |     22,846.1 ns |     274.01 ns |       242.91 ns |  1.12 |    0.03 |         - |         - |        - |       56 B |          NA |
| Newtonsoft_Stream           | Serialize Stream     | 75    |     68,091.9 ns |     658.64 ns |       550.00 ns |  3.35 |    0.09 |    3.5400 |    0.2441 |        - |    30137 B |          NA |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JsonModelHybrid_Stream      | Serialize Stream     | 7500  |  2,019,507.1 ns |  40,176.48 ns |    41,258.28 ns |  1.00 |    0.03 |         - |         - |        - |       22 B |        0.96 |
| JakarJson_Stream            | Serialize Stream     | 7500  |  2,019,902.6 ns |  37,890.55 ns |    38,910.81 ns |  1.00 |    0.03 |         - |         - |        - |       23 B |        1.00 |
| SystemTextJson_Stream       | Serialize Stream     | 7500  |  3,170,092.8 ns |  40,882.89 ns |    38,241.88 ns |  1.57 |    0.03 |         - |         - |        - |      527 B |       22.91 |
| Newtonsoft_Stream           | Serialize Stream     | 7500  |  6,396,410.1 ns |  98,534.03 ns |    92,168.79 ns |  3.17 |    0.07 |  101.5625 |         - |        - |   861783 B |   37,468.83 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JsonModelHybrid_Utf8        | Serialize UTF-8      | 1     |        567.6 ns |      10.71 ns |        11.00 ns |  0.98 |    0.02 |    0.0334 |         - |        - |      280 B |        1.00 |
| JakarJson_Utf8_IntoBuffer   | Serialize UTF-8      | 1     |        574.3 ns |       9.95 ns |         8.31 ns |  0.99 |    0.02 |         - |         - |        - |          - |        0.00 |
| JakarJson_Utf8              | Serialize UTF-8      | 1     |        579.1 ns |       4.36 ns |         3.86 ns |  1.00 |    0.01 |    0.0334 |         - |        - |      280 B |        1.00 |
| SystemTextJson_Utf8         | Serialize UTF-8      | 1     |        625.8 ns |       5.83 ns |         4.87 ns |  1.08 |    0.01 |    0.0324 |         - |        - |      272 B |        0.97 |
| Newtonsoft_Utf8             | Serialize UTF-8      | 1     |      2,923.3 ns |      55.95 ns |        74.69 ns |  5.05 |    0.13 |    1.2093 |    0.0343 |        - |    10128 B |       36.17 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_Utf8_IntoBuffer   | Serialize UTF-8      | 75    |     19,947.4 ns |     380.00 ns |       373.21 ns |  0.91 |    0.02 |         - |         - |        - |          - |        0.00 |
| JsonModelHybrid_Utf8        | Serialize UTF-8      | 75    |     21,660.3 ns |     271.80 ns |       254.24 ns |  0.99 |    0.02 |    0.9155 |         - |        - |     7848 B |        1.00 |
| JakarJson_Utf8              | Serialize UTF-8      | 75    |     21,829.1 ns |     270.08 ns |       252.63 ns |  1.00 |    0.02 |    0.9155 |         - |        - |     7848 B |        1.00 |
| SystemTextJson_Utf8         | Serialize UTF-8      | 75    |     23,309.5 ns |     465.28 ns |       553.88 ns |  1.07 |    0.03 |    0.9766 |         - |        - |     8200 B |        1.04 |
| Newtonsoft_Utf8             | Serialize UTF-8      | 75    |     70,944.8 ns |   1,344.14 ns |     1,494.01 ns |  3.25 |    0.08 |    5.8594 |    0.3662 |        - |    50017 B |        6.37 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_Utf8_IntoBuffer   | Serialize UTF-8      | 7500  |  1,964,757.8 ns |  27,404.60 ns |    25,634.28 ns |  0.92 |    0.02 |         - |         - |        - |       12 B |       0.000 |
| JakarJson_Utf8              | Serialize UTF-8      | 7500  |  2,126,733.9 ns |  33,824.00 ns |    29,984.09 ns |  1.00 |    0.02 |    7.8125 |    7.8125 |   7.8125 |   781406 B |       1.000 |
| JsonModelHybrid_Utf8        | Serialize UTF-8      | 7500  |  2,138,968.1 ns |  37,273.03 ns |    34,865.22 ns |  1.01 |    0.02 |    7.8125 |    7.8125 |   7.8125 |   781406 B |       1.000 |
| SystemTextJson_Utf8         | Serialize UTF-8      | 7500  |  2,377,260.7 ns |  44,105.14 ns |    41,255.98 ns |  1.12 |    0.02 |    7.8125 |    7.8125 |   7.8125 |   818886 B |       1.048 |
| Newtonsoft_Utf8             | Serialize UTF-8      | 7500  |  6,684,799.0 ns | 114,774.88 ns |   107,360.49 ns |  3.14 |    0.07 |  156.2500 |   46.8750 |  39.0625 |  3730824 B |       4.775 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_Chars_IntoBuffer  | Serialize string     | 1     |        507.3 ns |       9.72 ns |         9.98 ns |  0.95 |    0.02 |         - |         - |        - |          - |        0.00 |
| JsonModelHybrid_String      | Serialize string     | 1     |        524.0 ns |      10.32 ns |        13.06 ns |  0.99 |    0.03 |    0.0629 |         - |        - |      528 B |        1.00 |
| JakarJson_String            | Serialize string     | 1     |        531.6 ns |       9.71 ns |         9.08 ns |  1.00 |    0.02 |    0.0629 |         - |        - |      528 B |        1.00 |
| SystemTextJson_String       | Serialize string     | 1     |        686.3 ns |      11.94 ns |        11.17 ns |  1.29 |    0.03 |    0.0601 |         - |        - |      504 B |        0.95 |
| Newtonsoft_String           | Serialize string     | 1     |      1,926.7 ns |      37.80 ns |        55.40 ns |  3.63 |    0.12 |    0.3090 |         - |        - |     2616 B |        4.95 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_Chars_IntoBuffer  | Serialize string     | 75    |     15,500.9 ns |     305.05 ns |       417.56 ns |  0.89 |    0.03 |         - |         - |        - |          - |        0.00 |
| JakarJson_String            | Serialize string     | 75    |     17,512.9 ns |     335.04 ns |       329.06 ns |  1.00 |    0.03 |    1.8005 |         - |        - |    15208 B |        1.00 |
| JsonModelHybrid_String      | Serialize string     | 75    |     17,702.2 ns |     352.14 ns |       527.07 ns |  1.01 |    0.03 |    1.8005 |         - |        - |    15208 B |        1.00 |
| SystemTextJson_String       | Serialize string     | 75    |     24,460.6 ns |     483.52 ns |       537.43 ns |  1.40 |    0.04 |    1.9531 |         - |        - |    16376 B |        1.08 |
| Newtonsoft_String           | Serialize string     | 75    |     55,321.4 ns |   1,087.89 ns |     1,209.19 ns |  3.16 |    0.09 |    4.7607 |    0.1221 |        - |    40336 B |        2.65 |
|                             |                      |       |                 |               |                 |       |         |           |           |          |            |             |
| JakarJson_Chars_IntoBuffer  | Serialize string     | 7500  |  1,506,950.6 ns |  28,794.88 ns |    30,810.20 ns |  0.83 |    0.02 |         - |         - |        - |       12 B |       0.000 |
| JakarJson_String            | Serialize string     | 7500  |  1,826,195.8 ns |  35,850.53 ns |    35,210.01 ns |  1.00 |    0.03 |   33.2031 |   33.2031 |  33.2031 |  1517850 B |       1.000 |
| JsonModelHybrid_String      | Serialize string     | 7500  |  1,973,056.7 ns |  37,880.58 ns |    49,255.46 ns |  1.08 |    0.03 |   19.5313 |   19.5313 |  19.5313 |  1517780 B |       1.000 |
| SystemTextJson_String       | Serialize string     | 7500  |  2,542,442.1 ns |  44,915.42 ns |    42,013.91 ns |  1.39 |    0.03 |   31.2500 |   31.2500 |  31.2500 |  1637817 B |       1.079 |
| Newtonsoft_String           | Serialize string     | 7500  |  5,566,081.9 ns |  62,820.85 ns |    52,458.25 ns |  3.05 |    0.06 |  281.2500 |  164.0625 |  15.6250 |  3592182 B |       2.367 |

// * Hints *
Outliers
  JakarJson_Benchmarks.StjNode_SumPrices: Toolchain=InProcessEmitToolchain           -> 1 outlier  was  removed (1.52 us)
  JakarJson_Benchmarks.NewtonsoftJObject_SumPrices: Toolchain=InProcessEmitToolchain -> 2 outliers were removed, 3 outliers were detected (3.02 us, 3.24 us, 3.31 us)
  JakarJson_Benchmarks.StjNode_SumPrices: Toolchain=InProcessEmitToolchain           -> 2 outliers were removed, 3 outliers were detected (55.33 us, 57.47 us, 57.75 us)
  JakarJson_Benchmarks.NewtonsoftJObject_SumPrices: Toolchain=InProcessEmitToolchain -> 2 outliers were removed, 3 outliers were detected (111.88 us, 114.06 us, 114.97 us)
  JakarJson_Benchmarks.JakarJsonNode_SumPrices: Toolchain=InProcessEmitToolchain     -> 2 outliers were removed (9.19 ms, 9.26 ms)
  JakarJson_Benchmarks.NewtonsoftJObject_SumPrices: Toolchain=InProcessEmitToolchain -> 1 outlier  was  removed (48.04 ms)
  JakarJson_Benchmarks.SystemTextJson_FromUtf8: Toolchain=InProcessEmitToolchain     -> 1 outlier  was  removed, 2 outliers were detected (1.12 us, 1.14 us)
  JakarJson_Benchmarks.JakarJson_FromUtf8: Toolchain=InProcessEmitToolchain          -> 2 outliers were removed (1.32 us, 1.32 us)
  JakarJson_Benchmarks.JsonModelHybrid_FromUtf8: Toolchain=InProcessEmitToolchain    -> 1 outlier  was  removed (1.38 us)
  JakarJson_Benchmarks.Newtonsoft_FromUtf8: Toolchain=InProcessEmitToolchain         -> 1 outlier  was  detected (2.63 us)
  JakarJson_Benchmarks.SystemTextJson_FromUtf8: Toolchain=InProcessEmitToolchain     -> 1 outlier  was  removed (39.79 us)
  JakarJson_Benchmarks.Newtonsoft_FromUtf8: Toolchain=InProcessEmitToolchain         -> 1 outlier  was  removed (68.24 us)
  JakarJson_Benchmarks.SystemTextJson_FromUtf8: Toolchain=InProcessEmitToolchain     -> 2 outliers were removed (4.03 ms, 4.04 ms)
  JakarJson_Benchmarks.JakarJson_FromUtf8: Toolchain=InProcessEmitToolchain          -> 1 outlier  was  removed (4.21 ms)
  JakarJson_Benchmarks.Newtonsoft_FromUtf8: Toolchain=InProcessEmitToolchain         -> 1 outlier  was  removed (6.91 ms)
  JakarJson_Benchmarks.JakarJson_FromString: Toolchain=InProcessEmitToolchain        -> 1 outlier  was  removed (1.22 us)
  JakarJson_Benchmarks.SystemTextJson_FromString: Toolchain=InProcessEmitToolchain   -> 2 outliers were removed (1.24 us, 1.25 us)
  JakarJson_Benchmarks.JsonModelHybrid_FromString: Toolchain=InProcessEmitToolchain  -> 3 outliers were removed (38.11 us..38.32 us)
  JakarJson_Benchmarks.SystemTextJson_FromString: Toolchain=InProcessEmitToolchain   -> 1 outlier  was  removed (41.76 us)
  JakarJson_Benchmarks.Newtonsoft_FromString: Toolchain=InProcessEmitToolchain       -> 1 outlier  was  removed (66.72 us)
  JakarJson_Benchmarks.JsonModelHybrid_FromString: Toolchain=InProcessEmitToolchain  -> 1 outlier  was  removed (3.76 ms)
  JakarJson_Benchmarks.JakarJson_FromString: Toolchain=InProcessEmitToolchain        -> 2 outliers were removed (3.78 ms, 3.80 ms)
  JakarJson_Benchmarks.Newtonsoft_FromString: Toolchain=InProcessEmitToolchain       -> 1 outlier  was  removed (6.57 ms)
  JakarJson_Benchmarks.StjDocument_SumPrices: Toolchain=InProcessEmitToolchain       -> 1 outlier  was  removed (775.55 ns)
  JakarJson_Benchmarks.JakarJsonTape_SumPrices: Toolchain=InProcessEmitToolchain     -> 2 outliers were removed (801.23 ns, 801.73 ns)
  JakarJson_Benchmarks.StjDocument_SumPrices: Toolchain=InProcessEmitToolchain       -> 1 outlier  was  detected (3.35 ms)
  JakarJson_Benchmarks.SystemTextJson_Stream: Toolchain=InProcessEmitToolchain       -> 1 outlier  was  detected (602.67 ns)
  JakarJson_Benchmarks.Newtonsoft_Stream: Toolchain=InProcessEmitToolchain           -> 1 outlier  was  detected (2.62 us)
  JakarJson_Benchmarks.JakarJson_Stream: Toolchain=InProcessEmitToolchain            -> 2 outliers were detected (19.16 us, 19.20 us)
  JakarJson_Benchmarks.SystemTextJson_Stream: Toolchain=InProcessEmitToolchain       -> 1 outlier  was  removed, 2 outliers were detected (22.16 us, 23.41 us)
  JakarJson_Benchmarks.Newtonsoft_Stream: Toolchain=InProcessEmitToolchain           -> 2 outliers were removed (70.10 us, 70.28 us)
  JakarJson_Benchmarks.JsonModelHybrid_Stream: Toolchain=InProcessEmitToolchain      -> 1 outlier  was  detected (1.92 ms)
  JakarJson_Benchmarks.Newtonsoft_Stream: Toolchain=InProcessEmitToolchain           -> 2 outliers were detected (6.20 ms, 6.22 ms)
  JakarJson_Benchmarks.JakarJson_Utf8_IntoBuffer: Toolchain=InProcessEmitToolchain   -> 2 outliers were removed, 3 outliers were detected (554.61 ns, 605.51 ns, 608.54 ns)
  JakarJson_Benchmarks.JakarJson_Utf8: Toolchain=InProcessEmitToolchain              -> 1 outlier  was  removed (595.73 ns)
  JakarJson_Benchmarks.SystemTextJson_Utf8: Toolchain=InProcessEmitToolchain         -> 2 outliers were removed (651.88 ns, 661.36 ns)
  JakarJson_Benchmarks.Newtonsoft_Utf8: Toolchain=InProcessEmitToolchain             -> 1 outlier  was  removed, 2 outliers were detected (2.76 us, 3.09 us)
  JakarJson_Benchmarks.JsonModelHybrid_Utf8: Toolchain=InProcessEmitToolchain        -> 1 outlier  was  detected (21.10 us)
  JakarJson_Benchmarks.SystemTextJson_Utf8: Toolchain=InProcessEmitToolchain         -> 1 outlier  was  detected (21.60 us)
  JakarJson_Benchmarks.JakarJson_Utf8: Toolchain=InProcessEmitToolchain              -> 1 outlier  was  removed (2.23 ms)
  JakarJson_Benchmarks.SystemTextJson_String: Toolchain=InProcessEmitToolchain       -> 1 outlier  was  detected (666.21 ns)
  JakarJson_Benchmarks.JsonModelHybrid_String: Toolchain=InProcessEmitToolchain      -> 1 outlier  was  detected (16.19 us)
  JakarJson_Benchmarks.JakarJson_Chars_IntoBuffer: Toolchain=InProcessEmitToolchain  -> 3 outliers were detected (1.42 ms..1.48 ms)
  JakarJson_Benchmarks.JakarJson_String: Toolchain=InProcessEmitToolchain            -> 2 outliers were removed, 5 outliers were detected (1.77 ms..1.77 ms, 1.91 ms, 1.93 ms)
  JakarJson_Benchmarks.Newtonsoft_String: Toolchain=InProcessEmitToolchain           -> 2 outliers were removed, 3 outliers were detected (5.43 ms, 5.72 ms, 5.80 ms)

// * Legends *
  Categories  : All categories of the corresponded method, class, and assembly
  Lines       : Value of the 'Lines' parameter
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Ratio       : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD     : Standard deviation of the ratio distribution ([Current]/[Baseline])
  Gen0        : GC Generation 0 collects per 1000 operations
  Gen1        : GC Generation 1 collects per 1000 operations
  Gen2        : GC Generation 2 collects per 1000 operations
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  Alloc Ratio : Allocated memory ratio distribution ([Current]/[Baseline])
  1 ns        : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:26:34 (1594.7 sec), executed benchmarks: 81

Global total time: 00:26:36 (1596.55 sec), executed benchmarks: 81
// * Artifacts cleanup *
Artifacts cleanup is finished
*/



/// <summary>
///     Jakar.Json (<c>[JsonModel]</c>) against <c>[JsonModel]</c> (System.Text.Json source generation, the <c>SystemTextJson_*</c> methods) and Newtonsoft.Json (reflection), on the same model shape (Jakar.Json SPEC.md §12.5).
///     Jakar.Json is the baseline of every category: the context-less <c>[JsonModel]</c> <see cref="BenchOrder"/>, and <c>JsonTape</c> / <c>JNode</c> for the dynamic ones.
///     <para>
///         Each library gets its own model type (<see cref="BenchOrder"/> / <see cref="StjOrder"/>): BenchOrder is a context-less [JsonModel] (Jakar.Json only);
///         StjOrder has a context and Implementation = SystemTextJson, which would otherwise pick Jakar.Json;
///         HybridOrder is a plain context-backed [JsonModel], so its JsonModelHybrid_* rows measure IJsonModel on Jakar.Json (what Jakar.Extensions models use).
///         Newtonsoft reads and writes <see cref="StjOrder"/>, which is a plain POCO to it. All three write enums as names.
///     </para>
///     Run: <c>dotnet run -c Release --project Jakar.Extensions.Experiments -- --bench --filter *JakarJson*</c>
/// </summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory, BenchmarkLogicalGroupRule.ByParams)]
[CategoriesColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class JakarJson_Benchmarks
{
    private static readonly JsonSerializerSettings __newtonsoftSettings   = new() { Converters = [new StringEnumConverter()] };
    private static readonly NewtonsoftSerializer   __newtonsoftSerializer = NewtonsoftSerializer.Create(__newtonsoftSettings);
    private static readonly UTF8Encoding           __utf8NoBom            = new(false);

    private readonly char[]       __chars  = new char[4 * 1024 * 1024];
    private readonly byte[]       __bytes  = new byte[4 * 1024 * 1024];
    private readonly MemoryStream __output = new(4 * 1024 * 1024);

    private BenchOrder  __order    = null!;
    private StjOrder    __stjOrder = null!;
    private HybridOrder __hybrid   = null!;
    private string      __json     = "";
    private byte[]      __utf8     = [];


    /// <summary> Lines per order: ≈ 200 B, 10 KB and 1 MB documents. </summary>
    [Params(1, 75, 7_500)] public int Lines { get; set; }


    [GlobalSetup] public void Setup()
    {
        Random random = new(510);

        __order = new BenchOrder
                  {
                      Id       = new Guid(Enumerable.Range(0, 16).Select(i => (byte)random.Next(256)).ToArray()),
                      Customer = "Ada Lovelace",
                      Created  = new DateTimeOffset(2026, 10, 7, 12, 0, 0, TimeSpan.Zero),
                      Status   = BenchStatus.Paid,
                      Lines = Enumerable.Range(0, Lines)
                                        .Select(i => new BenchLine
                                                     {
                                                         Sku      = $"SKU-{i:D6}",
                                                         Name     = $"Item \"{i}\" — Ø",
                                                         Quantity = random.Next(1, 100),
                                                         Price    = Math.Round((decimal)random.NextDouble() * 100, 2),
                                                         Taxable  = i % 2 == 0
                                                     })
                                        .ToList()
                  };

        __stjOrder = StjOrder.From(__order);
        __hybrid   = HybridOrder.From(__order);

        // Every library reads the same document. All three accept the others' output; the inputs differ only in whitespace (Jakar.Json writes "name" : value).
        __json = __order.ToJson();
        __utf8 = __order.ToJsonUtf8();

        Verify();
    }

    /// <summary> Fails setup, instead of reporting meaningless numbers, if a library reads the shared document differently. </summary>
    private void Verify()
    {
        string expected = __stjOrder.ToJson();

        Check(nameof(SystemTextJson_FromString),  SystemTextJson_FromString().ToJson());
        Check(nameof(SystemTextJson_FromUtf8),    SystemTextJson_FromUtf8().ToJson());
        Check(nameof(Newtonsoft_FromString),      Newtonsoft_FromString()!.ToJson());
        Check(nameof(Newtonsoft_FromUtf8),        Newtonsoft_FromUtf8()!.ToJson());
        Check(nameof(JakarJson_FromString),       StjOrder.From(JakarJson_FromString()).ToJson());
        Check(nameof(JakarJson_FromUtf8),         StjOrder.From(JakarJson_FromUtf8()).ToJson());
        Check(nameof(Newtonsoft_String),          StjOrder.FromJson(Newtonsoft_String()).ToJson());
        Check(nameof(JsonModelHybrid_FromString), StjOrder.From(JsonModelHybrid_FromString().ToBench()).ToJson());
        Check(nameof(JsonModelHybrid_FromUtf8),   StjOrder.From(JsonModelHybrid_FromUtf8().ToBench()).ToJson());
        Check(nameof(JsonModelHybrid_String),     StjOrder.FromJson(JsonModelHybrid_String()).ToJson());

        // The hybrid rows only mean something if [JsonModel] really picked Jakar.Json for HybridOrder (it falls back to System.Text.Json otherwise: JJSON016).
        if ( !typeof(HybridOrder).GetInterfaces().Any(static i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(JJ.IJsonSerializable<>)) ) { throw new InvalidOperationException("HybridOrder isn't hybrid: [JsonModel] fell back to System.Text.Json."); }

        decimal sum = __order.Lines.Sum(static line => line.Price);
        if ( JakarJsonTape_SumPrices() != sum || JakarJsonNode_SumPrices() != sum || StjDocument_SumPrices() != sum || StjNode_SumPrices() != sum || NewtonsoftJObject_SumPrices() != sum ) { throw new InvalidOperationException("The navigation benchmarks disagree on the sum of prices."); }

        void Check( string name, string actual )
        {
            if ( actual != expected ) { throw new InvalidOperationException($"{name} doesn't round-trip the benchmark document."); }
        }
    }


    // ─── Serialize ───────────────────────────────────────────────────────────

    [Benchmark] [BenchmarkCategory(                 "Serialize string")] public string SystemTextJson_String()  => __stjOrder.ToJson();
    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize string")] public string JakarJson_String()       => __order.ToJson();
    [Benchmark] [BenchmarkCategory(                 "Serialize string")] public string Newtonsoft_String()      => JsonConvert.SerializeObject(__stjOrder, __newtonsoftSettings);
    [Benchmark] [BenchmarkCategory(                 "Serialize string")] public string JsonModelHybrid_String() => __hybrid.ToJson();

    [Benchmark] [BenchmarkCategory("Serialize string")] public int JakarJson_Chars_IntoBuffer() => __order.TryFormat(__chars, out int written, default, null)
                                                                                                       ? written
                                                                                                       : -1;

    [Benchmark] [BenchmarkCategory(                 "Serialize UTF-8")] public byte[] SystemTextJson_Utf8()  => __stjOrder.ToUtf8Json();
    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize UTF-8")] public byte[] JakarJson_Utf8()       => __order.ToJsonUtf8();
    [Benchmark] [BenchmarkCategory(                 "Serialize UTF-8")] public byte[] JsonModelHybrid_Utf8() => __hybrid.ToUtf8Json();

    [Benchmark] [BenchmarkCategory("Serialize UTF-8")] public byte[] Newtonsoft_Utf8()
    {
        using MemoryStream stream = new();

        using ( StreamWriter text = new(stream, __utf8NoBom, 4096, true) )
        using ( JsonTextWriter writer = new(text) ) { __newtonsoftSerializer.Serialize(writer, __stjOrder); }

        return stream.ToArray();
    }

    [Benchmark] [BenchmarkCategory("Serialize UTF-8")] public int JakarJson_Utf8_IntoBuffer() => __order.TryFormat(__bytes, out int written, default, null)
                                                                                                     ? written
                                                                                                     : -1;

    [Benchmark] [BenchmarkCategory("Serialize Stream")] public long SystemTextJson_Stream()
    {
        __output.Position = 0;
        __stjOrder.WriteToAsync(__output).GetAwaiter().GetResult();
        return __output.Position;
    }

    [Benchmark] [BenchmarkCategory("Serialize Stream")] public long JsonModelHybrid_Stream()
    {
        __output.Position = 0;
        __hybrid.WriteToAsync(__output).GetAwaiter().GetResult();
        return __output.Position;
    }

    [Benchmark(Baseline = true)] [BenchmarkCategory("Serialize Stream")] public long JakarJson_Stream()
    {
        __output.Position = 0;
        __order.ToJson(__output);
        return __output.Position;
    }

    [Benchmark] [BenchmarkCategory("Serialize Stream")] public long Newtonsoft_Stream()
    {
        __output.Position = 0;

        using ( StreamWriter text = new(__output, __utf8NoBom, 4096, true) )
        using ( JsonTextWriter writer = new(text) ) { __newtonsoftSerializer.Serialize(writer, __stjOrder); }

        return __output.Position;
    }


    // ─── Deserialize ─────────────────────────────────────────────────────────

    [Benchmark] [BenchmarkCategory(                 "Deserialize string")] public StjOrder    SystemTextJson_FromString()  => StjOrder.FromJson(__json);
    [Benchmark(Baseline = true)] [BenchmarkCategory("Deserialize string")] public BenchOrder  JakarJson_FromString()       => BenchOrder.FromJson(__json);
    [Benchmark] [BenchmarkCategory(                 "Deserialize string")] public StjOrder?   Newtonsoft_FromString()      => JsonConvert.DeserializeObject<StjOrder>(__json, __newtonsoftSettings);
    [Benchmark] [BenchmarkCategory(                 "Deserialize string")] public HybridOrder JsonModelHybrid_FromString() => HybridOrder.FromJson(__json);

    [Benchmark] [BenchmarkCategory(                 "Deserialize UTF-8")] public StjOrder    SystemTextJson_FromUtf8()  => StjOrder.FromJson(__utf8.AsSpan());
    [Benchmark(Baseline = true)] [BenchmarkCategory("Deserialize UTF-8")] public BenchOrder  JakarJson_FromUtf8()       => BenchOrder.FromJson(__utf8.AsSpan());
    [Benchmark] [BenchmarkCategory(                 "Deserialize UTF-8")] public HybridOrder JsonModelHybrid_FromUtf8() => HybridOrder.FromJson(__utf8.AsSpan());

    [Benchmark] [BenchmarkCategory("Deserialize UTF-8")] public StjOrder? Newtonsoft_FromUtf8()
    {
        using StreamReader   text   = new(new MemoryStream(__utf8, false), __utf8NoBom, false);
        using JsonTextReader reader = new(text);
        return __newtonsoftSerializer.Deserialize<StjOrder>(reader);
    }


    // ─── Dynamic ─────────────────────────────────────────────────────────────

    [Benchmark] [BenchmarkCategory("Navigate (read-only)")] public decimal StjDocument_SumPrices()
    {
        using JsonDocument document = JsonDocument.Parse(__utf8);
        decimal            sum      = 0;
        foreach ( JsonElement line in document.RootElement.GetProperty("Lines").EnumerateArray() ) { sum += line.GetProperty("Price").GetDecimal(); }

        return sum;
    }

    [Benchmark(Baseline = true)] [BenchmarkCategory("Navigate (read-only)")] public decimal JakarJsonTape_SumPrices()
    {
        using JJ.JsonTape tape = JJ.JsonTape.Parse(__utf8);
        decimal           sum  = 0;
        foreach ( JJ.JsonItem line in tape.Root["Lines"].EnumerateArray() ) { sum += line["Price"].GetDecimal(); }

        return sum;
    }

    [Benchmark] [BenchmarkCategory("DOM")] public decimal StjNode_SumPrices()
    {
        System.Text.Json.Nodes.JsonNode root = System.Text.Json.Nodes.JsonNode.Parse(__utf8)!;
        decimal                         sum  = 0;
        foreach ( System.Text.Json.Nodes.JsonNode? line in root["Lines"]!.AsArray() ) { sum += line!["Price"]!.GetValue<decimal>(); }

        return sum;
    }

    [Benchmark(Baseline = true)] [BenchmarkCategory("DOM")] public decimal JakarJsonNode_SumPrices()
    {
        JJ.JNode root = JJ.JNode.Parse(__utf8.AsSpan())!;
        decimal  sum  = 0;
        foreach ( JJ.JNode? line in root["Lines"]!.AsArray() ) { sum += (decimal)line!["Price"]; }

        return sum;
    }

    [Benchmark] [BenchmarkCategory("DOM")] public decimal NewtonsoftJObject_SumPrices()
    {
        JObject root = JObject.Parse(__json);
        decimal sum  = 0;
        foreach ( JToken line in (JArray)root["Lines"]! ) { sum += (decimal)line["Price"]!; }

        return sum;
    }
}



public enum BenchStatus
{
    Draft,
    Sent,
    Paid
}



// ─── Jakar.Json ──────────────────────────────────────────────────────────────



[JsonModel]
public sealed partial class BenchOrder
{
    public Guid            Id       { get; set; }
    public string          Customer { get; set; } = "";
    public DateTimeOffset  Created  { get; set; }
    public BenchStatus     Status   { get; set; }
    public List<BenchLine> Lines    { get; set; } = [];
}



[JsonModel]
public sealed partial class BenchLine
{
    public string  Sku      { get; set; } = "";
    public string  Name     { get; set; } = "";
    public int     Quantity { get; set; }
    public decimal Price    { get; set; }
    public bool    Taxable  { get; set; }
}



// ─── [JsonModel] / System.Text.Json (and Newtonsoft.Json, by reflection) ─────



[JsonModel(typeof(BenchContext), Implementation = JsonModelImplementation.SystemTextJson)] // the System.Text.Json baseline: without this, [JsonModel] would pick Jakar.Json
public sealed partial class StjOrder
{
    public Guid           Id       { get; set; }
    public string         Customer { get; set; } = "";
    public DateTimeOffset Created  { get; set; }
    public BenchStatus    Status   { get; set; }
    public List<StjLine>  Lines    { get; set; } = [];


    public static StjOrder From( BenchOrder order ) => new()
                                                       {
                                                           Id       = order.Id,
                                                           Customer = order.Customer,
                                                           Created  = order.Created,
                                                           Status   = order.Status,
                                                           Lines =
                                                           [
                                                               .. order.Lines.Select(static line => new StjLine
                                                                                                    {
                                                                                                        Sku      = line.Sku,
                                                                                                        Name     = line.Name,
                                                                                                        Quantity = line.Quantity,
                                                                                                        Price    = line.Price,
                                                                                                        Taxable  = line.Taxable
                                                                                                    })
                                                           ]
                                                       };
}



[JsonModel(typeof(BenchContext), Implementation = JsonModelImplementation.SystemTextJson)] // the System.Text.Json baseline: without this, [JsonModel] would pick Jakar.Json
public sealed partial class StjLine
{
    public string  Sku      { get; set; } = "";
    public string  Name     { get; set; } = "";
    public int     Quantity { get; set; }
    public decimal Price    { get; set; }
    public bool    Taxable  { get; set; }
}



// ─── [JsonModel] hybrid: IJsonModel on Jakar.Json, the path Jakar.Extensions models take ──



/// <summary> A plain context-backed <c>[JsonModel]</c>: it picks Jakar.Json (Implementation = Auto), so its <c>IJsonModel</c> members measure the hybrid path end to end. </summary>
[JsonModel(typeof(BenchContext))]
public sealed partial class HybridOrder
{
    public Guid             Id       { get; set; }
    public string           Customer { get; set; } = "";
    public DateTimeOffset   Created  { get; set; }
    public BenchStatus      Status   { get; set; }
    public List<HybridLine> Lines    { get; set; } = [];


    public static HybridOrder From( BenchOrder order ) => new()
                                                          {
                                                              Id       = order.Id,
                                                              Customer = order.Customer,
                                                              Created  = order.Created,
                                                              Status   = order.Status,
                                                              Lines =
                                                              [
                                                                  .. order.Lines.Select(static line => new HybridLine
                                                                                                       {
                                                                                                           Sku      = line.Sku,
                                                                                                           Name     = line.Name,
                                                                                                           Quantity = line.Quantity,
                                                                                                           Price    = line.Price,
                                                                                                           Taxable  = line.Taxable
                                                                                                       })
                                                              ]
                                                          };

    public BenchOrder ToBench() => new()
                                   {
                                       Id       = Id,
                                       Customer = Customer,
                                       Created  = Created,
                                       Status   = Status,
                                       Lines =
                                       [
                                           .. Lines.Select(static line => new BenchLine
                                                                          {
                                                                              Sku      = line.Sku,
                                                                              Name     = line.Name,
                                                                              Quantity = line.Quantity,
                                                                              Price    = line.Price,
                                                                              Taxable  = line.Taxable
                                                                          })
                                       ]
                                   };
}



[JsonModel(typeof(BenchContext))]
public sealed partial class HybridLine
{
    public string  Sku      { get; set; } = "";
    public string  Name     { get; set; } = "";
    public int     Quantity { get; set; }
    public decimal Price    { get; set; }
    public bool    Taxable  { get; set; }
}



// Enums as names, like Jakar.Json's default and the Newtonsoft StringEnumConverter above (mirrored into HybridOrder's Jakar.Json settings).
[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(StjOrder))]
[JsonSerializable(typeof(StjLine))]
[JsonSerializable(typeof(HybridOrder))]
[JsonSerializable(typeof(HybridLine))]
public sealed partial class BenchContext : JsonSerializerContext;

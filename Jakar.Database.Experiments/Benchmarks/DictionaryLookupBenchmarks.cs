// Jakar.Extensions :: Experiments
// 11/18/2023  10:37 PM

namespace Jakar.Database.Experiments.Benchmarks;
/*

   BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
   AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
   .NET SDK 8.0.100
   [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
   DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


   | Method                                         | Items  | Mean              | Error           | StdDev          | Ratio    | RatioSD | Gen0     | Gen1     | Gen2     | Allocated  | Alloc Ratio |
   |----------------------------------------------- |------- |------------------:|----------------:|----------------:|---------:|--------:|---------:|---------:|---------:|-----------:|------------:|
   | RandomIndex                                    | 10     |          3.864 ns |       0.0194 ns |       0.0181 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 10     |         44.992 ns |       0.8490 ns |       0.7942 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | RandomIndex                                    | 100    |          3.782 ns |       0.0146 ns |       0.0129 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 100    |         41.279 ns |       0.6971 ns |       0.6521 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | RandomIndex                                    | 500    |          3.774 ns |       0.0108 ns |       0.0090 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 500    |         42.134 ns |       0.6473 ns |       0.6055 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | RandomIndex                                    | 1000   |          3.771 ns |       0.0091 ns |       0.0071 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 1000   |         42.550 ns |       0.8442 ns |       0.7897 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | RandomIndex                                    | 5000   |          3.778 ns |       0.0197 ns |       0.0175 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 5000   |         42.535 ns |       0.5739 ns |       0.5368 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | RandomIndex                                    | 10000  |          3.893 ns |       0.0232 ns |       0.0194 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 10000  |         41.635 ns |       0.8161 ns |       0.7633 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | RandomIndex                                    | 50000  |          3.833 ns |       0.0156 ns |       0.0138 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 50000  |         45.197 ns |       0.4807 ns |       0.3753 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | RandomIndex                                    | 100000 |          3.819 ns |       0.0094 ns |       0.0079 ns |        ? |       ? |        - |        - |        - |          - |           ? |
   | RandomPair                                     | 100000 |         44.721 ns |       0.2066 ns |       0.1831 ns |        ? |       ? |   0.0105 |        - |        - |       88 B |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 10     |        215.895 ns |       2.5118 ns |       2.3495 ns | baseline |         |   0.0525 |        - |        - |      440 B |             |
   | ConstructReadOnlyDictionary                    | 10     |        211.863 ns |       3.0159 ns |       2.5184 ns |      -2% |    0.7% |   0.0572 |        - |        - |      480 B |         +9% |
   | ConstructConcurrentDictionary                  | 10     |        544.175 ns |       9.3400 ns |       8.2796 ns |    +152% |    0.9% |   0.2060 |   0.0010 |        - |     1728 B |       +293% |
   | ConstructImmutableDictionary                   | 10     |      1,213.956 ns |       1.7701 ns |       1.3820 ns |    +462% |    1.2% |   0.0839 |        - |        - |      712 B |        +62% |
   | ConstructFrozenDictionary                      | 10     |      4,184.898 ns |      15.7104 ns |      13.9269 ns |  +1,838% |    1.1% |   0.2975 |        - |        - |     2536 B |       +476% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 100    |      1,925.623 ns |       7.7202 ns |       7.2215 ns | baseline |         |   0.3738 |        - |        - |     3128 B |             |
   | ConstructReadOnlyDictionary                    | 100    |      2,036.643 ns |       9.8730 ns |       8.2444 ns |      +6% |    0.5% |   0.3777 |   0.0038 |        - |     3168 B |         +1% |
   | ConstructConcurrentDictionary                  | 100    |      4,034.689 ns |      16.6322 ns |      13.8887 ns |    +109% |    0.5% |   1.1215 |   0.0305 |        - |     9400 B |       +201% |
   | ConstructImmutableDictionary                   | 100    |     15,777.583 ns |      43.0712 ns |      35.9664 ns |    +719% |    0.5% |   0.7629 |        - |        - |     6472 B |       +107% |
   | ConstructFrozenDictionary                      | 100    |     49,128.049 ns |     130.9846 ns |     109.3781 ns |  +2,451% |    0.5% |   3.1128 |   0.0610 |        - |    26240 B |       +739% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 500    |     10,729.854 ns |      83.1796 ns |      69.4587 ns | baseline |         |   1.7548 |        - |        - |    14720 B |             |
   | ConstructReadOnlyDictionary                    | 500    |     10,733.863 ns |      83.2386 ns |      77.8614 ns |      -0% |    0.6% |   1.7548 |   0.0916 |        - |    14760 B |         +0% |
   | ConstructConcurrentDictionary                  | 500    |     21,488.798 ns |     151.4999 ns |     141.7131 ns |    +100% |    0.6% |   5.4321 |   0.7019 |        - |    45624 B |       +210% |
   | ConstructImmutableDictionary                   | 500    |    104,673.588 ns |     521.6990 ns |     487.9975 ns |    +876% |    0.6% |   3.7842 |   0.3662 |        - |    32072 B |       +118% |
   | ConstructFrozenDictionary                      | 500    |    192,140.928 ns |     833.1804 ns |     779.3575 ns |  +1,690% |    0.5% |  15.1367 |   0.9766 |        - |   128136 B |       +770% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 1000   |     20,964.362 ns |     145.3097 ns |     135.9228 ns | baseline |         |   3.6926 |        - |        - |    31016 B |             |
   | ConstructReadOnlyDictionary                    | 1000   |     21,206.079 ns |     130.2362 ns |     121.8230 ns |      +1% |    0.6% |   3.6926 |   0.3052 |        - |    31056 B |         +0% |
   | ConstructConcurrentDictionary                  | 1000   |     48,258.142 ns |     499.7426 ns |     467.4596 ns |    +130% |    0.8% |  11.9629 |   2.9907 |        - |   100480 B |       +224% |
   | ConstructImmutableDictionary                   | 1000   |    240,348.723 ns |   1,280.9209 ns |   1,135.5030 ns |  +1,046% |    1.0% |   7.3242 |   0.9766 |        - |    64072 B |       +107% |
   | ConstructFrozenDictionary                      | 1000   |    566,566.888 ns |   1,982.5860 ns |   1,757.5109 ns |  +2,602% |    0.7% |  29.2969 |   2.9297 |        - |   251321 B |       +710% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 5000   |    179,536.221 ns |   2,474.5575 ns |   2,314.7026 ns | baseline |         |  43.4570 |  43.4570 |  43.4570 |   163653 B |             |
   | ConstructReadOnlyDictionary                    | 5000   |    174,609.749 ns |     978.1149 ns |     816.7703 ns |      -3% |    1.5% |  43.4570 |  43.4570 |  43.4570 |   163693 B |         +0% |
   | ConstructConcurrentDictionary                  | 5000   |    189,918.908 ns |     969.3671 ns |     809.4654 ns |      +6% |    1.5% |  29.5410 |   9.7656 |        - |   247744 B |        +51% |
   | ConstructImmutableDictionary                   | 5000   |  1,453,253.613 ns |   7,087.1214 ns |   6,282.5485 ns |    +709% |    1.0% |  37.1094 |  13.6719 |        - |   320073 B |        +96% |
   | ConstructFrozenDictionary                      | 5000   |  1,763,966.127 ns |   9,608.5717 ns |   8,517.7485 ns |    +881% |    1.5% | 148.4375 | 148.4375 | 148.4375 |   899445 B |       +450% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 10000  |    259,864.498 ns |   1,122.0813 ns |     994.6958 ns | baseline |         |  67.8711 |  63.4766 |  62.9883 |   283495 B |             |
   | ConstructReadOnlyDictionary                    | 10000  |    262,501.664 ns |     990.7377 ns |     878.2632 ns |      +1% |    0.4% |  67.3828 |  62.5000 |  62.5000 |   283489 B |         -0% |
   | ConstructConcurrentDictionary                  | 10000  |  1,794,454.789 ns |  35,504.4833 ns |  93,533.0104 ns |    +548% |    2.9% | 148.4375 | 140.6250 |  46.8750 |  1021031 B |       +260% |
   | ConstructImmutableDictionary                   | 10000  |  3,165,851.107 ns |  10,128.1110 ns |   9,473.8414 ns |  +1,118% |    0.6% |  74.2188 |  35.1563 |        - |   640075 B |       +126% |
   | ConstructFrozenDictionary                      | 10000  |  5,980,773.177 ns |   6,698.2357 ns |   5,229.5418 ns |  +2,201% |    0.4% | 390.6250 | 343.7500 | 328.1250 |  1788527 B |       +531% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 50000  |  1,562,693.385 ns |   9,467.5732 ns |   8,855.9738 ns | baseline |         |  89.8438 |  89.8438 |  89.8438 |  1466935 B |             |
   | ConstructReadOnlyDictionary                    | 50000  |  1,565,946.387 ns |   6,863.8174 ns |   6,420.4191 ns |      +0% |    0.8% |  87.8906 |  87.8906 |  87.8906 |  1466958 B |         +0% |
   | ConstructConcurrentDictionary                  | 50000  |  6,249,346.605 ns |  68,722.6730 ns |  57,386.5444 ns |    +300% |    1.0% | 312.5000 | 304.6875 |  78.1250 |  2419976 B |        +65% |
   | ConstructImmutableDictionary                   | 50000  | 21,395,960.208 ns |  86,870.1283 ns |  81,258.3719 ns |  +1,269% |    0.6% | 375.0000 | 281.2500 |        - |  3200084 B |       +118% |
   | ConstructFrozenDictionary                      | 50000  | 21,382,899.107 ns |  93,759.8406 ns |  83,115.6565 ns |  +1,268% |    0.7% | 500.0000 | 500.0000 | 500.0000 |  8402894 B |       +473% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | ConstructDictionary                            | 100000 |  3,509,028.333 ns |  30,062.5742 ns |  28,120.5506 ns | baseline |         |  85.9375 |  85.9375 |  85.9375 |  3042468 B |             |
   | ConstructReadOnlyDictionary                    | 100000 |  3,473,198.438 ns |  20,309.3738 ns |  18,997.4009 ns |      -1% |    0.8% |  85.9375 |  85.9375 |  85.9375 |  3042502 B |         +0% |
   | ConstructConcurrentDictionary                  | 100000 | 15,188,241.629 ns | 235,011.2092 ns | 208,331.3156 ns |    +333% |    1.5% | 593.7500 | 578.1250 | 125.0000 |  4870229 B |        +60% |
   | ConstructImmutableDictionary                   | 100000 | 49,791,446.429 ns | 241,000.8281 ns | 213,640.9568 ns |  +1,319% |    1.1% | 700.0000 | 600.0000 |        - |  6400173 B |       +110% |
   | ConstructFrozenDictionary                      | 100000 | 40,118,820.130 ns | 188,910.5588 ns | 167,464.2898 ns |  +1,043% |    0.8% | 454.5455 | 454.5455 | 454.5455 | 17328419 B |       +470% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 10     |          7.971 ns |       0.0280 ns |       0.0248 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 10     |         43.870 ns |       0.1602 ns |       0.1420 ns |    +450% |    0.4% |   0.0067 |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 10     |        110.559 ns |       0.5524 ns |       0.5167 ns |  +1,287% |    0.5% |   0.0076 |        - |        - |       64 B |          NA |
   | ImmutableDictionary_ForEach                    | 10     |        483.953 ns |       2.2467 ns |       1.9916 ns |  +5,972% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_ForEach                       | 10     |         16.365 ns |       0.1005 ns |       0.0839 ns |    +105% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 100    |         76.700 ns |       0.2640 ns |       0.2470 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 100    |        306.851 ns |       0.4916 ns |       0.4105 ns |    +300% |    0.3% |   0.0067 |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 100    |        880.482 ns |       3.0048 ns |       2.8107 ns |  +1,048% |    0.5% |   0.0076 |        - |        - |       64 B |          NA |
   | ImmutableDictionary_ForEach                    | 100    |      4,441.218 ns |      10.2189 ns |       9.5588 ns |  +5,690% |    0.4% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_ForEach                       | 100    |        162.535 ns |       2.0592 ns |       1.9262 ns |    +112% |    1.3% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 500    |        365.839 ns |       1.8482 ns |       1.7288 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 500    |      1,450.635 ns |       7.1313 ns |       5.9550 ns |    +296% |    0.7% |   0.0057 |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 500    |      5,075.889 ns |      32.9246 ns |      27.4935 ns |  +1,287% |    0.8% |   0.0076 |        - |        - |       64 B |          NA |
   | ImmutableDictionary_ForEach                    | 500    |     22,303.866 ns |      41.3350 ns |      36.6424 ns |  +5,995% |    0.5% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_ForEach                       | 500    |        762.526 ns |       1.7548 ns |       1.6414 ns |    +108% |    0.5% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 1000   |        938.229 ns |       3.7470 ns |       3.5050 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 1000   |      2,850.634 ns |       4.2671 ns |       3.5633 ns |    +204% |    0.4% |   0.0038 |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 1000   |     11,768.721 ns |      70.1790 ns |      65.6454 ns |  +1,154% |    0.7% |        - |        - |        - |       64 B |          NA |
   | ImmutableDictionary_ForEach                    | 1000   |     39,559.224 ns |      52.7835 ns |      46.7912 ns |  +4,116% |    0.4% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_ForEach                       | 1000   |      1,517.124 ns |       1.6133 ns |       1.5091 ns |     +62% |    0.4% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 5000   |      4,659.921 ns |       4.3677 ns |       3.4100 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 5000   |     14,180.518 ns |      33.6816 ns |      31.5058 ns |    +204% |    0.2% |        - |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 5000   |     60,560.381 ns |     318.7516 ns |     298.1604 ns |  +1,201% |    0.5% |        - |        - |        - |       64 B |          NA |
   | ImmutableDictionary_ForEach                    | 5000   |    190,704.068 ns |     194.5841 ns |     162.4865 ns |  +3,993% |    0.1% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_ForEach                       | 5000   |      7,546.690 ns |      10.0659 ns |       7.8588 ns |     +62% |    0.1% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 10000  |      9,355.086 ns |      54.2376 ns |      50.7338 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 10000  |     28,339.796 ns |      65.5746 ns |      58.1301 ns |    +203% |    0.6% |        - |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 10000  |    157,478.781 ns |     536.2492 ns |     501.6078 ns |  +1,583% |    0.6% |        - |        - |        - |       64 B |          NA |
   | ImmutableDictionary_ForEach                    | 10000  |    412,801.214 ns |   1,328.7397 ns |   1,177.8931 ns |  +4,314% |    0.5% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_ForEach                       | 10000  |     15,166.275 ns |     123.2124 ns |     109.2246 ns |     +62% |    0.7% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 50000  |     35,237.391 ns |      86.5758 ns |      76.7472 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 50000  |    141,236.080 ns |     437.9263 ns |     365.6883 ns |    +301% |    0.3% |        - |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 50000  |    855,711.482 ns |   2,644.8640 ns |   2,344.6030 ns |  +2,328% |    0.3% |        - |        - |        - |       65 B |          NA |
   | ImmutableDictionary_ForEach                    | 50000  |  2,268,926.981 ns |  14,584.9684 ns |  12,929.1946 ns |  +6,339% |    0.6% |        - |        - |        - |        2 B |          NA |
   | FrozenDictionary_ForEach                       | 50000  |     76,556.394 ns |     528.8993 ns |     494.7328 ns |    +117% |    0.7% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_ForEach                             | 100000 |     70,346.920 ns |     109.8671 ns |      91.7440 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_ForEach                     | 100000 |    281,943.924 ns |     570.5088 ns |     533.6543 ns |    +301% |    0.3% |        - |        - |        - |       56 B |          NA |
   | ConcurrentDictionary_ForEach                   | 100000 |  1,444,509.681 ns |   6,860.9206 ns |   6,417.7094 ns |  +1,954% |    0.6% |        - |        - |        - |       65 B |          NA |
   | ImmutableDictionary_ForEach                    | 100000 |  4,748,232.861 ns |  90,426.4773 ns |  88,810.8918 ns |  +6,653% |    2.0% |        - |        - |        - |      292 B |          NA |
   | FrozenDictionary_ForEach                       | 100000 |    150,457.712 ns |     877.6882 ns |     820.9901 ns |    +114% |    0.5% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 10     |         60.072 ns |       1.1563 ns |       1.0816 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 10     |         62.427 ns |       1.0240 ns |       0.9578 ns |      +4% |    3.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 10     |         54.999 ns |       0.4714 ns |       0.3936 ns |      -8% |    2.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 10     |         95.047 ns |       0.5676 ns |       0.5309 ns |     +58% |    1.8% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 10     |         47.159 ns |       0.6665 ns |       0.6235 ns |     -21% |    1.9% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 100    |         65.714 ns |       0.6834 ns |       0.6058 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 100    |         69.164 ns |       0.7918 ns |       0.7407 ns |      +5% |    1.3% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 100    |         64.931 ns |       1.2417 ns |       1.1615 ns |      -1% |    2.1% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 100    |        101.012 ns |       0.6738 ns |       0.6303 ns |     +54% |    1.1% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 100    |         46.908 ns |       0.4990 ns |       0.4667 ns |     -29% |    1.4% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 500    |         68.507 ns |       0.9152 ns |       0.8561 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 500    |         68.313 ns |       1.0200 ns |       0.9541 ns |      -0% |    2.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 500    |         66.329 ns |       0.9785 ns |       0.9153 ns |      -3% |    1.5% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 500    |        115.007 ns |       0.7835 ns |       0.7329 ns |     +68% |    1.2% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 500    |         51.945 ns |       0.3241 ns |       0.3032 ns |     -24% |    1.4% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 1000   |         69.105 ns |       0.7233 ns |       0.6766 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 1000   |         69.269 ns |       1.0986 ns |       1.0276 ns |      +0% |    1.7% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 1000   |         66.779 ns |       1.1386 ns |       1.0651 ns |      -3% |    1.6% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 1000   |        129.353 ns |       1.0875 ns |       1.0172 ns |     +87% |    1.4% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 1000   |         52.544 ns |       0.5093 ns |       0.4515 ns |     -24% |    1.5% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 5000   |         77.748 ns |       0.8640 ns |       0.8082 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 5000   |         77.040 ns |       0.9290 ns |       0.8690 ns |      -1% |    0.9% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 5000   |         78.409 ns |       0.9087 ns |       0.8500 ns |      +1% |    1.3% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 5000   |        154.047 ns |       1.3969 ns |       1.3067 ns |     +98% |    1.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 5000   |         63.616 ns |       0.6444 ns |       0.6027 ns |     -18% |    1.7% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 10000  |         87.553 ns |       0.7708 ns |       0.7210 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 10000  |         88.066 ns |       0.7700 ns |       0.7203 ns |      +1% |    0.7% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 10000  |         84.966 ns |       0.8552 ns |       0.8000 ns |      -3% |    1.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 10000  |        178.119 ns |       0.9547 ns |       0.8930 ns |    +103% |    0.8% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 10000  |         72.917 ns |       1.0071 ns |       0.9420 ns |     -17% |    1.6% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 50000  |        111.530 ns |       1.5623 ns |       1.3849 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 50000  |        132.381 ns |       3.4441 ns |      10.1551 ns |     +18% |    6.8% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 50000  |        124.441 ns |       3.6050 ns |      10.6294 ns |     +11% |    7.8% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 50000  |        256.974 ns |       5.1758 ns |      13.7255 ns |    +131% |    4.8% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 50000  |        113.442 ns |       2.8981 ns |       8.5452 ns |      +2% |   10.4% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random                          | 100000 |        205.170 ns |       4.0552 ns |       6.5485 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_Get_Random                  | 100000 |        203.205 ns |       4.0288 ns |       8.0460 ns |      -1% |    4.7% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_Get_Random                | 100000 |        242.901 ns |       2.7359 ns |       2.4253 ns |     +17% |    2.6% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_Get_Random                 | 100000 |        398.249 ns |       7.8738 ns |      11.2924 ns |     +94% |    4.5% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_Get_Random                    | 100000 |        191.308 ns |       3.8398 ns |       7.9298 ns |      -7% |    5.9% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 10     |         14.215 ns |       0.0737 ns |       0.0653 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 10     |         14.680 ns |       0.0464 ns |       0.0434 ns |      +3% |    0.7% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 10     |         13.219 ns |       0.0511 ns |       0.0427 ns |      -7% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 10     |         37.555 ns |       0.1372 ns |       0.1283 ns |    +164% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 10     |          4.862 ns |       0.0143 ns |       0.0126 ns |     -66% |    0.5% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 100    |         14.236 ns |       0.0827 ns |       0.0774 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 100    |         17.007 ns |       0.0423 ns |       0.0396 ns |     +19% |    0.5% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 100    |         13.484 ns |       0.0628 ns |       0.0587 ns |      -5% |    0.4% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 100    |         38.136 ns |       0.1450 ns |       0.1356 ns |    +168% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 100    |          7.207 ns |       0.0554 ns |       0.0519 ns |     -49% |    1.1% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 500    |         14.802 ns |       0.0681 ns |       0.0637 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 500    |         14.637 ns |       0.0634 ns |       0.0495 ns |      -1% |    0.5% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 500    |         13.519 ns |       0.0543 ns |       0.0508 ns |      -9% |    0.7% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 500    |         41.248 ns |       0.1424 ns |       0.1189 ns |    +179% |    0.5% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 500    |          6.688 ns |       0.0265 ns |       0.0221 ns |     -55% |    0.4% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 1000   |         14.209 ns |       0.0787 ns |       0.0657 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 1000   |         14.744 ns |       0.1117 ns |       0.1045 ns |      +4% |    0.7% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 1000   |         13.696 ns |       0.0520 ns |       0.0487 ns |      -4% |    0.7% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 1000   |         41.180 ns |       0.1340 ns |       0.1188 ns |    +190% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 1000   |          6.400 ns |       0.0842 ns |       0.0703 ns |     -55% |    1.3% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 5000   |         15.136 ns |       0.1085 ns |       0.1015 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 5000   |         15.926 ns |       0.0916 ns |       0.0857 ns |      +5% |    0.7% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 5000   |         13.744 ns |       0.1131 ns |       0.1003 ns |      -9% |    1.0% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 5000   |         47.143 ns |       0.3040 ns |       0.2844 ns |    +211% |    0.9% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 5000   |          6.572 ns |       0.1579 ns |       0.1477 ns |     -57% |    2.3% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 10000  |         14.177 ns |       0.0720 ns |       0.0601 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 10000  |         15.540 ns |       0.0960 ns |       0.0898 ns |     +10% |    0.7% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 10000  |         13.931 ns |       0.0530 ns |       0.0496 ns |      -2% |    0.6% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 10000  |         44.317 ns |       0.1351 ns |       0.1264 ns |    +212% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 10000  |          7.078 ns |       0.0355 ns |       0.0332 ns |     -50% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 50000  |         14.252 ns |       0.1026 ns |       0.0959 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 50000  |         15.539 ns |       0.1152 ns |       0.1077 ns |      +9% |    0.9% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 50000  |         13.616 ns |       0.1014 ns |       0.0899 ns |      -4% |    0.9% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 50000  |         48.891 ns |       0.2707 ns |       0.2400 ns |    +243% |    0.9% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 50000  |          7.049 ns |       0.0563 ns |       0.0526 ns |     -51% |    1.0% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_Get_Random_Single                   | 100000 |         14.239 ns |       0.0909 ns |       0.0851 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_Get_Random_Single           | 100000 |         14.678 ns |       0.0844 ns |       0.0789 ns |      +3% |    0.7% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_Get_Random_Single         | 100000 |         13.926 ns |       0.0675 ns |       0.0564 ns |      -2% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_Get_Random_Single          | 100000 |         51.267 ns |       0.1360 ns |       0.1272 ns |    +260% |    0.7% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_Get_Random_Single             | 100000 |          7.378 ns |       0.0377 ns |       0.0334 ns |     -48% |    0.7% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 10     |         13.442 ns |       0.0904 ns |       0.0755 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 10     |         14.798 ns |       0.0624 ns |       0.0521 ns |     +10% |    0.6% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 10     |         13.087 ns |       0.0608 ns |       0.0539 ns |      -3% |    0.6% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 10     |         37.806 ns |       0.1679 ns |       0.1402 ns |    +181% |    0.7% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 10     |          6.601 ns |       0.0371 ns |       0.0310 ns |     -51% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 100    |         13.497 ns |       0.0660 ns |       0.0618 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 100    |         14.857 ns |       0.0853 ns |       0.0798 ns |     +10% |    0.6% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 100    |         13.021 ns |       0.0403 ns |       0.0357 ns |      -3% |    0.6% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 100    |         39.584 ns |       0.1425 ns |       0.1333 ns |    +193% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 100    |          6.872 ns |       0.0298 ns |       0.0249 ns |     -49% |    0.5% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 500    |         13.481 ns |       0.0649 ns |       0.0575 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 500    |         14.829 ns |       0.0535 ns |       0.0474 ns |     +10% |    0.5% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 500    |         13.097 ns |       0.0628 ns |       0.0587 ns |      -3% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 500    |         44.199 ns |       0.1726 ns |       0.1530 ns |    +228% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 500    |          6.885 ns |       0.0442 ns |       0.0369 ns |     -49% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 1000   |         13.462 ns |       0.0788 ns |       0.0698 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 1000   |         14.866 ns |       0.0673 ns |       0.0596 ns |     +10% |    0.7% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 1000   |         13.117 ns |       0.0539 ns |       0.0478 ns |      -3% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 1000   |         43.471 ns |       0.1036 ns |       0.0918 ns |    +223% |    0.5% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 1000   |          6.483 ns |       0.0263 ns |       0.0246 ns |     -52% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 5000   |         13.446 ns |       0.0518 ns |       0.0484 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 5000   |         14.878 ns |       0.0697 ns |       0.0652 ns |     +11% |    0.5% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 5000   |         13.081 ns |       0.0628 ns |       0.0556 ns |      -3% |    0.6% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 5000   |         45.014 ns |       0.1087 ns |       0.0964 ns |    +235% |    0.3% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 5000   |          6.475 ns |       0.0297 ns |       0.0278 ns |     -52% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 10000  |         13.438 ns |       0.0939 ns |       0.0878 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 10000  |         14.859 ns |       0.0674 ns |       0.0631 ns |     +11% |    0.8% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 10000  |         13.054 ns |       0.0432 ns |       0.0404 ns |      -3% |    0.8% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 10000  |         43.879 ns |       0.2491 ns |       0.2330 ns |    +227% |    0.9% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 10000  |          7.243 ns |       0.0449 ns |       0.0398 ns |     -46% |    0.9% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 50000  |         13.383 ns |       0.0681 ns |       0.0604 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 50000  |         14.830 ns |       0.0545 ns |       0.0483 ns |     +11% |    0.6% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 50000  |         13.130 ns |       0.0384 ns |       0.0360 ns |      -2% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 50000  |         46.153 ns |       0.1008 ns |       0.0787 ns |    +245% |    0.5% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 50000  |          7.292 ns |       0.0363 ns |       0.0339 ns |     -46% |    0.7% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_First                   | 100000 |         13.528 ns |       0.0827 ns |       0.0773 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_First           | 100000 |         14.818 ns |       0.0667 ns |       0.0591 ns |     +10% |    0.8% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_First         | 100000 |         13.038 ns |       0.0652 ns |       0.0610 ns |      -4% |    0.8% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_First          | 100000 |         50.500 ns |       0.3318 ns |       0.3104 ns |    +273% |    0.8% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_First             | 100000 |          7.232 ns |       0.0287 ns |       0.0268 ns |     -47% |    0.8% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 10     |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 10     |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 10     |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 10     |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 10     |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 100    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 100    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 100    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 100    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 100    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 500    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 500    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 500    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 500    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 500    |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 1000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 1000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 1000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 1000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 1000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 5000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 5000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 5000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 5000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 5000   |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 10000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 10000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 10000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 10000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 10000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 50000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 50000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 50000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 50000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 50000  |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Last                    | 100000 |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ReadOnlyDictionary_TryGetValue_Last            | 100000 |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ConcurrentDictionary_TryGetValue_Last          | 100000 |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | ImmutableDictionary_TryGetValue_Last           | 100000 |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   | FrozenDictionary_TryGetValue_Last              | 100000 |                NA |              NA |              NA |        ? |       ? |       NA |       NA |       NA |         NA |           ? |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 10     |         63.263 ns |       1.0237 ns |       0.9576 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 10     |         58.908 ns |       0.3993 ns |       0.3735 ns |      -7% |    1.6% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 10     |         64.422 ns |       0.6948 ns |       0.6499 ns |      +2% |    2.2% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 10     |         91.517 ns |       0.7615 ns |       0.7123 ns |     +45% |    1.9% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 10     |         46.395 ns |       0.6987 ns |       0.6194 ns |     -27% |    2.0% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 100    |         63.969 ns |       0.8167 ns |       0.7640 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 100    |         62.176 ns |       1.0126 ns |       0.9472 ns |      -3% |    2.4% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 100    |         64.244 ns |       0.8950 ns |       0.8371 ns |      +0% |    2.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 100    |        121.470 ns |       1.0246 ns |       0.9584 ns |     +90% |    1.2% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 100    |         49.796 ns |       0.6348 ns |       0.5628 ns |     -22% |    1.1% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 500    |         66.466 ns |       0.6358 ns |       0.5947 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 500    |         64.767 ns |       1.0421 ns |       0.9748 ns |      -3% |    1.5% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 500    |         64.132 ns |       0.9353 ns |       0.8748 ns |      -3% |    1.9% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 500    |        121.609 ns |       0.8980 ns |       0.7960 ns |     +83% |    1.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 500    |         51.420 ns |       0.6187 ns |       0.5787 ns |     -23% |    1.5% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 1000   |         63.814 ns |       0.8522 ns |       0.7971 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 1000   |         64.609 ns |       0.8932 ns |       0.8355 ns |      +1% |    1.6% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 1000   |         65.162 ns |       0.9552 ns |       0.8935 ns |      +2% |    1.2% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 1000   |        128.787 ns |       1.4614 ns |       1.3670 ns |    +102% |    1.9% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 1000   |         51.778 ns |       0.3990 ns |       0.3732 ns |     -19% |    1.6% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 5000   |         75.511 ns |       0.6967 ns |       0.6517 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 5000   |         77.670 ns |       0.6754 ns |       0.5987 ns |      +3% |    0.9% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 5000   |         79.128 ns |       0.5917 ns |       0.5535 ns |      +5% |    1.1% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 5000   |        162.091 ns |       0.8617 ns |       0.7639 ns |    +115% |    1.2% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 5000   |         64.503 ns |       1.0506 ns |       0.9828 ns |     -15% |    1.5% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 10000  |         97.357 ns |       1.3885 ns |       1.2988 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 10000  |         85.234 ns |       1.0080 ns |       0.9429 ns |     -12% |    1.7% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 10000  |         84.000 ns |       1.0061 ns |       0.9411 ns |     -14% |    1.8% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 10000  |        174.602 ns |       1.3415 ns |       1.1892 ns |     +79% |    1.3% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 10000  |         74.397 ns |       0.7638 ns |       0.7145 ns |     -24% |    1.5% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 50000  |        169.919 ns |       3.3799 ns |       4.8473 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 50000  |        148.851 ns |       2.9940 ns |       3.6769 ns |     -12% |    3.7% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 50000  |        166.440 ns |       3.3251 ns |       7.9668 ns |      -2% |    7.4% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 50000  |        292.313 ns |       5.8739 ns |      11.8656 ns |     +72% |    5.0% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 50000  |        124.075 ns |       2.6064 ns |       7.6441 ns |     -27% |    7.7% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random                  | 100000 |        200.042 ns |       3.7460 ns |       7.5671 ns | baseline |         |   0.0105 |        - |        - |       88 B |             |
   | ReadOnlyDictionary_TryGetValue_Random          | 100000 |        208.728 ns |       6.3869 ns |      18.8319 ns |      +5% |    9.1% |   0.0105 |        - |        - |       88 B |         +0% |
   | ConcurrentDictionary_TryGetValue_Random        | 100000 |        241.525 ns |       8.2384 ns |      24.2912 ns |     +21% |   10.7% |   0.0105 |        - |        - |       88 B |         +0% |
   | ImmutableDictionary_TryGetValue_Random         | 100000 |        401.784 ns |       7.9531 ns |       9.1589 ns |    +100% |    5.1% |   0.0105 |        - |        - |       88 B |         +0% |
   | FrozenDictionary_TryGetValue_Random            | 100000 |        185.756 ns |       3.7059 ns |       9.9557 ns |      -7% |    6.9% |   0.0105 |        - |        - |       88 B |         +0% |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 10     |         16.187 ns |       0.0643 ns |       0.0602 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 10     |         14.781 ns |       0.0705 ns |       0.0589 ns |      -9% |    0.6% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 10     |         12.961 ns |       0.0208 ns |       0.0162 ns |     -20% |    0.4% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 10     |         35.470 ns |       0.1346 ns |       0.1259 ns |    +119% |    0.4% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 10     |          6.611 ns |       0.0522 ns |       0.0488 ns |     -59% |    0.9% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 100    |         14.583 ns |       0.0944 ns |       0.0883 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 100    |         15.781 ns |       0.0644 ns |       0.0570 ns |      +8% |    0.8% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 100    |         13.003 ns |       0.0561 ns |       0.0497 ns |     -11% |    0.7% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 100    |         40.421 ns |       0.1317 ns |       0.1232 ns |    +177% |    0.5% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 100    |          6.916 ns |       0.0586 ns |       0.0520 ns |     -53% |    1.1% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 500    |         13.498 ns |       0.0848 ns |       0.0793 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 500    |         14.792 ns |       0.1700 ns |       0.1590 ns |     +10% |    1.5% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 500    |         13.022 ns |       0.0689 ns |       0.0611 ns |      -4% |    0.7% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 500    |         41.856 ns |       0.1424 ns |       0.1262 ns |    +210% |    0.7% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 500    |          6.878 ns |       0.0277 ns |       0.0260 ns |     -49% |    0.7% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 1000   |         13.425 ns |       0.0658 ns |       0.0583 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 1000   |         17.576 ns |       0.0768 ns |       0.0681 ns |     +31% |    0.5% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 1000   |         13.008 ns |       0.0250 ns |       0.0209 ns |      -3% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 1000   |         41.496 ns |       0.1460 ns |       0.1366 ns |    +209% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 1000   |          6.487 ns |       0.0280 ns |       0.0233 ns |     -52% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 5000   |         14.611 ns |       0.0980 ns |       0.0869 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 5000   |         14.748 ns |       0.0502 ns |       0.0445 ns |      +1% |    0.6% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 5000   |         12.965 ns |       0.0180 ns |       0.0140 ns |     -11% |    0.7% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 5000   |         42.003 ns |       0.0741 ns |       0.0657 ns |    +187% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 5000   |          6.495 ns |       0.0335 ns |       0.0313 ns |     -56% |    0.9% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 10000  |         14.542 ns |       0.0720 ns |       0.0639 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 10000  |         14.823 ns |       0.0801 ns |       0.0710 ns |      +2% |    0.8% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 10000  |         12.994 ns |       0.0571 ns |       0.0534 ns |     -11% |    0.6% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 10000  |         42.852 ns |       0.1409 ns |       0.1249 ns |    +195% |    0.4% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 10000  |          7.183 ns |       0.0254 ns |       0.0237 ns |     -51% |    0.6% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 50000  |         16.335 ns |       0.0748 ns |       0.0699 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 50000  |         14.734 ns |       0.0469 ns |       0.0439 ns |     -10% |    0.5% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 50000  |         14.165 ns |       0.0508 ns |       0.0424 ns |     -13% |    0.5% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 50000  |         46.920 ns |       0.1865 ns |       0.1653 ns |    +187% |    0.7% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 50000  |          7.233 ns |       0.0359 ns |       0.0318 ns |     -56% |    0.7% |        - |        - |        - |          - |          NA |
   |                                                |        |                   |                 |                 |          |         |          |          |          |            |             |
   | Dictionary_TryGetValue_Random_Single           | 100000 |         13.403 ns |       0.0604 ns |       0.0536 ns | baseline |         |        - |        - |        - |          - |          NA |
   | ReadOnlyDictionary_TryGetValue_Random_Single   | 100000 |         14.796 ns |       0.0673 ns |       0.0629 ns |     +10% |    0.6% |        - |        - |        - |          - |          NA |
   | ConcurrentDictionary_TryGetValue_Random_Single | 100000 |         12.974 ns |       0.0439 ns |       0.0411 ns |      -3% |    0.4% |        - |        - |        - |          - |          NA |
   | ImmutableDictionary_TryGetValue_Random_Single  | 100000 |         50.535 ns |       0.1787 ns |       0.1584 ns |    +277% |    0.6% |        - |        - |        - |          - |          NA |
   | FrozenDictionary_TryGetValue_Random_Single     | 100000 |          7.823 ns |       0.0245 ns |       0.0229 ns |     -42% |    0.6% |        - |        - |        - |          - |          NA |

 */



[Config( typeof(BenchmarkConfig) ), GroupBenchmarksBy( BenchmarkLogicalGroupRule.ByCategory ), SimpleJob( RuntimeMoniker.HostProcess ), MemoryDiagnoser, SuppressMessage( "ReSharper", "LoopCanBeConvertedToQuery" )]
public class DictionaryLookupBenchmarks
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ConcurrentDictionary<string, int> _concurrentDictionary;
    private Dictionary<string, int>           _dictionary;
    private FrozenDictionary<string, int>     _frozenDictionary;
    private ImmutableDictionary<string, int>  _immutableDictionary;
    private KeyValuePair<string, int>         _randomPair;
    private KeyValuePair<string, int>[]       _items;
    private ReadOnlyDictionary<string, int>   _readOnlyDictionary;
    private string                            _firstKey;
    private string                            _lastKey;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    [Params( 10, 100, 500, 1000, 5000, 10_000, 50_000, 100_000 )] public int Items { get; set; }


    [GlobalSetup]
    public void GlobalSetup()
    {
        _items                = Enumerable.Range( 0, Items ).Select( static _ => new KeyValuePair<string, int>( Guid.CreateVersion7().ToString(), Random.Shared.Next() ) ).ToArray();
        _firstKey             = _items.First().Key;
        _lastKey              = _items.Last().Key;
        _randomPair           = RandomPair();
        _dictionary           = new Dictionary<string, int>( _items );
        _readOnlyDictionary   = new ReadOnlyDictionary<string, int>( _items.ToDictionary( i => i.Key, i => i.Value ) );
        _concurrentDictionary = new ConcurrentDictionary<string, int>( _items );
        _immutableDictionary  = _items.ToImmutableDictionary();
        _frozenDictionary     = _items.ToFrozenDictionary();
    }


    [BenchmarkCategory( "Commons" ), Benchmark] public int                       RandomIndex() => Random.Shared.Next( 0, Items );
    [BenchmarkCategory( "Commons" ), Benchmark] public KeyValuePair<string, int> RandomPair()  => _items.Random( Random.Shared ).First();


    [BenchmarkCategory( "Construct" ), Benchmark( Baseline = true )] public Dictionary<string, int>           ConstructDictionary()           => new(_items);
    [BenchmarkCategory( "Construct" ), Benchmark]                    public ReadOnlyDictionary<string, int>   ConstructReadOnlyDictionary()   => new(ConstructDictionary());
    [BenchmarkCategory( "Construct" ), Benchmark]                    public ConcurrentDictionary<string, int> ConstructConcurrentDictionary() => new(_items);
    [BenchmarkCategory( "Construct" ), Benchmark]                    public ImmutableDictionary<string, int>  ConstructImmutableDictionary()  => _items.ToImmutableDictionary();
    [BenchmarkCategory( "Construct" ), Benchmark]                    public FrozenDictionary<string, int>     ConstructFrozenDictionary()     => _items.ToFrozenDictionary();


    [BenchmarkCategory( "ForEach" ), Benchmark( Baseline = true )]
    public void Dictionary_ForEach()
    {
        foreach ( KeyValuePair<string, int> _ in _dictionary ) { }
    }
    [BenchmarkCategory( "ForEach" ), Benchmark]
    public void ReadOnlyDictionary_ForEach()
    {
        foreach ( KeyValuePair<string, int> _ in _readOnlyDictionary ) { }
    }
    [BenchmarkCategory( "ForEach" ), Benchmark]
    public void ConcurrentDictionary_ForEach()
    {
        foreach ( KeyValuePair<string, int> _ in _concurrentDictionary ) { }
    }
    [BenchmarkCategory( "ForEach" ), Benchmark]
    public void ImmutableDictionary_ForEach()
    {
        foreach ( KeyValuePair<string, int> _ in _immutableDictionary ) { }
    }
    [BenchmarkCategory( "ForEach" ), Benchmark]
    public void FrozenDictionary_ForEach()
    {
        foreach ( KeyValuePair<string, int> _ in _frozenDictionary ) { }
    }


    [BenchmarkCategory( "TryGetValue_First" ), Benchmark( Baseline = true )] public bool Dictionary_TryGetValue_First()           => _dictionary.TryGetValue( _firstKey, out _ );
    [BenchmarkCategory( "TryGetValue_First" ), Benchmark]                    public bool ReadOnlyDictionary_TryGetValue_First()   => _readOnlyDictionary.TryGetValue( _firstKey, out _ );
    [BenchmarkCategory( "TryGetValue_First" ), Benchmark]                    public bool ConcurrentDictionary_TryGetValue_First() => _concurrentDictionary.TryGetValue( _firstKey, out _ );
    [BenchmarkCategory( "TryGetValue_First" ), Benchmark]                    public bool ImmutableDictionary_TryGetValue_First()  => _immutableDictionary.TryGetValue( _firstKey, out _ );
    [BenchmarkCategory( "TryGetValue_First" ), Benchmark]                    public bool FrozenDictionary_TryGetValue_First()     => _frozenDictionary.TryGetValue( _firstKey, out _ );


    [BenchmarkCategory( "TryGetValue_Last" ), Benchmark( Baseline = true )] public bool Dictionary_TryGetValue_Last()           => _dictionary.TryGetValue( _lastKey, out _ );
    [BenchmarkCategory( "TryGetValue_Last" ), Benchmark]                    public bool ReadOnlyDictionary_TryGetValue_Last()   => _readOnlyDictionary.TryGetValue( _lastKey, out _ );
    [BenchmarkCategory( "TryGetValue_Last" ), Benchmark]                    public bool ConcurrentDictionary_TryGetValue_Last() => _concurrentDictionary.TryGetValue( _lastKey, out _ );
    [BenchmarkCategory( "TryGetValue_Last" ), Benchmark]                    public bool ImmutableDictionary_TryGetValue_Last()  => _immutableDictionary.TryGetValue( _lastKey, out _ );
    [BenchmarkCategory( "TryGetValue_Last" ), Benchmark]                    public bool FrozenDictionary_TryGetValue_Last()     => _frozenDictionary.TryGetValue( _lastKey, out _ );


    [BenchmarkCategory( "TryGetValue_Random" ), Benchmark( Baseline = true )] public bool Dictionary_TryGetValue_Random()           => _dictionary.TryGetValue( RandomPair().Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random" ), Benchmark]                    public bool ReadOnlyDictionary_TryGetValue_Random()   => _readOnlyDictionary.TryGetValue( RandomPair().Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random" ), Benchmark]                    public bool ConcurrentDictionary_TryGetValue_Random() => _concurrentDictionary.TryGetValue( RandomPair().Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random" ), Benchmark]                    public bool ImmutableDictionary_TryGetValue_Random()  => _immutableDictionary.TryGetValue( RandomPair().Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random" ), Benchmark]                    public bool FrozenDictionary_TryGetValue_Random()     => _frozenDictionary.TryGetValue( RandomPair().Key, out _ );


    [BenchmarkCategory( "TryGetValue_Random_Single" ), Benchmark( Baseline = true )] public bool Dictionary_TryGetValue_Random_Single()           => _dictionary.TryGetValue( _randomPair.Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random_Single" ), Benchmark]                    public bool ReadOnlyDictionary_TryGetValue_Random_Single()   => _readOnlyDictionary.TryGetValue( _randomPair.Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random_Single" ), Benchmark]                    public bool ConcurrentDictionary_TryGetValue_Random_Single() => _concurrentDictionary.TryGetValue( _randomPair.Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random_Single" ), Benchmark]                    public bool ImmutableDictionary_TryGetValue_Random_Single()  => _immutableDictionary.TryGetValue( _randomPair.Key, out _ );
    [BenchmarkCategory( "TryGetValue_Random_Single" ), Benchmark]                    public bool FrozenDictionary_TryGetValue_Random_Single()     => _frozenDictionary.TryGetValue( _randomPair.Key, out _ );


    [BenchmarkCategory( "Get_Random_Single" ), Benchmark( Baseline = true )] public int Dictionary_Get_Random_Single()           => _dictionary[_randomPair.Key];
    [BenchmarkCategory( "Get_Random_Single" ), Benchmark]                    public int ReadOnlyDictionary_Get_Random_Single()   => _readOnlyDictionary[_randomPair.Key];
    [BenchmarkCategory( "Get_Random_Single" ), Benchmark]                    public int ConcurrentDictionary_Get_Random_Single() => _concurrentDictionary[_randomPair.Key];
    [BenchmarkCategory( "Get_Random_Single" ), Benchmark]                    public int ImmutableDictionary_Get_Random_Single()  => _immutableDictionary[_randomPair.Key];
    [BenchmarkCategory( "Get_Random_Single" ), Benchmark]                    public int FrozenDictionary_Get_Random_Single()     => _frozenDictionary[_randomPair.Key];


    [BenchmarkCategory( "Get_Random" ), Benchmark( Baseline = true )] public int Dictionary_Get_Random()           => _dictionary[RandomPair().Key];
    [BenchmarkCategory( "Get_Random" ), Benchmark]                    public int ReadOnlyDictionary_Get_Random()   => _readOnlyDictionary[RandomPair().Key];
    [BenchmarkCategory( "Get_Random" ), Benchmark]                    public int ConcurrentDictionary_Get_Random() => _concurrentDictionary[RandomPair().Key];
    [BenchmarkCategory( "Get_Random" ), Benchmark]                    public int ImmutableDictionary_Get_Random()  => _immutableDictionary[RandomPair().Key];
    [BenchmarkCategory( "Get_Random" ), Benchmark]                    public int FrozenDictionary_Get_Random()     => _frozenDictionary[RandomPair().Key];
}

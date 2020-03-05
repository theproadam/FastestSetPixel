# FastestSetPixel
This demo demonstrates the use of memory pointer tricks, bitwise operations and parallelization to reach extremely high speeds of SetPixel. This demo is written in C#.

### These are the benchmarks for a 1920x1080 32bpp buffer:
| Method        | Color?        | Alpha?        | Safe?         | ARGB Speed    | RGB Speed     | Parallel.For? |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| Byte Array    | Yes           | Yes           | Yes           | 4.21ms        | 3.51ms        | Yes           |
| Byte Set      | Yes           | Yes           | No            | 1.01ms        | 0.78ms        | Yes           |
| Integer Set   | Yes           | Yes           | No            | 0.49ms        | 0.49ms        | Yes           |
| Bitwise Set   | Yes           | Yes           | No            | 0.32ms        | 0.29ms        | Yes           |
| RtlZeroMemory | No            | No            | Yes           | 0.38ms        | 0.38ms        | No            |
| RtlZeroMemory2 | No           | No            | Yes           | 0.21ms        | 0.21ms        | Yes           |

<sub>Tested on i7-4710HQ 16GB DDR3 Win8.1, net 4.5<br/>
Speeds calculated from the average of 100 tries.<br/>
With the exception of **Byte Array**, all parallelized loops used lambdas for ~0.4ms time save.<br/>
Color values and resolution values were hardcoded, so expect a slight performance impact for runtime resolved resolutions and colors.<br/>
**Integer Set** only supports ARGB colors, **RtlZeroMemory** only supports zeroing out.<br/>
memset was not included as its performance is lackluster.
</sub>

### Safe Methods
**Byte Array** uses a simple 1920x1080 Parallel.For loop to set each individual pixel with a managed array.<br/>
**RtlZeroMemory** simply zeros the entire buffer out. It is it the fastest non parallelized method.<br/>
**RtlZeroMemory2** uses a 1080x Parallel.For loop to zero out 1920 pixel horizontally 1080 times.

### Unsafe Methods
##### All unsafe methods use a split 1080x Parallel.For loop which then calls a lambda which then sets 1920 pixels.
**Byte Set** sets each ARGB/RGB byte value individually via byte* pointer.<br/>
**Integer Set** treats each pixels ARGB/RGB values as an integer, and sets them with a precalculated integer via int* pointer<br/>
**Bitwise Set** bit shifts ARGB/RGB byte values into each individual pixel via int* pointer.<br/>


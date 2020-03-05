# FastestSetPixel
This demo demonstrates the use of memory pointer tricks, bitwise operations and parallelization to reach extremely high speeds of SetPixel. This demo is written in C#.

### These are the benchmarks for a 1920x1080 32bpp buffer:
| Method        | Color?        | Alpha?        | Safe?         | ARGB Speed    | RGB Speed     | Parallel.For? |
| ------------- | ------------- | ------------- | ------------- | ------------- | ------------- | ------------- |
| Byte Array    | Yes           | Yes           | Yes           | 3.77ms        | 3.62ms        | Yes           |
| Byte Set      | Yes           | Yes           | No            | 1.18ms        | 0.91ms        | Yes           |
| Integer Set   | Yes           | Yes           | No            | 0.47ms        | 0.47ms        | Yes           |
| Bitwise Set   | Yes           | Yes           | No            | 0.83ms        | 0.78ms        | Yes           |
| RtlZeroMemory | No            | No            | Yes           | 0.31ms        | 0.31ms        | No            |
| RtlZeroMemory2| No            | No            | Yes           | 0.22ms        | 0.22ms        | Yes           |

<sub>Tested on i7-4710HQ 16GB DDR3 Win8.1, NET 4.5. Speeds calculated from the average of 100 tries.<br/>
With the exception of **Byte Array**, all parallelized loops used lambdas for ~0.4ms time save.<br/>
Resolution values were hardcoded, so expect a slight performance impact for runtime resolved resolutions.<br/>
With hardcoded color values, **Integer and Bitwise Set** can reach 0.40ms speeds, due to the same IL Code.<br/>
**Integer Set** only supports ARGB colors, **RtlZeroMemory** only supports zeroing out.<br/>
memset was not included as its performance is lackluster.<br/>
All tests were run in release mode, outside of Visual Studio.
</sub>

### Safe Methods
**Byte Array** uses a simple 1920x1080 Parallel.For loop to set each individual pixel with a managed array.<br/>
**RtlZeroMemory** simply zeros the entire buffer out. It is it the fastest non parallelized method.<br/>
**RtlZeroMemory2** uses a 1080x Parallel.For loop to zero out 1920 pixels horizontally, 1080 times.

### Unsafe Methods
##### All unsafe methods use a split 1080x Parallel.For loop which then calls a lambda which then sets 1920 pixels.
**Byte Set** sets each ARGB/RGB byte value individually via byte* pointer.<br/>
**Integer Set** treats each pixels ARGB/RGB values as an integer, and sets them with a precalculated integer via int* pointer<br/>
**Bitwise Set** bit shifts ARGB/RGB byte values into each individual pixel via int* pointer.<br/>

## Conclusion
- For Color Clear, use **Integer Set**, and calculate the integer ARGB bytes from a bitwise operation.
- For Black Clear, use **RtlZeroMemory** as it is the fastest method.

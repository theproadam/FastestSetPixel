using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SetPixelSpeeds
{
    unsafe class Program
    {
        [DllImport("kernel32.dll")]
        static extern void RtlZeroMemory(IntPtr dst, int length);

        static void Main(string[] args)
        {
            //Initialze Buffer, Stopwatch, and ops class.
            IntPtr TargetArray = Marshal.AllocHGlobal(1920 * 1080 * 4);
            Stopwatch sw = new Stopwatch();

            ops op = new ops();
            op.bptr = (byte*)TargetArray;
            op.iptr = (int*)TargetArray;
            op.rptr = TargetArray;
            op.R = 0; op.G = 0; op.B = 0; op.A = 0;
            op.iValue = ((((((byte)0 << 8) | (byte)0) << 8) | (byte)0) << 8) | (byte)0; //ARGB Values

            sw.Reset();
            Console.WriteLine("1920x1080 ARGB Buffer Clear Speed Test:\n");

            //Warmup
            for (int t = 0; t < 100; t++)
                for (int i = 0; i < 1920 * 1080; i++) {}

            Console.WriteLine("Managed Clear: ");
            byte[] BGRA = new byte[1920 * 1080 * 4];


            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080 * 1920, i => {
                    BGRA[i] = 0;
                    BGRA[i + 1] = 0;
                    BGRA[i + 2] = 0;
                    BGRA[i + 3] = 0;
                });
            sw.Stop();
            Console.WriteLine("Standard Byte Array ARGB-> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080 * 1920, i =>
                {
                    BGRA[i] = 0;
                    BGRA[i + 1] = 0;
                    BGRA[i + 2] = 0;
                });
            sw.Stop();
            Console.WriteLine("Standard Byte Array RGB-> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            BGRA = null;
            GC.Collect();

            Console.WriteLine("\nUnmanaged Clear:");
            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080, op.ByteSetARGB);
            sw.Stop();
            Console.WriteLine("ByteSet ARGB-> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080, op.ByteSetRGB);
            sw.Stop();
            Console.WriteLine("ByteSet  RGB-> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080, op.IntSet);
            sw.Stop();
            Console.WriteLine("IntSet  ARGB-> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080, op.BitSet);
            sw.Stop();
            Console.WriteLine("Bitwise  RGB-> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080, op.BitSetARGB);
            sw.Stop();
            Console.WriteLine("Bitwise ARGB-> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            Console.WriteLine("\nBlack Only: ");

            sw.Start();
            for (int t = 0; t < 100; t++)
                RtlZeroMemory(TargetArray, 1920 * 1080 * 4);
            sw.Stop();
            Console.WriteLine("RtlZeroMemory -> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();

            sw.Start();
            for (int t = 0; t < 100; t++)
                Parallel.For(0, 1080, op.RtlSet);
            sw.Stop();
            Console.WriteLine("RtlZeroMemory2 -> " + (sw.Elapsed.TotalMilliseconds / 100f).ToString() + "ms");
            sw.Reset();


            Console.ReadLine();
        }
    }

    unsafe class ops
    {
        [DllImport("kernel32.dll")]
        static extern void RtlZeroMemory(IntPtr dst, int length);

        public int* iptr;
        public byte* bptr;
        public IntPtr rptr;
        int wSD = 1920 * 4;
        public int iValue;
        public byte B, G, R, A; 

        public void ByteSetARGB(int index)
        {
            for (int i = 0; i < 1920; ++i)
            {
                int a = wSD * index + 4 * i;
                *(bptr + a) = B;
                *(bptr + a + 1) = G;
                *(bptr + a + 2) = R;
                *(bptr + a + 3) = A;
            }
        }

        public void ByteSetRGB(int index)
        {
            for (int i = 0; i < 1920; ++i)
            {
                int a = wSD * index + 4 * i;
                *(bptr + a) = B;
                *(bptr + a + 1) = G;
                *(bptr + a + 2) = R;
            }
        }

        public void IntSet(int index)
        {
            for (int i = 0; i < 1920; ++i)
            {
                *(iptr + index * 1920 + i) = iValue;
            }
            //*(iptr + index * 1920 + i) = 0;
           // for (int* i = iptr + index * 1920; i < iptr + index * 1920 + 1920; ++i) *i = 0;
        }

        public void BitSet(int index)
        {
            for (int i = 0; i < 1920; ++i)
            {
                *(iptr + index * 1920 + i) = ((((byte)R << 8) | (byte)G) << 8) | (byte)B;
            }
        }

        public void BitSetARGB(int index)
        {
            for (int i = 0; i < 1920; ++i)
            {
                *(iptr + index * 1920 + i) = ((((((byte)A << 8) | (byte)R) << 8) | (byte)G) << 8) | (byte)B;
            }
        }

        public void RtlSet(int index)
        {
            RtlZeroMemory(IntPtr.Add(rptr, 1920 * 4 * index), 1920 * 4);
        }
    }
}

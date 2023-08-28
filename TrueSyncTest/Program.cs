using TrueSync;
using TrueSync;

class Program
{
    static void Main()
    {
        //for (int x = - 720; x <= 720; x += 30)
        //{
        //    var a = FP.Sin(30 * FP.Deg2Rad);
        //    var b = FP.FastSin(30 * FP.Deg2Rad);
        //    Console.WriteLine(string.Format("{0} {1} {2}", a, b, a == b));
        //}

        //var a = 4f;
        //var b = 2f;
        //var c1 = Math.Atan2(a, b);
        //var d1 = c1 / 3.14f * 180f;
        //Console.WriteLine(c1);

        //var c2 = FP.Atan2(a, b);
        //var d2 = c2 * FP.Rad2Deg;
        //Console.WriteLine(c2);

        Console.WriteLine(Math.Log2(10));
        Console.WriteLine(Math.Log2(5));

        Console.WriteLine(TSMath.Log2(10).AsFloat());
        Console.WriteLine(TSMath.Log2(5).AsFloat());

        //var a1 = Math.Sin(45);
        //var b1 = Math.Cos(45);
        //var c1 = Math.Atan2(a1, b1);
        //Console.WriteLine((c1 * FP.Rad2Deg).AsInt());

        //var a2 = FP.Sin(45 * FP.Deg2Rad);
        //var b2 = FP.Cos(45 * FP.Deg2Rad);
        //var c2 = FP.Atan2(a2, b2);
        //Console.WriteLine(c2);

        //FP Epsilon = 1.192092896e-7f;
        //Console.WriteLine(Epsilon._serializedValue);
    }
}

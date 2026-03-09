using System;
using System.Collections;


public class Program
{
    public class Temp
    {
        public int? a = null;
    }

    static int? Sum(int? a, int b) 
    {
        if (a == null)
        {
            return 0;
        }
        return a + b;
    }

    static void Main(string[]args)
    {
        Temp? a = null;

        Console.WriteLine(Sum(a?.a, 2));
    } 


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace provaOverloading
{
    class Program
    {
        static void Main(string[] args)
        {
            A1 a1;
            A2 a2;
            B1 b1;
            B2 b2;

            a1 = new A2();
            b1 = new B1();

            Filter.F(a1, b1);
        }
    }

    static class Filter
    {
        static public bool F(A1 a1, B1 b1)
        {
            return true;
        }

        static public bool F(A1 a1, B2 b2)
        {
            return true;
        }

        static public bool F(A2 a2, B1 b1)
        {
            return true;
        }

        static public bool F(A2 a2, B2 b2)
        {
            return true;
        }
    }

    public class A1
    {

    }

    public class A2 : A1
    {

    }

    public class B1
    {

    }

    public class B2 : B1
    {

    }
}

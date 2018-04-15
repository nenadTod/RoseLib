using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLibApp.RoseLib.Selectors;
using System;
using System.IO;

namespace RoseLibApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader reader = new StreamReader("..\\netcoreapp2.0\\RoseLib\\Test Cases\\ClassSelector.cs.t"))
            {

            }
        }



        #region Experimental

        public static implicit operator Program(byte b)  // explicit byte to digit conversion operator
        {
            return new Program();
        }

        public static implicit operator Program(int x)  // explicit byte to digit conversion operator
        {
            return new Program();
        }

        public static Program operator +(Program b)
        {
            return new Program();
        }

        #endregion
    }
}

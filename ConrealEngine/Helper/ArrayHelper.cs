using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConrealEngine.Helper
{
    public static class ArrayHelper
    {
        public static T[][] CreateJaggedArray<T>(int dim1, int dim2)
        {
            T[][] array = new T[dim1][];

            for(int i = 0; i < dim1; i++)
            {
                array[i] = new T[dim2];
            }
            return array;
        }

        public static T[][] CreateJaggedArray<T>(int dim1, int dim2, T fill)
        {
            T[][] array = CreateJaggedArray<T>(dim1, dim2);

            for(int x = 0; x < dim1; x++)
            {
                for(int y = 0; y < dim2; y++)
                {
                    array[x][y] = fill;
                }
            }

            return array;
        }

        public static string JaggedCharArrayToString(char[][] arr)
        {
            StringBuilder sb = new StringBuilder();

            foreach(char[] c in arr)
            {
                sb.AppendLine(new string(c));
            }

            return sb.ToString();
        }

        public static List<string> JaggedCharArrayToStringArray(char[][] arr)
        {
            List<string> res = new List<string>();

            foreach (char[] c in arr)
            {
                res.Add(new string(c));
            }

            return res;
        }
    }
}

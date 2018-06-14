
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM4DesignTool.Data
{
    using System.ComponentModel;

    class Test
    {



        public Test()
        {


            int[] numberlist = { 0, 4, 46, 57, 3, 5, 80 };

            for (int i = 0; i < numberlist.Length-1; i++)
            {
                for (int j = 0; j < numberlist.Length - i - 1; j++)
                {
                    int a = numberlist[j];
                    int b = numberlist[j + 1];

                    if (a > b)
                    {
                        numberlist[j] = b;
                        numberlist[j + 1] = a;
                    }
                }

            }

            Console.WriteLine();

        }
    }
}

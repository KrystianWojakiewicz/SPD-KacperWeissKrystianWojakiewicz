﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnsonAndNEHAlgorithms.BackEnd.Static_Algorithms
{
    static class CustomSwaps
    {
        public static void Swap<T>(List<T> list, int index1, int index2)
        {
            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }
    }
}

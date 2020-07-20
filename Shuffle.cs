﻿using System;

namespace Lobstermania
{
    class ArrayShuffle
    {
        public static void Shuffle<T>(T[] items)
        {
            Random rand = LM962.rand;

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
    }
}

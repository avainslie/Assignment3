using System;
using UnityEngine;
using TMPro;

namespace Antymology.UI
{
    public static class NestBlockUI
    {
        public static TMP_Text nestBlockCounter;

        private static int nestBlockCount = 0;

        public static void addNestBlockToCount()
        {
            nestBlockCount++;
            nestBlockCounter.text = nestBlockCount.ToString();
        }
    }
}

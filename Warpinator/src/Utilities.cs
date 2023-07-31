using System;
using System.Collections.Generic;
using SFS.UI;
using UnityEngine;
using Button = SFS.UI.ModGUI.Button;

namespace Warpinator
{
    public class Utilities
    {
        public class GenericPropertyComparer<T, TKey> : IComparer<T>
        {
            private readonly Func<T, TKey> _keySelector;
            private readonly IComparer<TKey> _keyComparer;

            public GenericPropertyComparer(Func<T, TKey> keySelector, IComparer<TKey> keyComparer = null)
            {
                _keySelector = keySelector;
                _keyComparer = keyComparer ?? Comparer<TKey>.Default;
            }

            public int Compare(T x, T y)
            {
                TKey keyX = _keySelector(x);
                TKey keyY = _keySelector(y);
                return _keyComparer.Compare(keyX, keyY);
            }
        }

        public static void FindRowsAndColumns((int min, int max) minMaxRows, (int min, int max) minMaxColumns, int buttonCount, out int rows, out int columns)
        {
            columns = Mathf.Clamp((int)Math.Ceiling((decimal)buttonCount / 13), minMaxColumns.min, minMaxColumns.max);
            Debug.Log("Columns: " + columns);
            rows = Mathf.Clamp((int)Math.Ceiling(buttonCount / (double)columns), minMaxRows.min, minMaxRows.max);
        }

        public static void ButtonTextScale(Button button, float scale)
        {
            button.gameObject.GetComponentInChildren<TextAdapter>().gameObject.transform.localScale =
                new Vector3(scale, scale);
        }
    }
}
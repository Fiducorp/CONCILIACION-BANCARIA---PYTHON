using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MOFIS_ERP.Classes
{
    /// <summary>
    /// Comparador para ordenamiento natural de strings (ej: "1, 2, 10" en lugar de "1, 10, 2")
    /// Utiliza la API de Windows StrCmpLogicalW
    /// </summary>
    public class NaturalSortComparer : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}

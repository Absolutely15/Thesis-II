using System.Collections.Generic;
using UnityEngine;

namespace NVTT
{
    public static class Utilities
    {
        private static readonly Dictionary<float, WaitForSeconds> Wfs = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWfs(float key)
        {
            if (!Wfs.ContainsKey(key))
            {
                Wfs[key] = new WaitForSeconds(key);
            }

            return Wfs[key];
        }
    }
}
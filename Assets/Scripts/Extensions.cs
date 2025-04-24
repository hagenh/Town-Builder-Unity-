using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Items;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Extensions
    {
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static KeyValuePair<Item, int>? SafeGetElementOrNull(this Dictionary<Item, int> dictionary, int index)
        {
            if (index < 0 || index >= dictionary.Count)
            {
                return null;
            }

            return dictionary.ElementAt(index);
        }
    }
}
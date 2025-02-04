using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEngine;

namespace PeddaBombs.Utilities
{
    public class ColorUtil
    {
        public static ReadOnlyDictionary<string, Color> Colors { get; }
        static ColorUtil()
        {
            var dic = new Dictionary<string, Color>();
            foreach (var colorProp in typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)) {
                var color = colorProp.GetValue(typeof(Color));
                if (color is Color value) {
                    dic.Add(colorProp.Name, value);
                }
            }
            Colors = new ReadOnlyDictionary<string, Color>(dic);
        }

        public static bool IsRainbow(string name)
        {
            return name == "rainbow";
        }
    }
}

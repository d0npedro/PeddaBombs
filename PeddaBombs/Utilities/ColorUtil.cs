using System.Collections.Generic;              // Enthält die generischen Collections (Dictionary, List, etc.)
using System.Collections.ObjectModel;          // Ermöglicht die Verwendung von ReadOnlyDictionary
using System.Reflection;                       // Ermöglicht Reflection, um zur Laufzeit auf Typinformationen zuzugreifen
using UnityEngine;                             // Stellt die Unity Engine Klassen, wie Color, zur Verfügung

namespace PeddaBombs.Utilities
{
    // Diese Klasse stellt Hilfsfunktionen zur Verfügung, um Farben zu verwalten.
    public class ColorUtil
    {
        // Eine schreibgeschützte (ReadOnly) Dictionary, in der alle statischen Farben von UnityEngine.Color gespeichert sind.
        // Der Schlüssel ist der Name der Farbe (z.B. "red"), der Wert ist der Color-Wert.
        public static ReadOnlyDictionary<string, Color> Colors { get; }

        // Statischer Konstruktor: Wird einmal beim ersten Zugriff auf die Klasse ausgeführt.
        // Hier werden alle öffentlichen statischen Properties des Typs Color aus der UnityEngine geladen und in ein Dictionary eingefügt.
        static ColorUtil()
        {
            // Erzeugt ein temporäres Dictionary, das später schreibgeschützt gemacht wird.
            var dic = new Dictionary<string, Color>();

            // Durchläuft alle öffentlichen statischen Properties des Typs Color.
            foreach (var colorProp in typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                // Ruft den Wert der aktuellen Property ab.
                var color = colorProp.GetValue(typeof(Color));
                // Prüft, ob der abgerufene Wert vom Typ Color ist.
                if (color is Color value)
                {
                    // Fügt den Namen der Property und den zugehörigen Color-Wert in das Dictionary ein.
                    dic.Add(colorProp.Name, value);
                }
            }
            // Weist dem statischen ReadOnlyDictionary die Werte des temporären Dictionaries zu.
            Colors = new ReadOnlyDictionary<string, Color>(dic);
        }

        // Eine Hilfsmethode, die überprüft, ob der übergebene String "rainbow" entspricht.
        // Diese Methode kann genutzt werden, um zu bestimmen, ob der Regenbogenmodus aktiviert werden soll.
        public static bool IsRainbow(string name)
        {
            return name == "rainbow";
        }
    }
}

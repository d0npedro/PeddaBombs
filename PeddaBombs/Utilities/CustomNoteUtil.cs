using IPA.Loader;                           // Ermöglicht den Zugriff auf den PluginManager, um installierte Plugins abzufragen.
using System;                                 // Basis-Namespace für grundlegende Funktionen (wie Type).
using System.Reflection;                      // Ermöglicht Reflection, um Typinformationen und Properties dynamisch abzurufen.
using UnityEngine;                            // Zugriff auf Unity-spezifische Klassen, wie GameObject und Color.
using Zenject;                                // Dependency Injection Framework, um Abhängigkeiten zu injizieren.

namespace PeddaBombs.Utilities
{
    // Diese Klasse stellt Hilfsfunktionen für den Umgang mit "Custom Notes" bereit.
    // Sie nutzt Reflection, um dynamisch auf Eigenschaften des "Custom Notes"-Plugins zuzugreifen.
    public class CustomNoteUtil
    {
        // Gibt an, ob das "Custom Notes"-Plugin installiert ist.
        public bool IsInstallCustomNote { get; private set; }

        // Ein Objekt, das vom NoteAssetLoader des "Custom Notes"-Plugins stammt.
        // Wird zur Laufzeit via Zenject aufgelöst.
        private readonly object _loader;

        // Liest den aktuell ausgewählten Noten-Index aus dem Loader.
        // Falls _loader null ist, wird -1 zurückgegeben.
        public int SelectedNoteIndex => this._loader == null
            ? -1
            : (int)this._loader.GetType().GetProperty("SelectedNote").GetValue(this._loader);

        // Gibt an, ob Custom Notes aktuell aktiviert ist.
        // Ruft über Reflection die Property "Enabled" des Loaders ab, wenn vorhanden.
        public bool Enabled => this._loader != null
            && (bool)this._loader.GetType().GetProperty("Enabled").GetValue(this._loader);

        // Statische Variable, die den Type des CustomNoteControllers speichert.
        // Dieser Type wird dynamisch anhand des Namens des Controllers abgerufen.
        private static readonly Type s_customNoteController;

        // Speichert die PropertyInfo für die "Color"-Eigenschaft des CustomNoteControllers.
        // Damit kann später der Farbwert via Reflection gesetzt werden.
        private static readonly PropertyInfo s_customNoteControllerColorInfo;

        // Statischer Konstruktor: Wird einmal beim Laden der Klasse ausgeführt.
        // Hier werden der CustomNoteController-Typ und die zugehörige Color-Property ermittelt.
        static CustomNoteUtil()
        {
            // Versucht, den Type "CustomNotes.Managers.CustomNoteController" aus dem "CustomNotes"-Assembly zu laden.
            s_customNoteController = Type.GetType("CustomNotes.Managers.CustomNoteController, CustomNotes");
            // Ermittelt, falls vorhanden, die öffentliche Instanz-Property "Color" aus dem CustomNoteController.
            s_customNoteControllerColorInfo = s_customNoteController?.GetProperty("Color", BindingFlags.Instance | BindingFlags.Public);
        }

        // Konstruktor, der von Zenject injiziert wird.
        // Er versucht, den NoteAssetLoader des "Custom Notes"-Plugins über den DI-Container aufzulösen.
        [Inject]
        public CustomNoteUtil(DiContainer container)
        {
            // Überprüft, ob das Plugin "Custom Notes" installiert ist.
            this.IsInstallCustomNote = PluginManager.GetPluginFromId("Custom Notes") != null;

            // Versucht, den Typ des NoteAssetLoader aus dem "Custom Notes"-Plugin zu laden.
            var loaderType = Type.GetType("CustomNotes.Managers.NoteAssetLoader, CustomNotes");
            // Falls der Typ gefunden wurde, versucht der DI-Container, eine Instanz dieses Typs bereitzustellen.
            // Andernfalls wird _loader auf null gesetzt.
            this._loader = loaderType == null ? null : container.TryResolve(loaderType);
        }

        // Diese Methode versucht, aus einem GameObject die Komponente zu erhalten, die für die Farbvisualisierung von Noten zuständig ist.
        // Sie gibt true zurück, wenn eine entsprechende Komponente gefunden wurde.
        public static bool TryGetColorNoteVisuals(GameObject gameObject, out ColorNoteVisuals colorNoteVisuals)
        {
            // Sucht nach einer Komponente vom Typ ColorNoteVisuals in den Kindobjekten.
            colorNoteVisuals = gameObject.GetComponentInChildren<ColorNoteVisuals>();

            // Falls keine Komponente gefunden wurde, versucht es mit einer alternativen, benutzerdefinierten Komponente.
            if (colorNoteVisuals == null)
            {
                // Lädt den Typ "CustomNotes.Overrides.CustomNoteColorNoteVisuals" aus dem "CustomNotes"-Plugin.
                var customColorType = Type.GetType("CustomNotes.Overrides.CustomNoteColorNoteVisuals, CustomNotes");
                // Sucht nach einer Komponente dieses Typs in den Kindobjekten und castet sie zu ColorNoteVisuals.
                colorNoteVisuals = (ColorNoteVisuals)gameObject.GetComponentInChildren(customColorType);
            }
            // Gibt zurück, ob eine Komponente gefunden wurde.
            return colorNoteVisuals != null;
        }

        // Diese Methode versucht, einen GameNoteController aus einem GameObject zu erhalten.
        // Gibt true zurück, wenn eine solche Komponente gefunden wurde.
        public static bool TryGetGameNoteController(GameObject gameObject, out GameNoteController component)
        {
            // Sucht nach einer Komponente vom Typ GameNoteController in den Kindobjekten.
            component = gameObject.GetComponentInChildren<GameNoteController>();
            // Gibt zurück, ob die Komponente vorhanden ist.
            return component != null;
        }

        // Diese Methode setzt die Farbe einer Note, die von "Custom Notes" gesteuert wird.
        // Sie nimmt ein GameObject, das den Note-Controller repräsentiert, und einen Color-Wert entgegen.
        public void SetColor(GameObject noteControllerGO, in Color color)
        {
            // Wenn Custom Notes nicht installiert ist, wird die Methode abgebrochen.
            if (!this.IsInstallCustomNote)
            {
                return;
            }
            // Falls der CustomNoteController oder dessen Color-Property nicht verfügbar sind, wird abgebrochen.
            if (s_customNoteController == null || s_customNoteControllerColorInfo == null)
            {
                return;
            }
            // Versucht, die Komponente vom Typ des CustomNoteControllers aus dem angegebenen GameObject zu erhalten.
            var instance = noteControllerGO.GetComponent(s_customNoteController);
            // Wenn keine entsprechende Komponente gefunden wurde, wird die Methode abgebrochen.
            if (instance == null)
            {
                return;
            }
            // Setzt über Reflection die "Color"-Property der gefundenen Komponente auf den übergebenen Farbwert.
            s_customNoteControllerColorInfo.SetValue(instance, color);
        }
    }
}

namespace PeddaBombs.Statics
{
    // Diese Klasse definiert feste, unveränderliche Schlüssel (Commands), die im Chat verwendet werden,
    // um bestimmte Funktionen im Plugin auszulösen.
    public class CommandKey
    {
        // Standardbefehl, der eine Bombenaktion auslöst.
        public const string BOMB = "!bomb";

        // Befehl, der zum Setzen der Farbe der Noten verwendet wird.
        public const string NOTE_COLOR = "!setnotecolor";

        // Befehl, der zum Ändern der Farbe der Sabers (Schwerter) dient.
        public const string SABER_COLOR = "!setsabercolor";

        // Befehl, der zum Ändern der Lichtfarbe genutzt wird.
        public const string LIGHT_COLOR = "!setlightcolor";

        // Befehl, der zum Ändern der Farbe der Wände verwendet wird.
        public const string WALL_COLOR = "!setwallcolor";
    }
}

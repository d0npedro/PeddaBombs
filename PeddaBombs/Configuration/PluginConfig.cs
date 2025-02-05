using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PeddaBombs.Configuration {
    internal class PluginConfig {
        public static PluginConfig Instance { get; set; }

        public virtual bool IsBombEnable { get; set; } = true;
        public virtual float TextViewSec { get; set; } = 1f;
        public virtual float MissTextViewSec { get; set; } = 0.7f;
        public virtual bool IsSaberColorEnable { get; set; } = true;
        public virtual bool IsWallColorEnable { get; set; } = true;
        public virtual bool IsNoteColorEnable { get; set; } = true;
        public virtual bool IsPratformColorEnable { get; set; } = true;
        public virtual int NameObjectLayer { get; set; } = 0;
        public virtual bool ReloadeIfMissCut { get; set; } = true;

        public virtual void OnReload() {}

        public virtual void Changed() {}

        public virtual void CopyFrom(PluginConfig other) {}
    }
}
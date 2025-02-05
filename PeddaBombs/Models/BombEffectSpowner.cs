using PeddaBombs.Configuration;
using UnityEngine;
using Zenject;

namespace PeddaBombs.Models {
    public class BombEffectSpowner : MonoBehaviour, IFlyingObjectEffectDidFinishEvent {
        public void Awake() {
            this._duaring = PluginConfig.Instance.TextViewSec;
            this._missDuaring = PluginConfig.Instance.MissTextViewSec;
            this._beatmapObjectManager.noteWasCutEvent += this.OnNoteWasCutEvent;
            this._beatmapObjectManager.noteWasMissedEvent += this.OnNoteWasMissedEvent;
        }

        public void OnDestroy() {
            this._beatmapObjectManager.noteWasCutEvent -= this.OnNoteWasCutEvent;
            this._beatmapObjectManager.noteWasMissedEvent -= this.OnNoteWasMissedEvent;
        }

        public void HandleFlyingObjectEffectDidFinish(FlyingObjectEffect flyingObjectEffect) {
            flyingObjectEffect.didFinishEvent.Remove(this);
            this._flyingBombNameEffectPool.Despawn(flyingObjectEffect as FlyingBombNameEffect);
        }

        private void OnNoteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo) {
            var dummyBomb = noteController.gameObject.GetComponent<DummyBomb>();
            if (dummyBomb == null) {
                return;
            }
            if (string.IsNullOrEmpty(dummyBomb.Text)) {
                return;
            }
            var effect = this._flyingBombNameEffectPool.Spawn();
            effect.transform.localPosition = noteCutInfo.cutPoint;
            effect.didFinishEvent.Add(this);
            var targetpos = noteController.worldRotation * new Vector3(0, 1.7f, 10f);
            effect.InitAndPresent(dummyBomb.Text, this._duaring, targetpos, noteController.worldRotation, Color.white, 10, false);
            dummyBomb.Text = "";
        }
        private void OnNoteWasMissedEvent(NoteController noteController) {
            var dummyBomb = noteController.gameObject.GetComponent<DummyBomb>();
            if (dummyBomb == null) {
                return;
            }
            if (string.IsNullOrEmpty(dummyBomb.Text)) {
                return;
            }
            if (PluginConfig.Instance.ReloadeIfMissCut) {
                DummyBomb.Senders.Enqueue(dummyBomb.Text);
                dummyBomb.Text = "";
            } else {
                var effect = this._flyingBombNameEffectPool.Spawn();
                effect.transform.localPosition = Vector3.zero;
                effect.didFinishEvent.Add(this);
                var targetpos = noteController.worldRotation * new Vector3(0, 1.7f, 10f);
                effect.InitAndPresent(dummyBomb.Text, this._missDuaring, targetpos, noteController.worldRotation, Color.red, 10, false);
                dummyBomb.Text = "";
            }
        }

        private BeatmapObjectManager _beatmapObjectManager;
        private MemoryPoolContainer<FlyingBombNameEffect> _flyingBombNameEffectPool;
        private float _duaring = 1f;
        private float _missDuaring = 0.7f;

        [Inject]
        public void Constractor(BeatmapObjectManager manager, FlyingBombNameEffect.Pool nameEffect) {
            this._beatmapObjectManager = manager;
            this._flyingBombNameEffectPool = new MemoryPoolContainer<FlyingBombNameEffect>(nameEffect);
        }
    }
}
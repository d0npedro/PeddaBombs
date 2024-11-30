using StreamPartyCommand.Utilities;
using System;
using UnityEngine;
using Zenject;

namespace StreamPartyCommand.Models
{
    public class DummyBombExprosionEffect : MonoBehaviour, INoteControllerNoteWasCutEvent
    {
        public void Awake()
        {
            if (CustomNoteUtil.TryGetGameNoteController(this.gameObject, out this._gameNoteController)) {
                this._gameNoteController.noteWasCutEvent.Add(this);
            }
            this._dummyBomb = this.gameObject.GetComponent<DummyBomb>();
            if (this._particleSystem != null) {
                Destroy(this._particleSystem);
            }
            this._particleSystem = Instantiate(this._particleAssetLoader.Particle);
            this._particleSystem.transform.SetParent(null, false);
            this._particleSystem.Stop();
        }
        public void OnDestroy()
        {
            if (this._particleSystem != null) {
                Destroy(this._particleSystem);
                this._particleSystem = null;
            }
            this._gameNoteController.noteWasCutEvent.Remove(this);
        }

        public virtual void SpawnExplosion(Vector3 pos)
        {
            try {
                this._particleSystem.transform.position = pos;
                this._particleSystem.Emit(50000);
            }
            catch (Exception e) {
                Plugin.Log.Error(e);
            }
        }

        public void HandleNoteControllerNoteWasCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK || !this._dummyBomb.EnableBombEffect) {
                return;
            }
            this.SpawnExplosion(noteCutInfo.cutPoint);
        }

        private ParticleSystem _particleSystem;
        private GameNoteController _gameNoteController;
        private DummyBomb _dummyBomb;

        [Inject]
        public void Constractor(ParticleAssetLoader loader)
        {
            this._particleAssetLoader = loader;
        }
        private ParticleAssetLoader _particleAssetLoader;
    }
}

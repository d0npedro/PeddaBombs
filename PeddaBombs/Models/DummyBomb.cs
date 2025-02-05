using IPA.Utilities;
using PeddaBombs.Utilities;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PeddaBombs.Models {
    public class DummyBomb : MonoBehaviour, INoteControllerDidInitEvent, INoteControllerNoteDidStartJumpEvent {

        public GameNoteController Controller { get; private set; }
        public string Text { get; set; }
        public static ConcurrentQueue<string> Senders { get; } = new ConcurrentQueue<string>();
        public bool EnableBombEffect { get; set; } = false;

        protected void Awake() {
            if (CustomNoteUtil.TryGetGameNoteController(this.gameObject, out var component)) {
                this.Controller = component;
                this.Controller.didInitEvent.Add(this);
                this.Controller.noteDidStartJumpEvent.Add(this);
            }
            this._noteCube = this.gameObject.transform.Find("NoteCube");
            if (CustomNoteUtil.TryGetColorNoteVisuals(this.gameObject, out var visuals)) {
                this._colorManager = visuals.GetField<ColorManager, ColorNoteVisuals>("_colorManager");
            }
            var disappearingArrowController = this.gameObject.GetComponentInParent<DisappearingArrowController>();
            this._noteMesh = disappearingArrowController.GetField<MeshRenderer, DisappearingArrowControllerBase<GameNoteController>>("_cubeMeshRenderer").gameObject.GetComponentsInChildren<MeshRenderer>(true).FirstOrDefault(x => x.name == "NoteCube");
        }
        protected void OnDestroy() {
            if (this.Controller != null) {
                this.Controller.didInitEvent.Remove(this);
                this.Controller.noteDidStartJumpEvent.Remove(this);
            }
            if (this._bombMesh != null) {
                Destroy(this._bombMesh);
            }
        }

        public void HandleNoteControllerDidInit(NoteControllerBase noteController) {
            if (this.Controller.noteVisualModifierType == NoteVisualModifierType.Ghost) {
                return;
            }
            this.EnableBombEffect = false;
            if (noteController is GameNoteController gameNoteController
                && gameNoteController.gameplayType == NoteData.GameplayType.Normal
                && this._bombMesh == null
                && BombMeshGetter.BombMesh != null) {
                this._bombMesh = Instantiate(BombMeshGetter.BombMesh);
                this._bombMesh.gameObject.transform.SetParent(this._noteCube, false);
            }
        }
        public void HandleNoteControllerNoteDidStartJump(NoteController noteController) {
            if (this.Controller.noteVisualModifierType == NoteVisualModifierType.Ghost) {
                return;
            }
            if (noteController is GameNoteController gameNoteController && gameNoteController.gameplayType == NoteData.GameplayType.Normal) {
                var color = this._colorManager.ColorForType(noteController.noteData.colorType);
                this.SetActiveBomb(Senders.TryDequeue(out var sender), in color, this._isCustomNote);
                this.Text = sender;
                this.EnableBombEffect = !string.IsNullOrEmpty(sender);
            }
        }

        private void SetActiveBomb(bool active, in Color noteColor, bool isInstallCustomNote = false) {
            if (!isInstallCustomNote && this._noteMesh != null) {
                this._noteMesh.forceRenderingOff = active;
            }
            if (this._bombMesh != null) {
                this._bombMesh.enabled = active;
                this._bombMesh.material.SetColor(s_bombColorId, noteColor);
            }
        }

        private Transform _noteCube;
        protected static readonly int s_bombColorId = Shader.PropertyToID("_SimpleColor");
        private MeshRenderer _bombMesh;
        private MeshRenderer _noteMesh;
        private ColorManager _colorManager;
        private int _selectedNoteIndex;
        private bool _isCustomNote;
        private CustomNoteUtil _customNoteUtil;

        [Inject]
        public void Init(CustomNoteUtil customNoteUtil) {
            this._customNoteUtil = customNoteUtil;
            this._selectedNoteIndex = this._customNoteUtil.SelectedNoteIndex;
            this._isCustomNote = this._customNoteUtil.IsInstallCustomNote && this._customNoteUtil.Enabled && 1 <= this._selectedNoteIndex;
        }

    }
    public delegate void NoteWasCutEventHandler(GameNoteController controller, in NoteCutInfo noteCutInfo);
}

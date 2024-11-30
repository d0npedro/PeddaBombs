using StreamPartyCommand.CommandControllers;
using StreamPartyCommand.Utilities;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace StreamPartyCommand.Models
{
    public class NoteRaibowColorController : MonoBehaviour, INoteControllerDidInitEvent, INoteControllerNoteDidFinishJumpEvent
    {
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // パブリックメソッド
        public void Awake()
        {
            _ = CustomNoteUtil.TryGetColorNoteVisuals(this.gameObject, out this._colorNoteVisuals);
            this._noteController = this.gameObject.GetComponent<NoteController>();
            this._noteController.didInitEvent.Add(this);
            this._noteController.noteDidFinishJumpEvent.Add(this);
        }
        public void Update()
        {
            if (!this._enable) {
                return;
            }
            if (this._beatmapUtil.IsChroma) {
                return;
            }
            if (this._noteController == null) {
                return;
            }
            switch (this._noteController.noteData.colorType) {
                case ColorType.ColorA: {
                        if (!this._noteColorController.RainbowLeft) {
                            return;
                        }
                        var color = this._noteColorController.Colors[this._noteColorController.LeftColorIndex];
                        if (this._customNoteUtil.Enabled && 0 <= this._customNoteUtil.SelectedNoteIndex) {
                            this._customNoteUtil.SetColor(this.gameObject, color);
                        }
                        else {
                            foreach (var propertyBlockController in (MaterialPropertyBlockController[])s_materialPropertyBlockControllerArray.GetValue(this._colorNoteVisuals)) {
                                propertyBlockController.materialPropertyBlock.SetColor(s_colorID, color);
                                propertyBlockController.ApplyChanges();
                            }
                        }
                    }
                    break;
                case ColorType.ColorB: {
                        if (!this._noteColorController.RainbowRight) {
                            return;
                        }
                        var color = this._noteColorController.Colors[this._noteColorController.RightColorIndex];
                        if (this._customNoteUtil.Enabled && 0 <= this._customNoteUtil.SelectedNoteIndex) {
                            this._customNoteUtil.SetColor(this.gameObject, color);
                        }
                        else {
                            foreach (var propertyBlockController in (MaterialPropertyBlockController[])s_materialPropertyBlockControllerArray.GetValue(this._colorNoteVisuals)) {
                                propertyBlockController.materialPropertyBlock.SetColor(s_colorID, color);
                                propertyBlockController.ApplyChanges();
                            }
                        }
                    }
                    break;
                case ColorType.None:
                default:
                    return;
            }
        }

        public void OnDestroy()
        {
            if (this._noteController != null) {
                this._noteController.didInitEvent.Remove(this);
                this._noteController.noteDidFinishJumpEvent.Remove(this);
            }
        }
        public void HandleNoteControllerDidInit(NoteControllerBase noteController)
        {
            this._enable = true;
        }

        public void HandleNoteControllerNoteDidFinishJump(NoteController noteController)
        {
            this._enable = false;
        }
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // メンバ変数
        private ColorNoteVisuals _colorNoteVisuals;
        private NoteColorController _noteColorController;
        private NoteController _noteController;
        private BeatmapUtil _beatmapUtil;
        private CustomNoteUtil _customNoteUtil;
        private static readonly FieldInfo s_materialPropertyBlockControllerArray = typeof(ColorNoteVisuals).GetField("_materialPropertyBlockControllers", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly int s_colorID = Shader.PropertyToID("_Color");
        private bool _enable;
        #endregion
        //ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*ﾟ+｡｡+ﾟ*｡+ﾟ ﾟ+｡*
        #region // 構築・破棄
        [Inject]
        public void Constract(NoteColorController noteColorController, BeatmapUtil util, CustomNoteUtil customNoteUtil)
        {
            this._noteColorController = noteColorController;
            this._beatmapUtil = util;
            this._customNoteUtil = customNoteUtil;
        }
        #endregion
    }
}

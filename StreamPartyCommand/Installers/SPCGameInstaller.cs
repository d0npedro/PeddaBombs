using SiraUtil.Extras;
using SiraUtil.Objects.Beatmap;
using StreamPartyCommand.CommandControllers;
using StreamPartyCommand.Models;
using TMPro;
using UnityEngine;
using Zenject;

namespace StreamPartyCommand.Installers
{
    public class SPCGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            _ = this.Container.BindInterfacesAndSelfTo<StreamPartyCommandController>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
            _ = this.Container.BindMemoryPool<FlyingBombNameEffect, FlyingBombNameEffect.Pool>().WithInitialSize(10).FromComponentInNewPrefab(this._flyingBombNameEffect).AsCached();
            _ = this.Container.BindInterfacesAndSelfTo<BombEffectSpowner>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<BeatmapUtil>().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<RainbowUtil>().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<BombMeshGetter>().AsSingle().NonLazy();
            _ = this.Container.BindInterfacesAndSelfTo<BombCommandController>().FromNewComponentOnNewGameObject().AsSingle();
            _ = this.Container.BindInterfacesAndSelfTo<WallColorController>().FromNewComponentOnNewGameObject().AsSingle();
            _ = this.Container.BindInterfacesAndSelfTo<LightColorController>().FromNewComponentOnNewGameObject().AsSingle();
            _ = this.Container.BindInterfacesAndSelfTo<NoteColorController>().FromNewComponentOnNewGameObject().AsSingle();
            _ = this.Container.BindInterfacesAndSelfTo<SaberColorController>().FromNewComponentOnNewGameObject().AsSingle();

            this.Container.RegisterRedecorator(new BasicNoteRegistration(this.RedecoreteNoteController));
            this.Container.RegisterRedecorator(new BurstSliderHeadNoteRegistration(this.RedecoreteSliderHeadNoteController));
            this.Container.RegisterRedecorator(new BurstSliderNoteRegistration(this.RedecoreteSliderNoteController));
        }

        private GameNoteController RedecoreteNoteController(GameNoteController noteController)
        {
            _ = noteController.gameObject.AddComponent<DummyBomb>();
            _ = noteController.gameObject.AddComponent<DummyBombExprosionEffect>();
            _ = noteController.gameObject.AddComponent<Models.NoteRaibowColorController>();
            return noteController;
        }

        private GameNoteController RedecoreteSliderHeadNoteController(GameNoteController noteController)
        {
            _ = noteController.gameObject.AddComponent<Models.NoteRaibowColorController>();
            return noteController;
        }

        private BurstSliderGameNoteController RedecoreteSliderNoteController(BurstSliderGameNoteController noteController)
        {
            _ = noteController.gameObject.AddComponent<Models.NoteRaibowColorController>();
            return noteController;
        }

        private readonly FlyingBombNameEffect _flyingBombNameEffect;

        public SPCGameInstaller()
        {
            this._flyingBombNameEffect = new GameObject("FlyingBombNameEffect", typeof(TextMeshPro), typeof(FlyingBombNameEffect)).GetComponent<FlyingBombNameEffect>();
        }

        ~SPCGameInstaller()
        {
            if (this._flyingBombNameEffect != null) {
                GameObject.Destroy(this._flyingBombNameEffect.gameObject);
            }
        }
    }
}

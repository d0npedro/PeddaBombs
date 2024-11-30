using System.Reflection;
using UnityEngine;
using Zenject;

namespace StreamPartyCommand.Models
{
    public class RainbowUtil
    {
        private static readonly int s_addColorID = Shader.PropertyToID("_AddColor");
        private static readonly int s_tintColorID = Shader.PropertyToID("_TintColor");

        private readonly IAudioTimeSource _timeSource;
        private readonly BasicBeatmapObjectManager _beatmapObjectManager;

        private static readonly FieldInfo s_stretchableObstacle = typeof(ObstacleController).GetField("_stretchableObstacle", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo s_obstacleFrame = typeof(StretchableObstacle).GetField("_obstacleFrame", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo s_obstacleFakeGlow = typeof(StretchableObstacle).GetField("_obstacleFakeGlow", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo s_materialPropertyBlockControllers = typeof(StretchableObstacle).GetField("_materialPropertyBlockControllers", BindingFlags.NonPublic | BindingFlags.Instance);

        [Inject]
        public RainbowUtil(IAudioTimeSource timeSource, BasicBeatmapObjectManager beatmapObjectManager)
        {
            this._timeSource = timeSource;
            this._beatmapObjectManager = beatmapObjectManager;
        }

        public void SetWallRainbowColor(Color color)
        {
            this.UpdateWallRainbowColor(color);
        }

        private void UpdateWallRainbowColor(Color color)
        {
            foreach (var wall in this._beatmapObjectManager.activeObstacleControllers) {
                var stretchable = (StretchableObstacle)s_stretchableObstacle.GetValue(wall);
                var frame = (ParametricBoxFrameController)s_obstacleFrame.GetValue(stretchable);
                var fakeGlow = (ParametricBoxFakeGlowController)s_obstacleFakeGlow.GetValue(stretchable);
                frame.color = color;
                frame.Refresh();
                if (fakeGlow != null) {
                    fakeGlow.color = color;
                    fakeGlow.Refresh();
                }

                var value = color * 0.1f;
                value.a = 0f;
                var propertyBlockControllers = (MaterialPropertyBlockController[])s_materialPropertyBlockControllers.GetValue(stretchable);
                foreach (var propertyBlockController in propertyBlockControllers) {
                    propertyBlockController.materialPropertyBlock.SetColor(s_addColorID, value);
                    propertyBlockController.materialPropertyBlock.SetColor(s_tintColorID, Color.Lerp(color, Color.white, 0.75f));
                    propertyBlockController.ApplyChanges();
                }
            }
        }

        public Color WallRainbowColor()
        {
            var hue = ((this._timeSource.songTime / 5f) + 0.25f) % 1f;
            return Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}

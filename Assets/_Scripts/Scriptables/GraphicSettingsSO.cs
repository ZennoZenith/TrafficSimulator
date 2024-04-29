using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(fileName = "DefaultGraphicSettings", menuName = "ScriptableObjects/Settings/GraphicSettings", order = 2)]
    public class GraphicSettingsSO : UnityEngine.ScriptableObject {
        public bool showFpsCount;
        public int defaultTargetFrameRate;

        [Tooltip("Interval to update FPS count in seconds")]
        public float updateInterval = 0.5f;


        #region DEBUG   
        // [Header("Debug")]

        #endregion

#if UNITY_EDITOR

#endif

    }
}


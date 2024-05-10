using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/SignalSettings", fileName = "DefaultSignalSettings", order = 1)]
    public class SignalSettingsSO : UnityEngine.ScriptableObject {

        [Header("Trafic Signal Timings")]
        public float defautGreenTime;
        public float defautYellowTime;
        public Material greenMaterial;
        public Material redMaterial;
        public float minGreenLightTime;
        public float maxGreenLightTime;
        //public float defautRedTime;
    }
}
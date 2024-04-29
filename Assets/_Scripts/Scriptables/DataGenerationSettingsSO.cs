using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(fileName = "DefaultDataGenerationSettings", menuName = "ScriptableObjects/Settings/DataGenerationSettings", order = 4)]
    public class DataGenerationSettingsSO : UnityEngine.ScriptableObject {

        [Tooltip("Time after which writing to file happens in sec")]
        public float bufferTime;
        public bool disableDataGeneration;

        #region DEBUG   
        // [Header("Debug")]

        #endregion

#if UNITY_EDITOR

#endif

    }
}


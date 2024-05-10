using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(fileName = "DefaultDataGenerationSettings", menuName = "ScriptableObjects/Settings/DataGenerationSettings", order = 1)]
    public class DataGenerationSettingsSO : UnityEngine.ScriptableObject {

        [Tooltip("Time after which writing to file happens in sec")]
        public bool disableDataGeneration;

        public int writeIntersectionThroughputPerNSec = 60; // default is 60 seconds

    }
}


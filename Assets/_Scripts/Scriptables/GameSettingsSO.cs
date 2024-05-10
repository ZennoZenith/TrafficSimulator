using UnityEngine;


namespace Simulator.ScriptableObject {
    [CreateAssetMenu(fileName = "DefaultGameSettings", menuName = "ScriptableObjects/Settings/GameSettings", order = 1)]
    public class GameSettingsSO : UnityEngine.ScriptableObject {
        public float defalultTimeScale = 1;
        [Tooltip("Number of next road blocks to consider for calculating traffic light collision")]
        public int numberOfHeuristicPoints;

        [Header("Spawner")]
        public int retryAfterSpawnTime;

        [Header("ML")]
        public float warmupTime;
        //public float cooldownTime;




    }
}
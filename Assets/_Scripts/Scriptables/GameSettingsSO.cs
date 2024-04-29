using UnityEngine;


namespace Simulator.ScriptableObject {
    [CreateAssetMenu(fileName = "DefaultGameSettings", menuName = "ScriptableObjects/Settings/GameSettings", order = 2)]
    public class GameSettingsSO : UnityEngine.ScriptableObject {
        public float defalultTimeScale = 1;

        [Header("Spawner")]
        public int retryAfterSpawnTime;

        [Header("ML")]
        public bool usML;


    }
}
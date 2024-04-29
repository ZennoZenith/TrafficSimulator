using UnityEngine;

namespace Simulator.Manager {
    public class VehicleSpawnerManager : MonoBehaviour {


        [field: SerializeField] public Spawner[] Spawners { get; private set; }
        [field: SerializeField] public DeSpawner[] Despawners { get; private set; }

    }

}

using Simulator.Road;
using System.Collections;
using UnityEngine;

namespace Simulator.Manager {
    public class VehicleSpawnerManager : MonoBehaviour {

        [field: SerializeField] public GraphGenerator GraphGenerator { get; private set; }
        [field: SerializeField] public Spawner[] Spawners { get; private set; }
        [field: SerializeField] public DeSpawner[] Despawners { get; private set; }

        #region Private Methods
        IEnumerator Start() {
            while (true) {
                if (GraphGenerator.IsInitialized) {
                    foreach (var item in Spawners) {
                        item.gameObject.SetActive(true);
                    }
                    break;
                }
                Debug.Log("GraphGenerator not yet initialized. Retrying in 1 second");
                yield return new WaitForSeconds(1f);
            }
        }
        #endregion        
    }
}

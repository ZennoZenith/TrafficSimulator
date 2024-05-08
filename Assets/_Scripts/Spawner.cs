using Simulator.AI;
using Simulator.Graph;
using Simulator.Manager;
using Simulator.Road;
using Simulator.ScriptableObject;
using Simulator.Vehicle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator {
    public class Spawner : MonoBehaviour {
        #region Public Fields
        [field: SerializeField] public SpawnerDataSO SpawnerData { get; private set; }
        [field: SerializeField] public RoadConnector RoadConnector { get; private set; }
        #endregion


        [SerializeField] private VehicleSpawnerManager vehicleSpawnerManager;
        [SerializeField] private float noSpawnRadius;

        private Coroutine coroutine;
        #region Unity Methods
        private void OnEnable() {
            vehicleSpawnerManager.GraphGenerator.OnInitilized += OnGraphInitilized;
            vehicleSpawnerManager.GraphGenerator.OnDeinitilized += OnGraphDeinitilized;
        }

        private void OnDisable() {
            vehicleSpawnerManager.GraphGenerator.OnInitilized -= OnGraphInitilized;
            vehicleSpawnerManager.GraphGenerator.OnDeinitilized -= OnGraphDeinitilized;
        }

        #endregion

        #region Private Methods
        private readonly Collider[] colliders = new Collider[1];
        private bool CanSpawn() {
            //if (lastSpawnedVehicle == null) {
            //    lastSpawnedVehicle = Spawn(spawnerInfo.roadSetup, vehicles, vehicleFrequencyList, vehicleFrequencySum, numberOfVehicles);
            //}
            //else if (Vector3.Distance(lastSpawnedVehicle.SpawnedAt, lastSpawnedVehicle.transform.position) > noSpawnRadius) {
            //    lastSpawnedVehicle = Spawn(spawnerInfo.roadSetup, vehicles, vehicleFrequencyList, vehicleFrequencySum, numberOfVehicles);
            //}
            if (Physics.OverlapSphereNonAlloc(transform.position, noSpawnRadius, colliders, LayerMask.GetMask("Vehicle")) != 0)
                return false;

            return true;

        }


        private void OnGraphInitilized() {
            coroutine = StartCoroutine(SpawnCorouting());
        }

        private void OnGraphDeinitilized() {
            if (coroutine != null) {
                StopCoroutine(coroutine);
            }
        }




        private IEnumerator SpawnCorouting() {
            //while (true) {
            //    if (vehicleSpawnerManager.GraphGenerator.IsInitialized)
            //        break;
            //    Debug.Log("GraphGenerator not yet initialized. Retrying in 1 second");
            //    yield return new WaitForSeconds(1f);
            //}


            SpawnerDataSO spawnerData = SpawnerData;

            float nextSpawnTime;
            List<int> vehicleFrequencyList = new();
            int vehicleFrequencySum = 0;
            VehicleFrequency[] vehicles = spawnerData.vehicles;
            int numberOfVehicles = spawnerData.vehicles.Length;

            for (int i = 0; i < numberOfVehicles; i++) {
                vehicleFrequencySum += spawnerData.vehicles[i].frequency;
                vehicleFrequencyList.Add(vehicleFrequencySum);
            }

            while (true) {
                nextSpawnTime = 60f / (spawnerData.spawnFrequency + Random.Range(-spawnerData.frequencyVariation, spawnerData.frequencyVariation));

                if (CanSpawn())
                    Spawn(RoadConnector.GraphNode, vehicles, vehicleFrequencyList, vehicleFrequencySum, numberOfVehicles);

                yield return new WaitForSeconds(nextSpawnTime);
            }
        }

        private VehicleDriverAI Spawn(Node fromNode, VehicleFrequency[] vehicles, List<int> vehicleFrequencyList, int vehicleFrequencySum, int numberOfVehicles = -1) {
            if (numberOfVehicles == -1) {
                numberOfVehicles = vehicles.Length;
            }

            //if (!fromRoad.roadSetup.IsInitialized)
            //    return null;

            int randIndex = Random.Range(0, vehicleFrequencySum);
            int spawnVehicleIndex = numberOfVehicles - 1;
            for (int i = 0; i < numberOfVehicles; i++) {
                if (randIndex < vehicleFrequencyList[i]) {
                    spawnVehicleIndex = i;
                    break;
                }
            }

            Node despawnNode = SelectRandomDespawnLocation(fromNode);
            if (despawnNode == null) {
                return null;
            }

            //GameObject vehicle = Instantiate(vehicles[spawnVehicleIndex].vehicle, fromRoad.transform.position, Quaternion.identity);
            GameObject vehicle = ObjectPoolManager.SpawnObject(vehicles[spawnVehicleIndex].vehicle, ObjectPoolManager.PoolType.GameObject);

            VehicleController vc = vehicle.GetComponent<VehicleController>();

            vc.Initialize();
            vc.VehicleDriverAI.Initialize(vehicleSpawnerManager.GraphGenerator, fromNode, despawnNode);

            return vc.VehicleDriverAI;

        }

        private Node SelectRandomDespawnLocation(Node from) {
            if (from.roadSetup.ReachableNodes.Count == 0) {
                return null;
            }

            int index = Random.Range(0, from.roadSetup.ReachableNodes.Count);
            return from.roadSetup.ReachableNodes[index];
        }

        #endregion



        #region DEBUG   
        // [Header("Debug")]

        #endregion


#if UNITY_EDITOR

#endif

    }
}


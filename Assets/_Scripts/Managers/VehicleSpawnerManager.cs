using Simulator.AI;
using Simulator.Road;
using Simulator.ScriptableObject;
using Simulator.Vehicle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator.Manager {
    [RequireComponent(typeof(GraphGenerator))]
    public class VehicleSpawnerManager : MonoBehaviour {
        [System.Serializable]
        public struct SpawnerInfo {
            public SpawnerDataSO spawnerData;
            public RoadSetup roadSetup;
        }

        [SerializeField] private GameSettingsSO gameSettings;
        [SerializeField] private float noSpawnRadius;
        [field: SerializeField] public SpawnerInfo[] Spawners { get; private set; }
        [field: SerializeField] public RoadSetup[] Despawners { get; private set; }

        private GraphGenerator graphGenerator;

        private void Awake() {
            graphGenerator = GetComponent<GraphGenerator>();
        }

        private void Start() {
            foreach (var spawnerInfo in Spawners) {
                StartCoroutine(SpawnCorouting(spawnerInfo));
            }

        }

        private IEnumerator SpawnCorouting(SpawnerInfo spawnerInfo) {
            SpawnerDataSO spawnerData = spawnerInfo.spawnerData;

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
                    Spawn(spawnerInfo.roadSetup, vehicles, vehicleFrequencyList, vehicleFrequencySum, numberOfVehicles);

                yield return new WaitForSeconds(nextSpawnTime);
            }
        }

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

        private VehicleDriverAI Spawn(RoadSetup fromRoad, VehicleFrequency[] vehicles, List<int> vehicleFrequencyList, int vehicleFrequencySum, int numberOfVehicles = -1) {
            if (numberOfVehicles == -1) {
                numberOfVehicles = vehicles.Length;
            }

            if (!fromRoad.Initialized)
                return null;

            int randIndex = Random.Range(0, vehicleFrequencySum);
            int spawnVehicleIndex = numberOfVehicles - 1;
            for (int i = 0; i < numberOfVehicles; i++) {
                if (randIndex < vehicleFrequencyList[i]) {
                    spawnVehicleIndex = i;
                    break;
                }
            }

            RoadSetup despawnNode = SelectRandomDespawnLocation(fromRoad);
            if (despawnNode == null) {
                return null;
            }

            //GameObject vehicle = Instantiate(vehicles[spawnVehicleIndex].vehicle, fromRoad.transform.position, Quaternion.identity);
            GameObject vehicle = ObjectPoolManager.SpawnObject(vehicles[spawnVehicleIndex].vehicle, ObjectPoolManager.PoolType.GameObject);

            VehicleController vc = vehicle.GetComponent<VehicleController>();

            vc.Initialize();
            vc.VehicleDriverAI.Initialize(graphGenerator, fromRoad, despawnNode);

            return vc.VehicleDriverAI;

        }

        private RoadSetup SelectRandomDespawnLocation(RoadSetup from) {
            if (from.ReachableNodes.Count == 0) {
                return null;
            }

            int index = Random.Range(0, from.ReachableNodes.Count);
            return from.ReachableNodes[index];
        }
    }

}

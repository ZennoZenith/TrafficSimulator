using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GraphGenerator))]
public class VehicleSpawnerManager : MonoBehaviour {
    [System.Serializable]
    public struct SpawnerInfo {
        public SpawnerDataScriptableObject spawnerData;
        public RoadSetup roadSetup;
    }

    [field: SerializeField] public SpawnerInfo[] Spawners { get; private set; }
    [field: SerializeField] public RoadSetup[] Despawners { get; private set; }
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    private GraphGenerator graphGenerator;

    private void Awake() {
        graphGenerator = GetComponent<GraphGenerator>();
    }

    private void Start() {
        //foreach (var spawnerInfo in Spawners) {
        //    StartCoroutine(SpawnCorouting(spawnerInfo));
        //}
        StartCoroutine(SpawnCorouting(Spawners[0]));


    }

    private IEnumerator SpawnCorouting(SpawnerInfo spawnerInfo) {
        SpawnerDataScriptableObject spawnerData = spawnerInfo.spawnerData;

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
            //print(nextSpawnTime);
            Spawn(spawnerInfo.roadSetup, vehicles, vehicleFrequencyList, vehicleFrequencySum, numberOfVehicles);
            yield return new WaitForSeconds(nextSpawnTime);
        }
    }
    private void Spawn(RoadSetup fromRoad, VehicleFrequency[] vehicles, List<int> vehicleFrequencyList, int vehicleFrequencySum, int numberOfVehicles = -1) {
        if (numberOfVehicles == -1) {
            numberOfVehicles = vehicles.Length;
        }

        if (!fromRoad.Initialized)
            return;

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
            return;
        }

        GameObject vehicle = Instantiate(vehicles[spawnVehicleIndex].vehicle, fromRoad.transform.position, Quaternion.identity);
        //vehicle.transform

        VehicleController vc = vehicle.GetComponent<VehicleController>();
        vc.Initialize();

        vc.VehicleDriverAI.Initialize(graphGenerator, fromRoad, despawnNode);
    }

    private RoadSetup SelectRandomDespawnLocation(RoadSetup from) {
        if (from.ReachableNodes.Count == 0) {
            return null;
        }

        int index = Random.Range(0, from.ReachableNodes.Count);
        return from.ReachableNodes[index];
    }

}

//private 

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Simulator.Manager {
    public class ObjectPoolManager : Singleton<ObjectPoolManager> {
        public static List<PooledObjectInfo> ObjectPools = new();

        private GameObject _objectPoolEmptyHolder;

        private static GameObject _particleSystemsEmpty;
        private static GameObject _gameObjectEmpty;

        public enum PoolType {
            ParticleSystem,
            GameObject,
            None
        }

        public static PoolType PoolingType;

        private void Awake() {
            SetupEmpties();
        }

        private void SetupEmpties() {
            _objectPoolEmptyHolder = new GameObject("Pooled Objects");

            _particleSystemsEmpty = new GameObject("Particle Effects");
            _particleSystemsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

            _gameObjectEmpty = new GameObject("GameObjects");
            _gameObjectEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
        }

        private static Vector3 V3ZERO = Vector3.zero;
        private static Quaternion Q_IDENTITY = Quaternion.identity;

        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform = null) {
            return SpawnObject(objectToSpawn, V3ZERO, Q_IDENTITY, parentTransform);
        }
        public static GameObject SpawnObject(GameObject objectToSpawn, PoolType poolType) {
            return SpawnObject(objectToSpawn, V3ZERO, Q_IDENTITY, poolType);
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, Transform parentTransform = null) {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupStirng == objectToSpawn.name);

            // If pool doesn't exist, create it
            if (pool == null) {
                pool = new PooledObjectInfo(objectToSpawn.name);
                ObjectPools.Add(pool);
            }

            if (spawnPosition == null) {
                spawnPosition = V3ZERO;
            }

            // Check if there is any inactive object in the pool
            GameObject spawnableObject = pool.InactiveObjects.FirstOrDefault();


            if (spawnableObject == null) {
                // There is no inactivate objects, create a new one
                spawnableObject = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
                spawnableObject.name = objectToSpawn.name;

                // assign parent
                if (parentTransform != null) {
                    spawnableObject.transform.SetParent(parentTransform);
                }
            }
            else {
                // If there is an inactive object, reactivate it
                spawnableObject.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
                pool.InactiveObjects.Remove(spawnableObject);
                spawnableObject.SetActive(true);
            }
            return spawnableObject;
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None) {
            // Find the parent of th empty object
            GameObject parentGameObject = SetParentObject(poolType);
            return SpawnObject(
              objectToSpawn,
              spawnPosition,
              spawnRotation,
              parentGameObject?.transform);
        }

        public static void ReturnObjectToPool(GameObject obj) {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupStirng == obj.name);

            if (pool == null) {
                Debug.LogWarning("Trying to release object that is not pooled: " + obj.name);
                Destroy(obj);
            }
            else {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }

        private static GameObject SetParentObject(PoolType poolType = PoolType.GameObject) {
            return poolType switch {
                PoolType.ParticleSystem => _particleSystemsEmpty,
                PoolType.GameObject => _gameObjectEmpty,
                PoolType.None => null,
                _ => null,
            };
        }

    }

    public class PooledObjectInfo {
        public string LookupStirng { get; private set; }

        public List<GameObject> InactiveObjects { get; private set; }

        public PooledObjectInfo(string name) {
            LookupStirng = name;
            InactiveObjects = new();
        }
    }
}
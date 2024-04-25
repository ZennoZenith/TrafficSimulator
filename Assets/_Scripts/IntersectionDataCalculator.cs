using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class IntersectionDataCalculator : MonoBehaviour {

    private int throughput = 0;

    private void Start() {
        StartCoroutine(Tick());
    }

    private IEnumerator Tick() {
        while (true) {
            yield return new WaitForSeconds(60f); //60 sec
            StoreData.WriteIntesectionThroughput(transform.name, throughput);
            throughput = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        throughput++;
    }


}

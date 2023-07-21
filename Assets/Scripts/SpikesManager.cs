using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesManager : MonoBehaviour
{
    public int spikeID;

    void Awake() {
        var random = new System.Random();
        spikeID = random.Next(0, 1000000);
    }

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        other.SendMessage("resetPosition");
    }
}

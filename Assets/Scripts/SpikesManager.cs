using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpikesManager : MonoBehaviour
{
    public int spikeID;

    void Awake() {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spikeID = gameManager.highestSpikeID;
        gameManager.SendMessage("increaseHighestSpikeID");
    }

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        other.SendMessage("resetPosition");
    }
}

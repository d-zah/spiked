using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        other.SendMessage("resetPosition");
    }
}

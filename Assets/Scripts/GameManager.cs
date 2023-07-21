using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int highestSpikeID;

    void Start() {
        highestSpikeID = 0;
    }

    void increaseHighestSpikeID() {
        highestSpikeID++;
    }
}

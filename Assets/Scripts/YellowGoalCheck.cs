using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class YellowGoalCheck : NetworkBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return; //if is a player, continue

        if(other.GetComponent<PlayerMovement>().team != 2) return; //if is on purple, continue

        resetRoundServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void resetRoundServerRpc(){
        resetRoundClientRpc();
    }

    [ClientRpc]
    void resetRoundClientRpc(){
        gameManager.resetRound();
    }
}

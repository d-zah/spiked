using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class GoalCheck : NetworkBehaviour
{
    public GameManager gameManager;
    public PlayerTeam playerTeam;

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return; //if is a player, continue

        if(other.GetComponent<PlayerMovement>().team != playerTeam) return; //if is wrong team, return

        resetRoundServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void resetRoundServerRpc(){
        resetRoundClientRpc();
    }

    [ClientRpc]
    void resetRoundClientRpc(){
        gameManager.resetRound(playerTeam);
    }
}

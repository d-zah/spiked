using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProjectileManager : NetworkBehaviour
{

    private void OnCollisionEnter(Collision other) {
        DestroyProjectileServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyProjectileServerRpc(){
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}

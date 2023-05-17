using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private GameObject battleShip;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "start0" || other.gameObject.name == "point1")
        {
            battleShip.gameObject.GetComponent<BattleShipMove>().MeshAndBoxTrigger();
        }
    }
}

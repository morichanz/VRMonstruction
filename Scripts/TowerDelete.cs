using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDelete : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    // Update is called once per frame
    void Update()
    {
        //確認用
        if (Input.GetKey(KeyCode.Space))
        {
            Destroy(gameObject);
            //爆発プレハブの生成
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerArm")
        {
            Destroy(gameObject);
            //爆発プレハブの生成
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}

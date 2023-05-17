using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDelete : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    // Update is called once per frame
    void Update()
    {
        //�m�F�p
        if (Input.GetKey(KeyCode.Space))
        {
            Destroy(gameObject);
            //�����v���n�u�̐���
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerArm")
        {
            Destroy(gameObject);
            //�����v���n�u�̐���
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Searching : MonoBehaviour
{
    [SerializeField] private GameObject battleShip;
    [SerializeField, Header("��͂̍��G�͈�")] private float distance = 400;

    //U�^�[�����͒e�𔭎˂��Ȃ��悤�ɐݒ�
    private bool shotFlg = false;

    void Update()
    {
        float dis = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, battleShip.transform.position);
        //Debug.Log(dis);
        //Debug.Log(shotFlg);
        if (dis < distance)
        {
            shotFlg = true;
        }
        else
        {
            shotFlg = false;
        }
    }

    public bool BattleShipShotFlg()
    {
        return shotFlg;
    }
}

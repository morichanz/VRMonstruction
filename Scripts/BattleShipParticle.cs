using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipParticle : MonoBehaviour
{
    void Start()
    {
        //2�b���DeleteExplosion���Ăяo��
        Invoke("DeleteExplosion", 2.0f);
    }

    //�����G�t�F�N�g�̍폜
    void DeleteExplosion()
    {
        Destroy(gameObject);
    }
}

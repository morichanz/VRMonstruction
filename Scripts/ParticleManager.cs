using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    //�ǋL�@�I�u�W�F�N�g�v�[���p�R���g���[���[�i�[�p�ϐ��錾
    //PoolManager objectPool;

    void Start()
    {
        //5�b���DeleteExplosion���Ăяo��
        Invoke("DeleteExplosion", 5.0f);
    }
    
    //�����G�t�F�N�g�̍폜
    void DeleteExplosion()
    {
        Destroy(gameObject);
    }
}

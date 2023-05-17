using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillSmokeManager : MonoBehaviour
{
    private BillEffectPoolManager objectPool;

    void Start()
    {
        //�ǋL�@�I�u�W�F�N�g�v�[�����擾
        objectPool = transform.parent.GetComponent<BillEffectPoolManager>();
        gameObject.SetActive(false);
    }

    private void OnParticleSystemStopped()
    {
        HideFromStage();
    }

    /**
     * �ݏo����
     */
    public void ShowInStage(Vector3 _pos)
    {
        //�݂��o����鏈�����L�q����
        //�ǋL�@position��n���ꂽ���W�ɐݒ�
        transform.position = _pos;
    }

    /**
     * �������
     */
    public void HideFromStage()
    {
        //�������鏈�����L�q����
        //�I�u�W�F�N�g�v�[����Collect�֐����Ăяo�����g�����
        objectPool.SmokeCollect(this.gameObject);
    }
}

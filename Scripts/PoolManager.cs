using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    //�e�̃v���n�u
    [SerializeField] private BulletMove bullet;
    //��͂̃v���n�u
    [SerializeField] private GameObject battleShip;

    //��������e��
    [SerializeField,Header("��������e��")] private int maxCount;
    //���������͂̐�
    [SerializeField, Header("���������͂̐�")] private int battleShipCnt;

    //���������e���i�[����Queue
    private Queue<BulletMove> bulletQueue;
    private Queue<GameObject> battleShipQueue;

    //���񐶐����̃|�W�V����
    Vector3 setPos = new Vector3(100, 100, 0);

    private void Awake()
    {
        //Queue�̏�����
        bulletQueue = new Queue<BulletMove>();
        battleShipQueue = new Queue<GameObject>();

        //�e�𐶐�
        for (int i = 0; i < maxCount; i++)
        {
            //����
            BulletMove tmpBullet = Instantiate(bullet, setPos, Quaternion.identity, transform);

            //Queue�ɒǉ�
            bulletQueue.Enqueue(tmpBullet);
        }
        //��͂𐶐�
        for (int i = 0; i < battleShipCnt; i++)
        {
            //����
            GameObject tmpBattleShip = Instantiate(battleShip, setPos, Quaternion.identity, transform);

            //Queue�ɒǉ�
            battleShipQueue.Enqueue(tmpBattleShip);
        }
    }

    /**
     * �e��݂��o������
     */
    public BulletMove Launch(Vector3 _pos,Vector3 _vec)
    {
        //Queue����Ȃ�null
        if (bulletQueue.Count <= 0) return null;
        //Queue����e������o��
        BulletMove tmpBullet = bulletQueue.Dequeue();
        //�e��\������
        tmpBullet.gameObject.SetActive(true);
        //�n���ꂽ���W�ɒe���ړ�����
        tmpBullet.ShowInStage(_pos, _vec);
        //�Ăяo�����ɓn��
        return tmpBullet;
    }

    /**
     * �e�̉������
     */
    public void Collect(BulletMove _bullet)
    {
        //�e�̃Q�[���I�u�W�F�N�g���\��
        _bullet.gameObject.SetActive(false);
        //Queue�Ɋi�[
        bulletQueue.Enqueue(_bullet);
    }

    /**
     * ��͂�݂��o������
     */
    public GameObject Spawn()
    {
        //Queue����Ȃ�null
        if (battleShipQueue.Count <= 0) return null;
        //Queue�����͂�����o��
        GameObject tmpBattleShip = battleShipQueue.Dequeue();
        //��͂�\������
        tmpBattleShip.gameObject.SetActive(true);
        //�Ăяo�����ɓn��
        return tmpBattleShip;
    }

    public int battleShipCounter()
    {
        return battleShipCnt;
    }

    public void BattleShipDelete()
    {
        this.gameObject.SetActive(false);
    }
}
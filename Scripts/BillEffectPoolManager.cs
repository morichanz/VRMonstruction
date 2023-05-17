using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BillEffectPoolManager : MonoBehaviour
{
    //�����̃v���n�u
    [SerializeField] private BillExplosionManager explosion;
    [SerializeField] private GameObject smoke;
    [SerializeField, Header("�������锚���̐�")] private int maxExplosionCount;
    [SerializeField, Header("�������鉌�̐�")] private int maxSmokeCount;

    //���������������i�[����Queue
    private Queue<BillExplosionManager> explosionQueue;

    //�������������i�[����Queue
    private Queue<GameObject> smokeQueue;

    //���񐶐����̃|�W�V����
    Vector3 setPos = new Vector3(100, 100, 0);

    private void Awake()
    {
        //Queue�̏�����
        explosionQueue = new Queue<BillExplosionManager>();
        smokeQueue = new Queue<GameObject>();

        //�����𐶐�
        for (int i = 0; i < maxExplosionCount; i++)
        {
            //����
            BillExplosionManager tmpExplosion = Instantiate(explosion, setPos, Quaternion.identity, transform);

            //Queue�ɒǉ�
            explosionQueue.Enqueue(tmpExplosion);
        }
        //���𐶐�
        for (int i = 0; i < maxSmokeCount; i++)
        {
            //����
            GameObject tmpSmoke = Instantiate(smoke, setPos, Quaternion.identity, transform);

            //Queue�ɒǉ�
            smokeQueue.Enqueue(tmpSmoke);
        }
    }

    /**
     * ������݂��o������
     */
    public BillExplosionManager Launch(Vector3 _pos)
    {
        //Queue����Ȃ�null
        if (explosionQueue.Count <= 0) return null;
        //Queue���甚��������o��
        BillExplosionManager tmpExplosion = explosionQueue.Dequeue();
        //������\������
        tmpExplosion.gameObject.SetActive(true);
        //�n���ꂽ���W�ɔ������ړ�����
        tmpExplosion.ShowInStage(_pos);
        //�Ăяo�����ɓn��
        return tmpExplosion;
    }

    /**
     * �����̉������
     */
    public void ExCollect(BillExplosionManager _explosion)
    {
        //�����̃Q�[���I�u�W�F�N�g���\��
        _explosion.gameObject.SetActive(false);
        //Queue�Ɋi�[
        explosionQueue.Enqueue(_explosion);
    }

    /**
     * ����݂��o������
     */
    public GameObject SetSmoke(Vector3 _pos)
    {
        //Queue����Ȃ�null
        if (explosionQueue.Count <= 0) return null;
        //Queue���甚��������o��
        GameObject tmpSmoke = smokeQueue.Dequeue();
        //������\������
        tmpSmoke.gameObject.SetActive(true);
        //�n���ꂽ���W�ɔ������ړ�����
        //tmpSmoke.ShowInStage(_pos);
        //�Ăяo�����ɓn��
        return tmpSmoke;
    }

    /**
     * ���̉������
     */
    public void SmokeCollect(GameObject _smoke)
    {
        //�����̃Q�[���I�u�W�F�N�g���\��
        _smoke.gameObject.SetActive(false);
        //Queue�Ɋi�[
        smokeQueue.Enqueue(_smoke);
    }
}
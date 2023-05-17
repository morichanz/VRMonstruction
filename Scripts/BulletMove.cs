using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    //�ǋL�@�I�u�W�F�N�g�v�[���p�R���g���[���[�i�[�p�ϐ��錾
    private PoolManager objectPool;

    [SerializeField, Header("�e�̑���")] private float speed = 250.0f;
    [SerializeField] private GameObject explosionPrefab;
    
    private Rigidbody rb;

    //SE
    private GameObject sound;
    private YoshigaSoundManager soundManager;
    [SerializeField] private GameObject bullet;
    private MeshRenderer bulletMesh;
    private ParticleSystem particle;
    private SphereCollider bulletCollider;
    [SerializeField] private ParticleSystem fire;

    void Start()
    {
        //�T�E���h�}�l�[�W���[�̎擾
        sound = GameObject.Find("SoundManager");
        soundManager = sound.transform.GetComponent<YoshigaSoundManager>();
        //�ǋL�@�I�u�W�F�N�g�v�[�����擾
        objectPool = transform.parent.GetComponent<PoolManager>();
        rb = GetComponent<Rigidbody>();
        particle = GetComponent<ParticleSystem>();
        fire = GetComponent<ParticleSystem>();
        gameObject.SetActive(false);
        bulletMesh = bullet.transform.GetComponent<MeshRenderer>();
        bulletCollider = this.transform.GetComponent<SphereCollider>();
    }

    /**
     * ���������^�O��BattleShip�ȊO��������폜
     */
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "BattleShip")
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            soundManager.PlaySE("BulletExplosionSound");
            bulletMesh.enabled = false;
            bulletCollider.enabled = false;
            particle.Stop();
            fire.Stop();
        }
    }

    private void OnParticleSystemStopped()
    {
        bulletMesh.enabled = true;
        bulletCollider.enabled = true;
        HideFromStage();
    }

    /**
     * �ݏo����
     */
    public void ShowInStage(Vector3 _pos,Vector3 _vec)
    {
        //�݂��o����鏈�����L�q����
        //�ǋL�@position��n���ꂽ���W�ɐݒ�
        transform.position = _pos;
        rb.velocity = _vec * speed;
    }

    /**
     * �������
     */
    public void HideFromStage()
    {
        //�������鏈�����L�q����
        //�I�u�W�F�N�g�v�[����Collect�֐����Ăяo�����g�����
        objectPool.Collect(this);
    }
}

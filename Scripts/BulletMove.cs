using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    //追記　オブジェクトプール用コントローラー格納用変数宣言
    private PoolManager objectPool;

    [SerializeField, Header("弾の速さ")] private float speed = 250.0f;
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
        //サウンドマネージャーの取得
        sound = GameObject.Find("SoundManager");
        soundManager = sound.transform.GetComponent<YoshigaSoundManager>();
        //追記　オブジェクトプールを取得
        objectPool = transform.parent.GetComponent<PoolManager>();
        rb = GetComponent<Rigidbody>();
        particle = GetComponent<ParticleSystem>();
        fire = GetComponent<ParticleSystem>();
        gameObject.SetActive(false);
        bulletMesh = bullet.transform.GetComponent<MeshRenderer>();
        bulletCollider = this.transform.GetComponent<SphereCollider>();
    }

    /**
     * 当たったタグがBattleShip以外だったら削除
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
     * 貸出処理
     */
    public void ShowInStage(Vector3 _pos,Vector3 _vec)
    {
        //貸し出される処理を記述する
        //追記　positionを渡された座標に設定
        transform.position = _pos;
        rb.velocity = _vec * speed;
    }

    /**
     * 回収処理
     */
    public void HideFromStage()
    {
        //回収される処理を記述する
        //オブジェクトプールのCollect関数を呼び出し自身を回収
        objectPool.Collect(this);
    }
}

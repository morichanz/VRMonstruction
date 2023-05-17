using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    //弾のプレハブ
    [SerializeField] private BulletMove bullet;
    //戦艦のプレハブ
    [SerializeField] private GameObject battleShip;

    //生成する弾数
    [SerializeField,Header("生成する弾数")] private int maxCount;
    //生成する戦艦の数
    [SerializeField, Header("生成する戦艦の数")] private int battleShipCnt;

    //生成した弾を格納するQueue
    private Queue<BulletMove> bulletQueue;
    private Queue<GameObject> battleShipQueue;

    //初回生成時のポジション
    Vector3 setPos = new Vector3(100, 100, 0);

    private void Awake()
    {
        //Queueの初期化
        bulletQueue = new Queue<BulletMove>();
        battleShipQueue = new Queue<GameObject>();

        //弾を生成
        for (int i = 0; i < maxCount; i++)
        {
            //生成
            BulletMove tmpBullet = Instantiate(bullet, setPos, Quaternion.identity, transform);

            //Queueに追加
            bulletQueue.Enqueue(tmpBullet);
        }
        //戦艦を生成
        for (int i = 0; i < battleShipCnt; i++)
        {
            //生成
            GameObject tmpBattleShip = Instantiate(battleShip, setPos, Quaternion.identity, transform);

            //Queueに追加
            battleShipQueue.Enqueue(tmpBattleShip);
        }
    }

    /**
     * 弾を貸し出す処理
     */
    public BulletMove Launch(Vector3 _pos,Vector3 _vec)
    {
        //Queueが空ならnull
        if (bulletQueue.Count <= 0) return null;
        //Queueから弾を一つ取り出す
        BulletMove tmpBullet = bulletQueue.Dequeue();
        //弾を表示する
        tmpBullet.gameObject.SetActive(true);
        //渡された座標に弾を移動する
        tmpBullet.ShowInStage(_pos, _vec);
        //呼び出し元に渡す
        return tmpBullet;
    }

    /**
     * 弾の回収処理
     */
    public void Collect(BulletMove _bullet)
    {
        //弾のゲームオブジェクトを非表示
        _bullet.gameObject.SetActive(false);
        //Queueに格納
        bulletQueue.Enqueue(_bullet);
    }

    /**
     * 戦艦を貸し出す処理
     */
    public GameObject Spawn()
    {
        //Queueが空ならnull
        if (battleShipQueue.Count <= 0) return null;
        //Queueから戦艦を一つ取り出す
        GameObject tmpBattleShip = battleShipQueue.Dequeue();
        //戦艦を表示する
        tmpBattleShip.gameObject.SetActive(true);
        //呼び出し元に渡す
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
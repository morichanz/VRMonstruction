using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BillEffectPoolManager : MonoBehaviour
{
    //爆発のプレハブ
    [SerializeField] private BillExplosionManager explosion;
    [SerializeField] private GameObject smoke;
    [SerializeField, Header("生成する爆発の数")] private int maxExplosionCount;
    [SerializeField, Header("生成する煙の数")] private int maxSmokeCount;

    //生成した爆発を格納するQueue
    private Queue<BillExplosionManager> explosionQueue;

    //生成した煙を格納するQueue
    private Queue<GameObject> smokeQueue;

    //初回生成時のポジション
    Vector3 setPos = new Vector3(100, 100, 0);

    private void Awake()
    {
        //Queueの初期化
        explosionQueue = new Queue<BillExplosionManager>();
        smokeQueue = new Queue<GameObject>();

        //爆発を生成
        for (int i = 0; i < maxExplosionCount; i++)
        {
            //生成
            BillExplosionManager tmpExplosion = Instantiate(explosion, setPos, Quaternion.identity, transform);

            //Queueに追加
            explosionQueue.Enqueue(tmpExplosion);
        }
        //煙を生成
        for (int i = 0; i < maxSmokeCount; i++)
        {
            //生成
            GameObject tmpSmoke = Instantiate(smoke, setPos, Quaternion.identity, transform);

            //Queueに追加
            smokeQueue.Enqueue(tmpSmoke);
        }
    }

    /**
     * 爆発を貸し出す処理
     */
    public BillExplosionManager Launch(Vector3 _pos)
    {
        //Queueが空ならnull
        if (explosionQueue.Count <= 0) return null;
        //Queueから爆発を一つ取り出す
        BillExplosionManager tmpExplosion = explosionQueue.Dequeue();
        //爆発を表示する
        tmpExplosion.gameObject.SetActive(true);
        //渡された座標に爆発を移動する
        tmpExplosion.ShowInStage(_pos);
        //呼び出し元に渡す
        return tmpExplosion;
    }

    /**
     * 爆発の回収処理
     */
    public void ExCollect(BillExplosionManager _explosion)
    {
        //爆発のゲームオブジェクトを非表示
        _explosion.gameObject.SetActive(false);
        //Queueに格納
        explosionQueue.Enqueue(_explosion);
    }

    /**
     * 煙を貸し出す処理
     */
    public GameObject SetSmoke(Vector3 _pos)
    {
        //Queueが空ならnull
        if (explosionQueue.Count <= 0) return null;
        //Queueから爆発を一つ取り出す
        GameObject tmpSmoke = smokeQueue.Dequeue();
        //爆発を表示する
        tmpSmoke.gameObject.SetActive(true);
        //渡された座標に爆発を移動する
        //tmpSmoke.ShowInStage(_pos);
        //呼び出し元に渡す
        return tmpSmoke;
    }

    /**
     * 煙の回収処理
     */
    public void SmokeCollect(GameObject _smoke)
    {
        //爆発のゲームオブジェクトを非表示
        _smoke.gameObject.SetActive(false);
        //Queueに格納
        smokeQueue.Enqueue(_smoke);
    }
}
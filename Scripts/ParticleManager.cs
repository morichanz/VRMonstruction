using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    //追記　オブジェクトプール用コントローラー格納用変数宣言
    //PoolManager objectPool;

    void Start()
    {
        //5秒後にDeleteExplosionを呼び出す
        Invoke("DeleteExplosion", 5.0f);
    }
    
    //爆発エフェクトの削除
    void DeleteExplosion()
    {
        Destroy(gameObject);
    }
}

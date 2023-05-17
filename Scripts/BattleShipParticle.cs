using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipParticle : MonoBehaviour
{
    void Start()
    {
        //2秒後にDeleteExplosionを呼び出す
        Invoke("DeleteExplosion", 2.0f);
    }

    //爆発エフェクトの削除
    void DeleteExplosion()
    {
        Destroy(gameObject);
    }
}

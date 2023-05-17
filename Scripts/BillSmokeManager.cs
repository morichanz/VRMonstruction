using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillSmokeManager : MonoBehaviour
{
    private BillEffectPoolManager objectPool;

    void Start()
    {
        //追記　オブジェクトプールを取得
        objectPool = transform.parent.GetComponent<BillEffectPoolManager>();
        gameObject.SetActive(false);
    }

    private void OnParticleSystemStopped()
    {
        HideFromStage();
    }

    /**
     * 貸出処理
     */
    public void ShowInStage(Vector3 _pos)
    {
        //貸し出される処理を記述する
        //追記　positionを渡された座標に設定
        transform.position = _pos;
    }

    /**
     * 回収処理
     */
    public void HideFromStage()
    {
        //回収される処理を記述する
        //オブジェクトプールのCollect関数を呼び出し自身を回収
        objectPool.SmokeCollect(this.gameObject);
    }
}

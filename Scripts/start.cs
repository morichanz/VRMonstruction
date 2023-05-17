using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start : MonoBehaviour
{
    [SerializeField] private PoolManager objectPool;
    [SerializeField,Header("戦艦スポーンインターバル")] private float Interval = 15;
    private int cnt = 0;
    private float spownCount = 0;

    void Update()
    {
        spownCount -= Time.deltaTime;

        if(objectPool.battleShipCounter() > 0)
        {
            if(spownCount <= 0)
            {
                objectPool.Spawn();
                cnt += 1;
                spownCount = Interval;
            }
        }


        if (cnt > objectPool.battleShipCounter())
        {
            this.gameObject.SetActive(false);
        }
    }
}
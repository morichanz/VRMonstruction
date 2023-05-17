using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipMove : MonoBehaviour
{
    //戦艦の当たり判定
    private BoxCollider box;

    //戦艦の表示
    private SkinnedMeshRenderer myMesh1;

    //オブジェクトプール
    private PoolManager objectPool;

    //発射台のメッシュを取得
    private SkinnedMeshRenderer barrelMesh;
    private SkinnedMeshRenderer canon_PartsMesh;
    private SkinnedMeshRenderer canonMesh;

    [SerializeField] private GameObject shipRig;
    [SerializeField] private GameObject barrelRig;
    [SerializeField] private GameObject canonRig;

    private GameObject battleShipSystem;

    // 戦艦を発進するオブジェクトを取得
    private GameObject start0;

    // 戦艦のターゲットのオブジェクトを取得
    private GameObject point1;
    private GameObject point2;
    private GameObject point3;

    //爆発
    [SerializeField] private GameObject explosionOnlyPrefab;
    //発射台
    [SerializeField] private GameObject canon;
    [SerializeField] private GameObject canon_Parts;
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject shipObject;

    [SerializeField, Header("Uターン開始時の旋回角度")] private float changeTime = 100;
    [SerializeField, Header("戦艦の速さ")] private float moveSpeed = 60;
    [SerializeField, Header("スタートから放物線移動時の中間点のX軸")] private float moveMaxHalwayPoint1 = 1450;
    [SerializeField, Header("ポイント２から放物線移動時の中間点のX軸")] private float ReturnmoveMaxHalwayPoint = 1000;
    [SerializeField, Header("旋回時の中間点のX軸")] private float turnMaxHalwayPoint = 250;
    [SerializeField, Header("弾の間隔")] private float interval = 1.0f;
    [SerializeField, Header("戦艦の索敵範囲")] private float distance = 400;

    //戦艦爆発フラグ
    private bool explosionFlg = false;
    //プレイヤーオブジェクトの取得
    private GameObject player;
    //Update内の関数を一回のみの呼び出しにする
    private int cnt;
    //弾を撃つカウント
    private float shotCnt;

    //スタート地点
    private Vector3 arrow_start;
    //終着地点
    private Vector3 arrow_end;
    //中間地点1
    private Vector3 arrow_middle;
    //中間地点２
    private Vector3 arrow_middle1;

    //発射台の座標調整
    private Vector3 bulletPos = new Vector3(0.0f, 30.5f, 0.0f);

    //音声
    private GameObject sound;
    private YoshigaSoundManager soundManager;

    private enum Roop
    {
      FirstStart,
      FirstTurn,
      SecondStart,
      SecondTurn,
    }
    [SerializeField,Header("確認用スタート位置")] private Roop roop = 0;

    private Vector3 latestPos;  //前回のPosition

    private void Start()
    {
        //サウンドマネージャーの取得
        sound = GameObject.Find("SoundManager");
        soundManager = sound.transform.GetComponent<YoshigaSoundManager>();
        //プレイヤー情報の取得
        player = GameObject.FindGameObjectWithTag("Player");
        //戦艦情報の取得
        battleShipSystem = GameObject.Find("BattleShipSystem");
        start0 = battleShipSystem.transform.GetChild(0).gameObject;
        point1 = battleShipSystem.transform.GetChild(1).gameObject;
        point2 = battleShipSystem.transform.GetChild(2).gameObject;
        point3 = battleShipSystem.transform.GetChild(3).gameObject;
        //オブジェクトプールの取得
        objectPool = battleShipSystem.transform.GetChild(5).GetComponent<PoolManager>();
        cnt = 0;
        //メッシュとコライダーの取得
        barrelMesh = barrel.transform.GetComponent<SkinnedMeshRenderer>();
        canon_PartsMesh = canon_Parts.transform.GetComponent<SkinnedMeshRenderer>();
        canonMesh = canon.transform.GetComponent<SkinnedMeshRenderer>();
        myMesh1 = shipObject.transform.GetComponent<SkinnedMeshRenderer>();
        box = transform.GetComponent<BoxCollider>();

        gameObject.SetActive(false);
    }

    void Update()
    {
        switch (roop)
        {
            case Roop.FirstStart:
                if (cnt == 0) ThrowingArrow();
                Searching();
                break;
            case Roop.FirstTurn:
                if (cnt == 0) ThrowingReturn();
                break;
            case Roop.SecondStart:
                if (cnt == 0) RThrowingArrow();
                Searching();
                break;
            case Roop.SecondTurn:
                if (cnt == 0) RThrowingReturn();
                break;
        }
    }

    /**
     * スタートからポイント1まで
     */
    private void ThrowingArrow()
    {
        //ベジェ曲線の制御点を取得
        //開始地点
        arrow_start = start0.transform.position;
        //終着地点
        arrow_end = point1.transform.position;
        //中間地点1
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.0f);
        //中間地点２
        arrow_middle1 = Vector3.Lerp(arrow_middle, arrow_end, 1.0f);

        //放物線を描くために中間地点のY軸を上に移動
        arrow_middle.x -= moveMaxHalwayPoint1;
        arrow_middle1.x -= moveMaxHalwayPoint1;

        //戦艦のスピード
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);
        //TankArrowコルーチンを開始
        StartCoroutine(TankArrow(arrow_start, arrow_end, arrow_middle, arrow_middle1, divide_distance));
    }
    private IEnumerator TankArrow(Vector3 start, Vector3 end, Vector3 middle, Vector3 middle1, float arrow_speed)
    {
        cnt ++;
        //ベジェ曲線用の変数の宣言
        float t = 0.0f;
        float elapsed = 0.0f;

        //ループを抜けるまで以下を繰り返す
        while (roop == Roop.FirstStart)
        {
            LaunchPortTarget();
            if (t > 1)
            {
                //ループを抜ける
                roop = Roop.FirstTurn;
                cnt = 0;
                yield break;
            }

            //ベジェ曲線の処理
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, middle1, t);
            Vector3 c = Vector3.Lerp(middle1, end, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            //座標を代入
            transform.position = Vector3.Lerp(d, e, t);

            //向き
            //前回からどこに進んだかをベクトルで取得
            Vector3 diff = transform.position - latestPos;

            //前回のPositionの更新
            latestPos = transform.position;

            if (diff.magnitude > 0.01f)
            {
                //向きを変更する
                Quaternion rot = Quaternion.LookRotation(diff);
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
    * ポイント1で旋回
    */
    private void ThrowingReturn()
    {
        //ベジェ曲線の制御点を取得
        //開始地点
        arrow_start = point1.transform.position;
        //終着地点
        arrow_end = point2.transform.position;
        //中間地点
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.5f);

        //放物線を描くために中間地点のY軸を上に移動
        arrow_middle.x += turnMaxHalwayPoint;

        //戦艦のスピード
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);

        //TankReturnArrowコルーチンを開始
        StartCoroutine(TankReturnArrow(arrow_start, arrow_end, arrow_middle, divide_distance));
    }
    private IEnumerator TankReturnArrow(Vector3 start, Vector3 end, Vector3 middle, float arrow_speed)
    {
        cnt++;
        //ベジェ曲線用の変数の宣言
        float elapsed = 0.0f;
        float t = 0.0f;
        //ループを抜けるまで以下を繰り返す
        while (roop == Roop.FirstTurn)
        {
            RLaunchPortTarget();
            if (t > 1)
            {
                //ループを抜ける
                roop = Roop.SecondStart;
                cnt = 0;
                yield break;
            }

            //ベジェ曲線の処理
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, end, t);
            //座標を代入
            transform.position = Vector3.Lerp(a, b, t);

            //向き
            //前回からどこに進んだかをベクトルで取得
            Vector3 diff = transform.position - latestPos;

            //前回のPositionの更新
            latestPos = transform.position;  
            if (diff.magnitude > 0.01f)
            {
                //向きを変更する
                Quaternion rot = Quaternion.LookRotation(diff); 
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
     * ポイント２からポイント３まで
     */
    private void RThrowingArrow()
    {
        //ベジェ曲線の制御点を取得
        //開始地点
        arrow_start = point2.transform.position;
        //終着地点
        arrow_end = point3.transform.position;
        //中間地点
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.0f);
        //中間地点２
        arrow_middle1 = Vector3.Lerp(arrow_middle, arrow_end, 1.0f);

        //放物線を描くために中間地点のY軸を上に移動
        arrow_middle.x -= ReturnmoveMaxHalwayPoint;
        arrow_middle1.x -= ReturnmoveMaxHalwayPoint;

        //戦艦のスピード
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);

        //RTankArrowコルーチンを開始
         StartCoroutine(RTankArrow(arrow_start, arrow_end, arrow_middle, arrow_middle1, divide_distance));
    }

    private IEnumerator RTankArrow(Vector3 start, Vector3 end, Vector3 middle, Vector3 middle1,float arrow_speed)
    {
        cnt++;
        //ベジェ曲線用の変数の宣言
        float t = 0.0f;
        float elapsed = 0.0f;
        //ループを抜けるまで以下を繰り返す
        while (roop == Roop.SecondStart)
        {
            RLaunchPortTarget();
            if (t > 1)
            {
                //ループを抜ける
                roop = Roop.SecondTurn;
                cnt = 0;
                yield break;
            }

            //ベジェ曲線の処理
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, middle1, t);
            Vector3 c = Vector3.Lerp(middle1, end, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);


            //座標を代入
            transform.position = Vector3.Lerp(d, e, t);

            //向き
            //前回からどこに進んだかをベクトルで取得
            Vector3 diff = transform.position - latestPos;

            //前回のPositionの更新
            latestPos = transform.position;
            if (diff.magnitude > 0.01f)
            {
                //向きを変更する
                Quaternion rot = Quaternion.LookRotation(diff);
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
    * スタートに向かって旋回
    */
    private void RThrowingReturn()
    {
        //ベジェ曲線の制御点を取得
        //開始地点
        arrow_start = point3.transform.position;
        //終着地点
        arrow_end = start0.transform.position;
        //中間地点
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.5f);
        //放物線を描くために中間地点のY軸を上に移動
        arrow_middle.x += turnMaxHalwayPoint;

        //戦艦のスピード
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);

        //RTankRetrnArrowコルーチンを開始
        StartCoroutine(RTankRetrnArrow(arrow_start, arrow_end, arrow_middle, divide_distance));
    }
    private IEnumerator RTankRetrnArrow(Vector3 start, Vector3 end, Vector3 middle, float arrow_speed)
    {
        cnt++;
        //ベジェ曲線用の変数の宣言
        float t = 0.0f;
        float elapsed = 0.0f;
        //ループを抜けるまで以下を繰り返す
        while (roop == Roop.SecondTurn)
        {
            LaunchPortTarget();
            if (t > 1)
            {
                //ループを抜ける
                roop = Roop.FirstStart;
                cnt = 0;
                yield break;
            }

            //ベジェ曲線の処理
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, end, t);
            //座標を代入
            transform.position = Vector3.Lerp(a, b, t);

            //向き
            //前回からどこに進んだかをベクトルで取得
            Vector3 diff = transform.position - latestPos;

            //前回のPositionの更新
            latestPos = transform.position;
            if (diff.magnitude > 0.01f)
            {
                //向きを変更する
                Quaternion rot = Quaternion.LookRotation(diff);
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
     * 発射台の向き
     */
    public void LaunchPortTarget()
    {
        canonRig.transform.rotation = transform.rotation;
    }
    public void RLaunchPortTarget()
    {
        Quaternion rot = Quaternion.Euler(0, 180, 0);
        Quaternion q = this.transform.rotation;
        canonRig.transform.rotation = q * rot;
    }

    /**
     * プレイヤーから攻撃を受けたとき
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "HeatBeam")
        {
            //戦艦の非表示
            explosionFlg = true;
            myMesh1.enabled = false;
            barrelMesh.enabled = false;
            canon_PartsMesh.enabled = false;
            canonMesh.enabled = false;
            box.enabled = false;
            soundManager.PlaySE("BattleShipBreakSound");
            //爆発プレハブの生成
            Instantiate(explosionOnlyPrefab, transform.position,Quaternion.identity);
            TelopManager.Instance.BreakNormalObject(TelopManager.BREAKABLE_OBJECTS.BATTLE_SHIP);
        }
    }
    /**
     * 戦艦の表示
     */
    public void MeshAndBoxTrigger()
    {
        explosionFlg = false;
        barrelMesh.enabled = true;
        myMesh1.enabled = true;
        canon_PartsMesh.enabled = true;
        canonMesh.enabled = true;
        box.enabled = true;
    }

    /**
     * 戦艦の索敵範囲
     */
    private void Searching()
    {
        //プレイヤーと戦艦のdistance
        float dis = Vector3.Distance(player.transform.position, this.transform.position);
        if (dis < distance)
        {
            shotCnt -= Time.deltaTime;
            if (shotCnt <= 0 && !explosionFlg)
            {
                objectPool.Launch(canonRig.transform.position, Vector3.Normalize(player.transform.position + bulletPos - canonRig.transform.position));
                soundManager.PlaySE("ShotSound");
                shotCnt = interval;
            }
        }
        else
        {
            shotCnt = 0;
        }
    }
}
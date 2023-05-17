using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleShipMove : MonoBehaviour
{
    //��͂̓����蔻��
    private BoxCollider box;

    //��͂̕\��
    private SkinnedMeshRenderer myMesh1;

    //�I�u�W�F�N�g�v�[��
    private PoolManager objectPool;

    //���ˑ�̃��b�V�����擾
    private SkinnedMeshRenderer barrelMesh;
    private SkinnedMeshRenderer canon_PartsMesh;
    private SkinnedMeshRenderer canonMesh;

    [SerializeField] private GameObject shipRig;
    [SerializeField] private GameObject barrelRig;
    [SerializeField] private GameObject canonRig;

    private GameObject battleShipSystem;

    // ��͂𔭐i����I�u�W�F�N�g���擾
    private GameObject start0;

    // ��͂̃^�[�Q�b�g�̃I�u�W�F�N�g���擾
    private GameObject point1;
    private GameObject point2;
    private GameObject point3;

    //����
    [SerializeField] private GameObject explosionOnlyPrefab;
    //���ˑ�
    [SerializeField] private GameObject canon;
    [SerializeField] private GameObject canon_Parts;
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject shipObject;

    [SerializeField, Header("U�^�[���J�n���̐���p�x")] private float changeTime = 100;
    [SerializeField, Header("��͂̑���")] private float moveSpeed = 60;
    [SerializeField, Header("�X�^�[�g����������ړ����̒��ԓ_��X��")] private float moveMaxHalwayPoint1 = 1450;
    [SerializeField, Header("�|�C���g�Q����������ړ����̒��ԓ_��X��")] private float ReturnmoveMaxHalwayPoint = 1000;
    [SerializeField, Header("���񎞂̒��ԓ_��X��")] private float turnMaxHalwayPoint = 250;
    [SerializeField, Header("�e�̊Ԋu")] private float interval = 1.0f;
    [SerializeField, Header("��͂̍��G�͈�")] private float distance = 400;

    //��͔����t���O
    private bool explosionFlg = false;
    //�v���C���[�I�u�W�F�N�g�̎擾
    private GameObject player;
    //Update���̊֐������݂̂̌Ăяo���ɂ���
    private int cnt;
    //�e�����J�E���g
    private float shotCnt;

    //�X�^�[�g�n�_
    private Vector3 arrow_start;
    //�I���n�_
    private Vector3 arrow_end;
    //���Ԓn�_1
    private Vector3 arrow_middle;
    //���Ԓn�_�Q
    private Vector3 arrow_middle1;

    //���ˑ�̍��W����
    private Vector3 bulletPos = new Vector3(0.0f, 30.5f, 0.0f);

    //����
    private GameObject sound;
    private YoshigaSoundManager soundManager;

    private enum Roop
    {
      FirstStart,
      FirstTurn,
      SecondStart,
      SecondTurn,
    }
    [SerializeField,Header("�m�F�p�X�^�[�g�ʒu")] private Roop roop = 0;

    private Vector3 latestPos;  //�O���Position

    private void Start()
    {
        //�T�E���h�}�l�[�W���[�̎擾
        sound = GameObject.Find("SoundManager");
        soundManager = sound.transform.GetComponent<YoshigaSoundManager>();
        //�v���C���[���̎擾
        player = GameObject.FindGameObjectWithTag("Player");
        //��͏��̎擾
        battleShipSystem = GameObject.Find("BattleShipSystem");
        start0 = battleShipSystem.transform.GetChild(0).gameObject;
        point1 = battleShipSystem.transform.GetChild(1).gameObject;
        point2 = battleShipSystem.transform.GetChild(2).gameObject;
        point3 = battleShipSystem.transform.GetChild(3).gameObject;
        //�I�u�W�F�N�g�v�[���̎擾
        objectPool = battleShipSystem.transform.GetChild(5).GetComponent<PoolManager>();
        cnt = 0;
        //���b�V���ƃR���C�_�[�̎擾
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
     * �X�^�[�g����|�C���g1�܂�
     */
    private void ThrowingArrow()
    {
        //�x�W�F�Ȑ��̐���_���擾
        //�J�n�n�_
        arrow_start = start0.transform.position;
        //�I���n�_
        arrow_end = point1.transform.position;
        //���Ԓn�_1
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.0f);
        //���Ԓn�_�Q
        arrow_middle1 = Vector3.Lerp(arrow_middle, arrow_end, 1.0f);

        //��������`�����߂ɒ��Ԓn�_��Y������Ɉړ�
        arrow_middle.x -= moveMaxHalwayPoint1;
        arrow_middle1.x -= moveMaxHalwayPoint1;

        //��͂̃X�s�[�h
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);
        //TankArrow�R���[�`�����J�n
        StartCoroutine(TankArrow(arrow_start, arrow_end, arrow_middle, arrow_middle1, divide_distance));
    }
    private IEnumerator TankArrow(Vector3 start, Vector3 end, Vector3 middle, Vector3 middle1, float arrow_speed)
    {
        cnt ++;
        //�x�W�F�Ȑ��p�̕ϐ��̐錾
        float t = 0.0f;
        float elapsed = 0.0f;

        //���[�v�𔲂���܂ňȉ����J��Ԃ�
        while (roop == Roop.FirstStart)
        {
            LaunchPortTarget();
            if (t > 1)
            {
                //���[�v�𔲂���
                roop = Roop.FirstTurn;
                cnt = 0;
                yield break;
            }

            //�x�W�F�Ȑ��̏���
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, middle1, t);
            Vector3 c = Vector3.Lerp(middle1, end, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            //���W����
            transform.position = Vector3.Lerp(d, e, t);

            //����
            //�O�񂩂�ǂ��ɐi�񂾂����x�N�g���Ŏ擾
            Vector3 diff = transform.position - latestPos;

            //�O���Position�̍X�V
            latestPos = transform.position;

            if (diff.magnitude > 0.01f)
            {
                //������ύX����
                Quaternion rot = Quaternion.LookRotation(diff);
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
    * �|�C���g1�Ő���
    */
    private void ThrowingReturn()
    {
        //�x�W�F�Ȑ��̐���_���擾
        //�J�n�n�_
        arrow_start = point1.transform.position;
        //�I���n�_
        arrow_end = point2.transform.position;
        //���Ԓn�_
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.5f);

        //��������`�����߂ɒ��Ԓn�_��Y������Ɉړ�
        arrow_middle.x += turnMaxHalwayPoint;

        //��͂̃X�s�[�h
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);

        //TankReturnArrow�R���[�`�����J�n
        StartCoroutine(TankReturnArrow(arrow_start, arrow_end, arrow_middle, divide_distance));
    }
    private IEnumerator TankReturnArrow(Vector3 start, Vector3 end, Vector3 middle, float arrow_speed)
    {
        cnt++;
        //�x�W�F�Ȑ��p�̕ϐ��̐錾
        float elapsed = 0.0f;
        float t = 0.0f;
        //���[�v�𔲂���܂ňȉ����J��Ԃ�
        while (roop == Roop.FirstTurn)
        {
            RLaunchPortTarget();
            if (t > 1)
            {
                //���[�v�𔲂���
                roop = Roop.SecondStart;
                cnt = 0;
                yield break;
            }

            //�x�W�F�Ȑ��̏���
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, end, t);
            //���W����
            transform.position = Vector3.Lerp(a, b, t);

            //����
            //�O�񂩂�ǂ��ɐi�񂾂����x�N�g���Ŏ擾
            Vector3 diff = transform.position - latestPos;

            //�O���Position�̍X�V
            latestPos = transform.position;  
            if (diff.magnitude > 0.01f)
            {
                //������ύX����
                Quaternion rot = Quaternion.LookRotation(diff); 
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
     * �|�C���g�Q����|�C���g�R�܂�
     */
    private void RThrowingArrow()
    {
        //�x�W�F�Ȑ��̐���_���擾
        //�J�n�n�_
        arrow_start = point2.transform.position;
        //�I���n�_
        arrow_end = point3.transform.position;
        //���Ԓn�_
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.0f);
        //���Ԓn�_�Q
        arrow_middle1 = Vector3.Lerp(arrow_middle, arrow_end, 1.0f);

        //��������`�����߂ɒ��Ԓn�_��Y������Ɉړ�
        arrow_middle.x -= ReturnmoveMaxHalwayPoint;
        arrow_middle1.x -= ReturnmoveMaxHalwayPoint;

        //��͂̃X�s�[�h
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);

        //RTankArrow�R���[�`�����J�n
         StartCoroutine(RTankArrow(arrow_start, arrow_end, arrow_middle, arrow_middle1, divide_distance));
    }

    private IEnumerator RTankArrow(Vector3 start, Vector3 end, Vector3 middle, Vector3 middle1,float arrow_speed)
    {
        cnt++;
        //�x�W�F�Ȑ��p�̕ϐ��̐錾
        float t = 0.0f;
        float elapsed = 0.0f;
        //���[�v�𔲂���܂ňȉ����J��Ԃ�
        while (roop == Roop.SecondStart)
        {
            RLaunchPortTarget();
            if (t > 1)
            {
                //���[�v�𔲂���
                roop = Roop.SecondTurn;
                cnt = 0;
                yield break;
            }

            //�x�W�F�Ȑ��̏���
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, middle1, t);
            Vector3 c = Vector3.Lerp(middle1, end, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);


            //���W����
            transform.position = Vector3.Lerp(d, e, t);

            //����
            //�O�񂩂�ǂ��ɐi�񂾂����x�N�g���Ŏ擾
            Vector3 diff = transform.position - latestPos;

            //�O���Position�̍X�V
            latestPos = transform.position;
            if (diff.magnitude > 0.01f)
            {
                //������ύX����
                Quaternion rot = Quaternion.LookRotation(diff);
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
    * �X�^�[�g�Ɍ������Đ���
    */
    private void RThrowingReturn()
    {
        //�x�W�F�Ȑ��̐���_���擾
        //�J�n�n�_
        arrow_start = point3.transform.position;
        //�I���n�_
        arrow_end = start0.transform.position;
        //���Ԓn�_
        arrow_middle = Vector3.Lerp(arrow_start, arrow_end, 0.5f);
        //��������`�����߂ɒ��Ԓn�_��Y������Ɉړ�
        arrow_middle.x += turnMaxHalwayPoint;

        //��͂̃X�s�[�h
        float divide_distance = moveSpeed / Vector3.Distance(arrow_start, arrow_end);

        //RTankRetrnArrow�R���[�`�����J�n
        StartCoroutine(RTankRetrnArrow(arrow_start, arrow_end, arrow_middle, divide_distance));
    }
    private IEnumerator RTankRetrnArrow(Vector3 start, Vector3 end, Vector3 middle, float arrow_speed)
    {
        cnt++;
        //�x�W�F�Ȑ��p�̕ϐ��̐錾
        float t = 0.0f;
        float elapsed = 0.0f;
        //���[�v�𔲂���܂ňȉ����J��Ԃ�
        while (roop == Roop.SecondTurn)
        {
            LaunchPortTarget();
            if (t > 1)
            {
                //���[�v�𔲂���
                roop = Roop.FirstStart;
                cnt = 0;
                yield break;
            }

            //�x�W�F�Ȑ��̏���
            t += arrow_speed * Time.deltaTime;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, end, t);
            //���W����
            transform.position = Vector3.Lerp(a, b, t);

            //����
            //�O�񂩂�ǂ��ɐi�񂾂����x�N�g���Ŏ擾
            Vector3 diff = transform.position - latestPos;

            //�O���Position�̍X�V
            latestPos = transform.position;
            if (diff.magnitude > 0.01f)
            {
                //������ύX����
                Quaternion rot = Quaternion.LookRotation(diff);
                elapsed += Time.deltaTime;
                rot = Quaternion.Lerp(transform.rotation, rot, elapsed / changeTime);
                transform.rotation = rot;
            }
            yield return null;
        }
    }

    /**
     * ���ˑ�̌���
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
     * �v���C���[����U�����󂯂��Ƃ�
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "HeatBeam")
        {
            //��͂̔�\��
            explosionFlg = true;
            myMesh1.enabled = false;
            barrelMesh.enabled = false;
            canon_PartsMesh.enabled = false;
            canonMesh.enabled = false;
            box.enabled = false;
            soundManager.PlaySE("BattleShipBreakSound");
            //�����v���n�u�̐���
            Instantiate(explosionOnlyPrefab, transform.position,Quaternion.identity);
            TelopManager.Instance.BreakNormalObject(TelopManager.BREAKABLE_OBJECTS.BATTLE_SHIP);
        }
    }
    /**
     * ��͂̕\��
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
     * ��͂̍��G�͈�
     */
    private void Searching()
    {
        //�v���C���[�Ɛ�͂�distance
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCtr : PlayerCtr
{
    #region 强力
    [Space]
    [Header("强力技能")]
    public bool canStrong;
    [Header("丢人力度")]
    public float throwForce;
    [Header("强力技能冷却")]
    public float strongSkillColdTime;
    private float strongSkillColdTimer;
    [Header("强力技能持续时间")]
    public float strongKeepTime = 4f;
    [Header("强力技能前摇")]
    public float strongBootTime = 0.5f;
    [Header("强力技能增加的移动速度")]
    public float addMoveSpeed;
    [Header("强力技能增加的攀爬速度")]
    public float addClimbSpeed;
    [Space]
    [Header("强力丢人判定")]
    public Vector3 pointOffsetStrong;
    public Vector3 sizeStrong;
    public LayerMask strongLayerMask;
    private Vector3 strongOffset;
    #endregion


    void Start()
    {
        strongOffset = pointOffsetStrong;
    }

    void Update()
    {
        Skill_1();
        Skill_2();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + pointOffsetStrong, sizeStrong);
    }

    #region 强力技能
    private Collider IsCanThrow()
    {
        Collider[] collider =
            Physics.OverlapBox(transform.position + pointOffsetStrong, sizeStrong, Quaternion.identity, strongLayerMask);
        foreach (var item in collider)
        {
            if (item.gameObject.name == "Player1")
            {
                return item;
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// 强力技能
    /// </summary>
    protected override void Skill_1()
    {
        if (Input.GetAxisRaw("Horizontal_Player2") == 1 || Input.GetAxisRaw("Horizontal_Player2") == -1)
            pointOffsetStrong.x = strongOffset.x * Input.GetAxisRaw("Horizontal_Player2");
        strongSkillColdTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Keypad1) && strongSkillColdTimer <= 0)//按下使用强力技能的按钮 并且技能不在冷却
        {
            strongSkillColdTimer = strongSkillColdTime;
            //能跳跃 属性增强
            canJump = true;
            moveSpeed += addMoveSpeed;
            //TODO 攀爬能力提升
            StartCoroutine(StrongTime());//技能持续时间倒计时
        }

        if (canStrong)
        {
            //能丢起小女孩
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                if (IsCanThrow() != null)
                {
                    Collider girl = IsCanThrow();
                    Rigidbody girlRg = girl.GetComponent<Rigidbody>();
                    girlRg.AddForce(Vector3.up * throwForce, ForceMode.Impulse);
                }
            }
        }
    }

    private IEnumerator StrongTime()
    {
        yield return new WaitForSeconds(strongBootTime);
        canStrong = true;
        yield return new WaitForSeconds(strongKeepTime);
        canStrong = false;
        canJump = false;
        moveSpeed -= addMoveSpeed;
    }

    #endregion

    protected override void Skill_2()
    {
        if (Input.GetAxisRaw("Skill2_Player2") ==1)
        {
            if (Input.GetAxisRaw("Vertical_Player2") >0)
            {
                transform.localScale += new Vector3(0, 1 * Time.deltaTime, 0);
            }
            else if (Input.GetAxisRaw("Vertical_Player2") < 0)
            {
                transform.localScale -= new Vector3(0, 1 * Time.deltaTime, 0);
            }
        }
    }

}

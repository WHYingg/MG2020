using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum PlayerType
{
    Player1,
    Player2
}
public class PlayerCtr : MonoBehaviour
{
    public PlayerType playerType;
    #region 移动
    [Header("移动")]
    public bool canMove = true;
    [Header("移动速度")]
    public float moveSpeed;
    [Header("到达目标速度时间")]
    public float accelerateTime;
    [Header("减速到0时间")]
    public float decelerateTime;
    [Header("输入偏移量")]
    public Vector3 inputOffset;
    private float velocityX;

    #endregion

    #region 跳跃
    [Space]
    [Header("跳跃")]
    public bool canJump = true;
    [Header("跳跃速度")]
    public float jumpingSpeed;
    [Header("长按下落系数")]
    public float longJumpMultiplier;
    [Header("到达最高点后下落系数")]
    public float fallMultiplier;
    [Header("提前松开下落系数")]
    public float lowJumpMultiplier;
    private bool isJumping = false;

    [Header("触地判定")]
    public Vector3 pointOffset;
    public Vector3 size;
    public LayerMask groundLayerMask;
    /// <summary>
    /// 角色是否在地上
    /// </summary>
    private bool isOnGround;
    #endregion

    #region 冲刺
    [Space]
    [Header("冲刺")]
    public bool canDash = false;
    [Header("冲刺力度")]
    public float dashForce;
    [Header("冲刺时间")]
    public float dashTime;
    [Header("空气阻力最大值")]
    public float dragMaxForce;
    [Header("空气阻力持续时间")]
    public float dragDuration;
    private bool isDash = false;
    private Vector3 dashDir;

    #endregion

    #region 投掷
    [Space]
    [Header("投掷")]
    public bool canThrow;
    public bool isUseGravity = false;
    [Header("投掷判定")]
    public Vector3 throwPointOffset;
    public Vector3 throwSize;
    public LayerMask throwLayerMask;
    private Vector3 throwOffset;
    /// <summary>
    /// 瞄准点
    /// </summary>
    private GameObject aim;
    private float aimAngle;
    private float AimAngle { get { return aimAngle; } set { aimAngle = Mathf.Clamp(value, 0, 85); } }
    private GameObject bullseye;
    [Header("射线检测层级")]
    public LayerMask HitLayer;
    [Header("线段材质")]
    public Material LineMaterial;

    [Header("线段间距")]
    [Range(0, 1)] public float segmentBreak = 0.1f;
    [Header("线段初始速度")]
    [Range(0, 50)] public float lineStaetVelocity = 15f;
    [Header("线段数量")]
    public int segmentCount = 25;
    [Header("线段宽度")]
    public float segmentWidth = 0.1f;
    [Header("线段长度")]
    public float arcDuration = 3.0f;
    private float arcTimeOffset = 0;

    private float throwVelocityX;
    private float throwVelocityY;
    private float currentTime;
    private float currentTimeOffset;
    private float hitTime;
    private float launchAngle;

    private bool isMove = false;

    private List<LineRenderer> LineRendererList = new List<LineRenderer>();

    private RaycastHit hitInfo;

    private Vector3 arcSpeed;
    private Vector3 currentAngle;

    public static bool Pointer;
    private GameObject throwObj;
    #endregion

    #region 抓取
    [Space]
    [Header("抓取")]
    public bool canTake;
    [Header("抓取判定")]
    public Vector3 takePointOffset;
    public Vector3 takeSize;
    public LayerMask takeLayerMask;
    private Vector3 takeOffset;
    private GameObject takeObj;
    private bool isTakeing;
    #endregion

    #region 爬墙
    [Space]
    [Header("爬墙")]
    public bool canClimb;
    public float climbSpeed;
    private bool isClimbing;
    [Header("爬墙判定")]
    public Vector3 climbPointOffset;
    public Vector3 climbSize;
    public LayerMask climbLayerMask;
    private Vector3 climbOffset;
    #endregion

    #region 爬墙跳
    [Space]
    [Header("爬墙跳")]
    public bool canClimbJump;
    public float climbJumpForceX;
    public float climbJumpForceY;
    #endregion

    #region 属性
    protected Rigidbody rg;
    protected Vector3 scale;
    #endregion

    protected virtual void Awake()
    {
        rg = GetComponent<Rigidbody>();
        scale = transform.localScale;
        CreateArc();
        climbOffset = climbPointOffset;
        throwOffset = throwPointOffset;
        takeOffset = takePointOffset;
    }

    private void Update()
    {
        aim.transform.rotation = Quaternion.Euler(0, 0, AimAngle * transform.localScale.x);
        Throw();
        Take();

    }

    protected virtual void FixedUpdate()
    {
        Move();
        Jump();
        Dash();
        DrawArc();
        AngleSetting();
        ControlMove();
        Climb();
        ClimbJump();
    }

    #region 基础操控
    private void Move()
    {
        if (canMove)
        {
            if (playerType == PlayerType.Player1)
            {
                if (Input.GetAxisRaw("Horizontal_Player1") > inputOffset.x)
                {
                    if (rg.velocity.x < moveSpeed * Time.fixedDeltaTime * 60)
                        rg.velocity = new Vector3(Mathf.SmoothDamp(rg.velocity.x, moveSpeed * Time.fixedDeltaTime * 60, ref velocityX, accelerateTime), rg.velocity.y, rg.velocity.z);
                    transform.localScale = new Vector3(scale.x, scale.y, scale.z);
                }
                else if (Input.GetAxisRaw("Horizontal_Player1") < -inputOffset.x)
                {
                    if (rg.velocity.x > moveSpeed * Time.fixedDeltaTime * -60)
                        rg.velocity = new Vector3(Mathf.SmoothDamp(rg.velocity.x, moveSpeed * Time.fixedDeltaTime * -60, ref velocityX, accelerateTime), rg.velocity.y, rg.velocity.z);
                    transform.localScale = new Vector3(-scale.x, scale.y, scale.z);

                }
                else
                {
                    rg.velocity = new Vector3(Mathf.SmoothDamp(rg.velocity.x, 0, ref velocityX, decelerateTime), rg.velocity.y, rg.velocity.z);
                }
            }
            else
            {
                if (Input.GetAxisRaw("Horizontal_Player2") > inputOffset.x)
                {
                    if (rg.velocity.x < moveSpeed * Time.fixedDeltaTime * 60)
                        rg.velocity = new Vector3(Mathf.SmoothDamp(rg.velocity.x, moveSpeed * Time.fixedDeltaTime * 60, ref velocityX, accelerateTime), rg.velocity.y, rg.velocity.z);
                    transform.localScale = new Vector3(scale.x, scale.y, scale.z);
                }
                else if (Input.GetAxisRaw("Horizontal_Player2") < -inputOffset.x)
                {
                    if (rg.velocity.x > moveSpeed * Time.fixedDeltaTime * -60)
                        rg.velocity = new Vector3(Mathf.SmoothDamp(rg.velocity.x, moveSpeed * Time.fixedDeltaTime * -60, ref velocityX, accelerateTime), rg.velocity.y, rg.velocity.z);
                    transform.localScale = new Vector3(-scale.x, scale.y, scale.z);

                }
                else
                {
                    rg.velocity = new Vector3(Mathf.SmoothDamp(rg.velocity.x, 0, ref velocityX, decelerateTime), rg.velocity.y, rg.velocity.z);
                }
            }

        }

    }
    private void Jump()
    {
        if (canJump)
        {
            if (playerType == PlayerType.Player1)
            {
                isOnGround = IsOnGround();
                //跳跃键按下并且没在跳
                if (Input.GetAxis("Jump_Player1") == 1 && !isJumping)
                {
                    rg.velocity = new Vector3(rg.velocity.x, jumpingSpeed, rg.velocity.z);
                    isJumping = true;
                }

                if (Input.GetAxis("Jump_Player1") == 1 && isJumping)//长跳时的加速下落
                {
                    rg.velocity += -Vector3.up * (longJumpMultiplier - 1) * Time.fixedDeltaTime;
                }

                if (isOnGround && Input.GetAxis("Jump_Player1") == 0)
                {
                    isJumping = false;
                }

                if (rg.velocity.y < 0)//加速下坠
                {
                    rg.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                }
                else if (rg.velocity.y > 0 && Input.GetAxis("Jump_Player1") != 1)//松开减缓上升
                {
                    rg.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
                }
            }
            else
            {
                isOnGround = IsOnGround();
                //跳跃键按下并且没在跳
                if (Input.GetAxis("Jump_Player2") == 1 && !isJumping)
                {
                    rg.velocity = new Vector3(rg.velocity.x, jumpingSpeed, rg.velocity.z);
                    isJumping = true;
                }

                if (Input.GetAxis("Jump_Player2") == 1 && isJumping)//长跳时的加速下落
                {
                    rg.velocity += -Vector3.up * (longJumpMultiplier - 1) * Time.fixedDeltaTime;
                }

                if (isOnGround && Input.GetAxis("Jump_Player2") == 0)
                {
                    isJumping = false;
                }

                if (rg.velocity.y < 0)//加速下坠
                {
                    rg.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
                }
                else if (rg.velocity.y > 0 && Input.GetAxis("Jump_Player2") != 1)//松开减缓上升
                {
                    rg.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
                }
            }

        }

    }
    private bool IsOnGround()
    {
        Collider[] collider =
            Physics.OverlapBox(transform.position + pointOffset, size / 2, Quaternion.identity, groundLayerMask);
        if (collider.Length == 0)
        {
            return false;
        }
        return true;
    }
    private void Dash()
    {
        if (canDash)
        {
            if (playerType == PlayerType.Player1)
            {
                if (Input.GetAxisRaw("Dash") == 1 && !isDash)
                {
                    isDash = true;
                    dashDir = new Vector3(Input.GetAxisRaw("Horizontal_Player1"), 0);
                    if (dashDir == Vector3.zero && transform.localScale.x == scale.x)
                    {
                        dashDir.x = 1;
                    }
                    else if (dashDir == Vector3.zero && transform.localScale.x == -5)
                    {
                        dashDir.x = -1;
                    }
                    //动量清零
                    rg.velocity = Vector3.zero;
                    //施力冲刺
                    rg.velocity += dashDir.normalized * dashForce;
                    StartCoroutine(RealyDash());
                }
                if (isOnGround && Input.GetAxisRaw("Dash") == 0)
                {
                    isDash = false;
                }
            }
        }
    }
    private IEnumerator RealyDash()
    {
        //关闭移动和跳跃、攻击
        canJump = false;
        canMove = false;
        //关闭重力影响
        rg.useGravity = false;
        //施加空气阻力
        DOVirtual.Float(dragMaxForce, 0, dragDuration, (a) => { rg.drag = a; });
        //等待冲刺结束
        yield return new WaitForSeconds(dashTime);
        //恢复功能
        canJump = true;
        canMove = true;
        rg.useGravity = true;
    }
    #endregion

    #region 投掷
    public void CreateArc()
    {
        aim = new GameObject("Aim");
        aim.transform.parent = transform;
        aim.transform.position = transform.position + throwPointOffset;
        bullseye = new GameObject("Bullseye");
        GameObject arc = new GameObject("Arc");
        arc.transform.parent = transform;
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject cl = new GameObject("Arc_" + i);
            cl.transform.SetParent(arc.transform);
            LineRendererList.Add(cl.AddComponent<LineRenderer>());
            LineRendererList[i].startWidth = segmentWidth;
            LineRendererList[i].endWidth = segmentWidth;
            LineRendererList[i].material = LineMaterial;
            LineRendererList[i].shadowCastingMode = ShadowCastingMode.Off;
            LineRendererList[i].receiveShadows = false;
            LineRendererList[i].lightProbeUsage = LightProbeUsage.Off;
            LineRendererList[i].reflectionProbeUsage = ReflectionProbeUsage.Off;
            LineRendererList[i].enabled = false;
        }
    }

    private void DrawArc()
    {
        if (canThrow)
        {
            float timeStep = arcDuration / segmentCount;
            arcSpeed = aim.transform.right * lineStaetVelocity * transform.localScale.x;
            currentTimeOffset = (Time.time - arcTimeOffset) * 0.2f;
            if (currentTimeOffset > (timeStep + segmentBreak))
            {
                currentTimeOffset = 0;
                arcTimeOffset = Time.time;
            }
            float startTime = currentTimeOffset;

            hitTime = GetHitTime(out hitInfo);
            int i = 0;
            for (i = 0; i < segmentCount; i++)
            {
                float endTime = startTime + timeStep;
                Vector3 StartPos = GetTimePosition(startTime);
                Vector3 Endpos = GetTimePosition(endTime);
                SetPostion(i, StartPos, Endpos);
                startTime += timeStep + segmentBreak;
                if (startTime > hitTime)
                {
                    break;
                }
            }
            HideLine(i);
        }
    }

    public void SetPostion(int index, Vector3 StartPostion, Vector3 endPostion)
    {
        LineRendererList[index].enabled = true;
        LineRendererList[index].SetPosition(0, StartPostion);
        LineRendererList[index].SetPosition(1, endPostion);
    }

    private Vector3 GetTimePosition(float time)
    {
        Vector3 useGravity = isUseGravity ? Physics.gravity : Vector3.zero;
        Vector3 result = aim.transform.position + time * arcSpeed + (0.5f * time * time) * useGravity;
        return result;
    }

    //获取线段与碰撞时间
    private float GetHitTime(out RaycastHit hitInfo)
    {
        float timeStep = arcDuration / segmentCount;
        float startTime_1 = 0.0f;
        Vector3 startPos_1 = GetTimePosition(startTime_1);
        hitInfo = new RaycastHit();
        for (int i = 0; i < segmentCount; i++)
        {
            float endtime_1 = timeStep + startTime_1;
            Vector3 endpos_1 = GetTimePosition(endtime_1);
            Debug.DrawLine(startPos_1, endpos_1);
            if (Physics.Linecast(startPos_1, endpos_1, out hitInfo, HitLayer))
            {
                bullseye.transform.position = hitInfo.point + Vector3.up * 0.1F;
                bullseye.SetActive(true);
                Pointer = true;
                float distance = Vector3.Distance(startPos_1, endpos_1);
                float hitdistance = hitInfo.distance;
                return startTime_1 + (hitdistance / distance) * timeStep;
            }
            else
            {
                Pointer = false;
            }
            startPos_1 = endpos_1;
            startTime_1 = endtime_1;
        }
        return float.MaxValue;
    }

    private void HideLine(int Num)
    {
        for (int i = Num; i < segmentCount; i++)
        {
            LineRendererList[i].enabled = false;
        }
    }

    private void AngleSetting()
    {
        if (canThrow)
        {
            if (playerType == PlayerType.Player1)
            {
                if (Input.GetAxisRaw("Vertical_Player1") > 0)
                {
                    AimAngle += Time.fixedTime * 0.1f;
                }
                else if (Input.GetAxisRaw("Vertical_Player1") < 0)
                {
                    AimAngle -= Time.fixedTime * 0.1f;
                }
            }
            else
            {
                if (Input.GetAxisRaw("Vertical_Player2") > 0)
                {
                    AimAngle += Time.fixedTime * 0.1f;
                }
                else if (Input.GetAxisRaw("Vertical_Player2") < 0)
                {
                    AimAngle -= Time.fixedTime * 0.1f;
                }
            }
        }

    }

    private Collider[] GetThrowObj()
    {
        Collider[] collider =
            Physics.OverlapBox(transform.position + throwPointOffset, throwSize / 2, Quaternion.identity, throwLayerMask);
        if (collider.Length != 0)
        {
            return collider;
        }
        return null;
    }

    private void Throw()
    {
        if (canThrow)
        {
            if (playerType == PlayerType.Player1)
            {
                if (Input.GetAxisRaw("Horizontal_Player1") == 1 || Input.GetAxisRaw("Horizontal_Player1") == -1)
                    throwPointOffset.x = throwOffset.x * Input.GetAxisRaw("Horizontal_Player1");
                if (Input.GetAxisRaw("Throw_Player1") > 0)
                {
                    if (GetThrowObj() != null)
                    {
                        foreach (var item in GetThrowObj())
                        {
                            if (item.gameObject.tag == "battery" || item.gameObject.tag == "Player")
                            {
                                throwObj = item.gameObject;
                            }
                        }
                        currentTime = Time.time;
                        launchAngle = aim.transform.eulerAngles.z;
                        if (launchAngle > 180)
                        {
                            launchAngle = 360 - launchAngle;

                        }
                        isMove = !isMove;
                    }

                }
            }
            else
            {
                if (Input.GetAxisRaw("Horizontal_Player2") == 1 || Input.GetAxisRaw("Horizontal_Player2") == -1)
                    throwPointOffset.x = throwOffset.x * Input.GetAxisRaw("Horizontal_Player2");
                if (Input.GetAxisRaw("Throw_Player2") > 0)
                {
                    if (GetThrowObj() != null)
                    {
                        foreach (var item in GetThrowObj())
                        {
                            if (item.gameObject.tag == "battery" || item.gameObject.tag == "Player")
                            {
                                throwObj = item.gameObject;
                            }
                        }
                        currentTime = Time.time;
                        launchAngle = aim.transform.eulerAngles.z;
                        print(aim.transform.eulerAngles);
                        if (launchAngle > 180)
                        {
                            launchAngle = 360 - launchAngle;

                        }
                        isMove = !isMove;
                    }

                }
            }
        }
    }

    private void ControlMove()
    {
        if (isMove)
        {
            ThrowObjMove();
        }
    }

    private void ThrowObjMove()
    {
        float time = Time.time - currentTime;
        Vector3 playerXZ = aim.transform.position;
        playerXZ.y = 0;
        throwVelocityX = arcSpeed.magnitude;
        if (isUseGravity)
            throwVelocityY = lineStaetVelocity * Mathf.Sin(launchAngle * Mathf.Deg2Rad) + Physics.gravity.y * time;
        else
            throwVelocityY = lineStaetVelocity * Mathf.Sin(launchAngle * Mathf.Deg2Rad);
        Vector3 speed = aim.transform.right;
        speed *= throwVelocityX;
        speed.y = throwVelocityY;
        speed.x *= transform.localScale.x;
        throwObj.transform.position += (speed) * Time.fixedDeltaTime;
        Vector3 speedXZ = speed;
        speedXZ.y = 0;

        float tempTan = speedXZ.magnitude / speed.y;
        float hu = Mathf.Atan(tempTan);
        float angle = hu * Mathf.Rad2Deg;
        throwObj.transform.eulerAngles = new Vector3(angle, throwObj.transform.eulerAngles.y, throwObj.transform.eulerAngles.z);

        if (Vector3.Distance(throwObj.transform.position, bullseye.transform.position) < 0.05f)
        {
            isMove = !isMove;
        }
    }

    #endregion

    #region 抓取

    private GameObject GetTakeObj()
    {
        Collider[] collider =
           Physics.OverlapBox(transform.position + takePointOffset, takeSize / 2, Quaternion.identity, takeLayerMask);
        if (collider.Length > 0)
        {
            return collider[0].gameObject;
        }
        return null;
    }

    private void Take()
    {
        if (canTake)
        {
            if (takeObj != null && isTakeing)
                takeObj.transform.position = transform.position + takePointOffset;
            if (playerType == PlayerType.Player1)
            {
                if (Input.GetAxisRaw("Horizontal_Player1") == 1 || Input.GetAxisRaw("Horizontal_Player1") == -1)
                    takePointOffset.x = takeOffset.x * Input.GetAxisRaw("Horizontal_Player1");
                if (Input.GetAxisRaw("Interact_Player1") == 1)
                {
                    if (GetTakeObj() != null)
                    {
                        takeObj = GetTakeObj();
                        takeObj.transform.position = transform.position + takePointOffset;
                        isTakeing = true;
                    }

                }
                else
                {
                    isTakeing = false;
                }
            }
            else
            {
                if (Input.GetAxisRaw("Horizontal_Player2") == 1 || Input.GetAxisRaw("Horizontal_Player2") == -1)
                    takePointOffset.x = takeOffset.x * Input.GetAxisRaw("Horizontal_Player2");
                if (Input.GetAxisRaw("Interact_Player2") == 1)
                {
                    if (GetTakeObj() != null)
                    {
                        GameObject take = GetTakeObj();
                        take.transform.position = transform.position + takePointOffset;
                        isTakeing = true;
                    }
                }
                else
                {
                    isTakeing = false;
                }

            }

        }

    }

    #endregion

    #region 爬墙
    private bool IsOnWall()
    {
        Collider[] collider =
           Physics.OverlapBox(transform.localPosition + climbPointOffset, climbSize / 2, Quaternion.identity, climbLayerMask);
        if (collider.Length == 0)
        {
            return false;
        }
        return true;
    }

    private void Climb()
    {
        if (playerType == PlayerType.Player1)
        {
            if (Input.GetAxisRaw("Horizontal_Player1") == 1 || Input.GetAxisRaw("Horizontal_Player1") == -1)
                climbPointOffset.x = climbOffset.x * Input.GetAxisRaw("Horizontal_Player1");
            if (canClimb)
            {
                if (IsOnWall())
                {
                    if (Input.GetAxisRaw("Climb_Player1") > 0)
                    {
                        rg.velocity = new Vector3(rg.velocity.x, climbSpeed, rg.velocity.z);
                        isClimbing = true;
                    }
                    else
                    {
                        isClimbing = false;
                    }

                }
                else
                {
                    isClimbing = false;
                }
            }
        }
        else
        {
            if (Input.GetAxisRaw("Horizontal_Player2") == 1 || Input.GetAxisRaw("Horizontal_Player2") == -1)
                climbPointOffset.x = climbOffset.x * Input.GetAxisRaw("Horizontal_Player2");
            if (canClimb)
            {
                if (IsOnWall())
                {
                    if (Input.GetAxisRaw("Climb_Player2") > 0)
                    {
                        rg.velocity = new Vector3(rg.velocity.x, climbSpeed, rg.velocity.z);
                        isClimbing = true;
                    }
                    else
                    {
                        isClimbing = false;
                    }

                }
                else
                {
                    isClimbing = false;
                }
            }
        }

    }
    #endregion

    #region 爬墙跳

    private void ClimbJump()
    {
        if (playerType == PlayerType.Player1)
        {
            if (Input.GetAxisRaw("Jump_Player1") == 1 && isClimbing)
            {
                rg.velocity = new Vector3(climbJumpForceX * -transform.localScale.x, climbJumpForceY, rg.velocity.z);
            }
        }
        else
        {
            if (Input.GetAxisRaw("Jump_Player1") == 1 && isClimbing)
            {
                rg.velocity = new Vector3(climbJumpForceX * -transform.localScale.x, climbJumpForceY, rg.velocity.z);
            }
        }

    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + pointOffset, size);
        Gizmos.DrawWireCube(transform.position + throwPointOffset, throwSize);
        Gizmos.DrawWireCube(transform.position + takePointOffset, takeSize);
        Gizmos.DrawWireCube(transform.position + climbPointOffset, climbSize);

    }

    protected virtual void Skill_1() { }
    protected virtual void Skill_2() { }
    protected virtual void Skill_3() { }


}

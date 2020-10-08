using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test : MonoBehaviour
{
    public Transform player;
    public Transform target;

    public GameObject Bullseye;

    public LayerMask HitLayer;

    public Material LineMaterial;

    [Range(0, 1)] public float segmentBreak;
    [Range(0, 30)] public float startVelocity;

    public float segmentwidth = 0.1f;
    public float arcDuration = 3.0f;
    public float arcTimeOffset = 0;

    private float velocityX;
    private float velocityY;
    private float currentTime;
    private float currentTimeOffset;
    private float hitTime;
    private float launchAngle;

    private bool IsMove = false;

    [SerializeField]
    private bool IsUseGravity = false;

    private int segmentCount = 40;

    public List<LineRenderer> LineRendererList = new List<LineRenderer>();

    private RaycastHit hitInfo;

    private Vector3 arcSpeed;
    private Vector3 currentAngle;

    public static bool Pointer;

    public GameObject bullet;

    void Start()
    {
        if (target == null)
        {
            target = GameObject.Find("Target").transform;
            Bullseye = GameObject.Find("Target");
        }

        Bullseye.SetActive(false);
        CreateArc();
        currentAngle = Vector3.zero;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (bullet != null)
            //    Destroy(bullet);
            currentTime = Time.time;

            //bullet = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            InitBullet();

            launchAngle = player.transform.eulerAngles.x;
            if (launchAngle > 180)
            {
                launchAngle = 360 - launchAngle;

            }
            IsMove = !IsMove;
        }
    }

    private void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    //if (bullet != null)
        //    //    Destroy(bullet);
        //    print("shoot");
        //    currentTime = Time.time;

        //    //bullet = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        //    InitBullet();

        //    launchAngle = player.transform.eulerAngles.x;
        //    if (launchAngle > 180)
        //    {
        //        launchAngle = 360 - launchAngle;

        //    }
        //    IsMove = !IsMove;
        //}
        ControlMove();
        DrawArc();
    }

    public void ControlMove()
    {
        if (IsMove)
        {
            BulletMove();
        }
    }

    public void InitBullet()
    {
        bullet.transform.localScale = Vector3.one * 3;
        bullet.transform.position = player.transform.position;
        bullet.transform.rotation = player.transform.rotation;
    }

    public void BulletMove()
    {
        float time = Time.time - currentTime;
        Vector3 playerXZ = transform.position;
        playerXZ.y = 0;
        velocityX = arcSpeed.magnitude;
        velocityY = startVelocity * Mathf.Sin(launchAngle * Mathf.Deg2Rad) + Physics.gravity.y * time;
        Vector3 speed = transform.right;
        speed *= velocityX;
        speed.y = velocityY;

        bullet.transform.position += (speed) * Time.fixedDeltaTime;
        Vector3 speedXZ = speed;
        speedXZ.y = 0;

        float tempTan = speedXZ.magnitude / speed.y;
        float hu = Mathf.Atan(tempTan);
        float angle = hu * Mathf.Rad2Deg;
        bullet.transform.eulerAngles = new Vector3(angle, bullet.transform.eulerAngles.y, bullet.transform.eulerAngles.z);

        if (Vector3.Distance(bullet.transform.position, Bullseye.transform.position) < 0.1f)
        {
            IsMove = !IsMove;
        }
    }


    public void CreateArc()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject cl = new GameObject("Arc_" + i);
            cl.transform.SetParent(player.transform.Find("Arc"));
            //cl.layer = 10;
            LineRendererList.Add(cl.AddComponent<LineRenderer>());

            LineRendererList[i].startWidth = segmentwidth;
            LineRendererList[i].endWidth = segmentwidth;
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
        float timeStep = arcDuration / segmentCount;
        arcSpeed = transform.right * startVelocity;
        currentTimeOffset = (Time.time - arcTimeOffset) * 0.2f;
        if (currentTimeOffset > (timeStep + segmentBreak))
        {
            currentTimeOffset = 0;
            arcTimeOffset = Time.time;
        }
        float starttime = currentTimeOffset;

        hitTime = GetHitTime(out hitInfo);
        int i = 0;
        for (i = 0; i < segmentCount; i++)
        {
            float endTime = starttime + timeStep;
            Vector3 StartPos = GetTimePosition(starttime);
            Vector3 Endpos = GetTimePosition(endTime);
            SetPostion(i, StartPos, Endpos);
            starttime += timeStep + segmentBreak;
            if (starttime > hitTime)
            {
                break;
            }
        }
        HideLine(i);
    }
    //设置坐标
    public void SetPostion(int index, Vector3 StartPostion, Vector3 endPostion)
    {
        LineRendererList[index].enabled = true;
        LineRendererList[index].SetPosition(0, StartPostion);
        LineRendererList[index].SetPosition(1, endPostion);
    }

    public Vector3 GetTimePosition(float time)
    {
        Vector3 usegravity = IsUseGravity ? Physics.gravity : Vector3.zero;
        Vector3 result = player.transform.position + time * arcSpeed + (0.5f * time * time) * usegravity;
        return result;
    }

    //获取线段与碰撞时间
    public float GetHitTime(out RaycastHit hitInfo)
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
                Bullseye.transform.position = hitInfo.point + Vector3.up * 0.1F;
                Bullseye.SetActive(true);
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

    //隐藏线段
    public void HideLine(int Num)
    {
        for (int i = Num; i < segmentCount; i++)
        {
            LineRendererList[i].enabled = false;
        }
    }
}

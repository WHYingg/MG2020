using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public enum JoystickType
{
    Fixed,
    Create,
    Follow,
}

public enum JoystickEventType
{
    OnJoystickDown,
    OnJoystickUp,
    OnJoystickDrag
}

public class JoystickPanel : PanelBase
{
    /// <summary>
    /// 摇杆的生成范围
    /// </summary>
    private Image touchRange;

    private Image joystickBg;

    private Image joystick;

    public float maxDistance = 120;

    public bool isOnlyHorizontal = false;
    public bool isOnlyVertical = false;

    public JoystickType joystickType;

    private Color joystickColor;

    void Start()
    {
        touchRange = GetControl<Image>("TouchRange");
        joystickBg = GetControl<Image>("JoystickBg");
        joystick = GetControl<Image>("Joystick");
        joystickColor = joystick.color;
        UIManager.AddCustomEventListener(touchRange, EventTriggerType.PointerDown, OnPointerDown);
        UIManager.AddCustomEventListener(touchRange, EventTriggerType.PointerUp, OnPointerUp);
        UIManager.AddCustomEventListener(touchRange, EventTriggerType.Drag, OnDrag);
        if (joystickType == JoystickType.Create || joystickType == JoystickType.Follow)
            joystickBg.gameObject.SetActive(false);

    }

    private void OnPointerDown(BaseEventData data)
    {
        //改变透明度
        joystick.color = new Color(joystickColor.r, joystickColor.g, joystickColor.b, joystickColor.a * 0.5f);
        //变换位置
        if (joystickType == JoystickType.Create || joystickType == JoystickType.Follow)
        {
            joystickBg.gameObject.SetActive(true);
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
        touchRange.rectTransform,//需改变位置的对象的父对象
        (data as PointerEventData).position,//当前屏幕鼠标位置
        (data as PointerEventData).pressEventCamera,//UI对应的相机
        out localPos);//转换来的 相对坐标
                      //更新位置
            joystickBg.transform.localPosition = localPos;
        }
        //广播按下事件
        EventCenter.Broadcast(EventDefine.JoystickEvent, JoystickEventType.OnJoystickDown);
    }

    private void OnPointerUp(BaseEventData data)
    {
        //恢复原来设置
        joystick.color = joystickColor;
        joystick.transform.localPosition = Vector3.zero;
        EventCenter.Broadcast(EventDefine.Joystick, Vector2.zero);
        switch (joystickType)
        {
            case JoystickType.Fixed:
                break;
            case JoystickType.Create:
                joystickBg.gameObject.SetActive(false);
                break;
            case JoystickType.Follow:
                joystickBg.gameObject.SetActive(false);
                break;
        }
        //广播抬起事件
        EventCenter.Broadcast(EventDefine.JoystickEvent, JoystickEventType.OnJoystickUp);
    }

    private void OnDrag(BaseEventData data)
    {

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBg.rectTransform,//需改变位置的对象的父对象
            (data as PointerEventData).position,//当前屏幕鼠标位置
            (data as PointerEventData).pressEventCamera,//UI对应的相机
            out localPos);//转换来的 相对坐标
                          //更新位置

        if (isOnlyVertical && Mathf.Abs(localPos.y) > Mathf.Abs(localPos.x))
        {
            localPos = new Vector3(0, localPos.y * 500, 0);
        }
        if (isOnlyHorizontal && Mathf.Abs(localPos.x) > Mathf.Abs(localPos.y))
        {
            localPos = new Vector3(localPos.x * 500, 0, 0);
        }

        joystick.transform.localPosition = localPos;

        switch (joystickType)
        {
            case JoystickType.Fixed:
                //范围判断
                if (localPos.magnitude > maxDistance)
                {
                    joystick.transform.localPosition = localPos.normalized * maxDistance;
                }
                break;
            case JoystickType.Create:
                //范围判断
                if (localPos.magnitude > maxDistance)
                {
                    joystick.transform.localPosition = localPos.normalized * maxDistance;
                }
                break;
            case JoystickType.Follow:
                //范围判断
                if (localPos.magnitude > maxDistance)
                {
                    joystickBg.transform.localPosition += (Vector3)(localPos.normalized * (localPos.magnitude - maxDistance));
                    joystick.transform.localPosition = localPos.normalized * maxDistance;
                }
                break;
        }
        //广播摇杆拖拽事件
        EventCenter.Broadcast(EventDefine.Joystick, localPos.normalized);
        EventCenter.Broadcast(EventDefine.JoystickEvent, JoystickEventType.OnJoystickDrag);
    }
}

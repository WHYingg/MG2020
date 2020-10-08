using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum UILayer
{
    Bottom,
    Mid,
    Top,
    System
}

/// <summary>
/// UI管理模块
/// </summary>
public class UIManager : ManagerBase<UIManager>
{
    public RectTransform canvas;

    private Dictionary<string, PanelBase> panels = new Dictionary<string, PanelBase>();

    /// <summary>
    /// UI层级
    /// </summary>
    private Transform bottom, mid, top, system;

    private ResourceManager rm;

    public UIManager()
    {
        rm = ResourceManager.GetResourceManager();
        //创建Canvas
        GameObject obj = rm.load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);
        GameObject es = rm.load<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(es);
        //获得各层级
        bottom = canvas.Find("Bottom");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">PanelBase</typeparam>
    /// <param name="panleName">面板名称 path=""UI/" + panleName"</param>
    /// <param name="layer">层级</param>
    /// <param name="callBack">面板创建完成后的回调</param>
    public void ShowPanle<T>(string path, UILayer layer = UILayer.Mid, UnityAction<T> callBack = null) where T : PanelBase
    {
        string[] s = path.Split('/');
        string panleName = s[s.Length - 1];
        if (panels.ContainsKey(panleName))//面板是否已经显示
        {
            panels[panleName].OnShowPanel();
            callBack?.Invoke(panels[panleName] as T);
            return;
        }
        rm.LoadAsync<GameObject>(path, (obj) =>
        {
            Transform parent;
            switch (layer)
            {
                case UILayer.Bottom:
                    parent = bottom;
                    break;
                case UILayer.Mid:
                    parent = mid;
                    break;
                case UILayer.Top:
                    parent = top;
                    break;
                case UILayer.System:
                    parent = system;
                    break;
                default:
                    parent = bottom;
                    break;
            }
            //设置父对象和初始化
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            //得到面板上的面板脚本
            T panel = obj.GetComponent<T>();
            panel.OnShowPanel();
            //处理面板创建完成后的逻辑
            callBack?.Invoke(panel);
            //存储面板
            panels.Add(panleName, panel);

        });

    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">PanelBase</typeparam>
    /// <param name="panleName">面板名称</param>
    /// <param name="pos">显示位置</param>
    /// <param name="scale">显示时大小</param>
    /// <param name="layer">层级</param>
    /// <param name="callBack">面板显示完成后的回调</param>
    public void ShowPanle<T>(string path, Vector3 pos, Vector3 scale, UILayer layer = UILayer.Mid, UnityAction<T> callBack = null) where T : PanelBase
    {
        string[] s = path.Split('/');
        string panleName = s[s.Length - 1];
        if (panels.ContainsKey(panleName))//面板是否已经显示
        {
            panels[panleName].OnShowPanel();
            callBack?.Invoke(panels[panleName] as T);
            return;
        }

        rm.LoadAsync<GameObject>(path, (obj) =>
        {
            Transform parent;
            switch (layer)
            {
                case UILayer.Bottom:
                    parent = bottom;
                    break;
                case UILayer.Mid:
                    parent = mid;
                    break;
                case UILayer.Top:
                    parent = top;
                    break;
                case UILayer.System:
                    parent = system;
                    break;
                default:
                    parent = bottom;
                    break;
            }
            //设置父对象和初始化
            obj.transform.SetParent(parent);
            obj.transform.localPosition = pos;
            obj.transform.localScale = scale;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            //得到面板上的面板脚本
            T panel = obj.GetComponent<T>();
            panel.OnShowPanel();
            //处理面板创建完成后的逻辑
            callBack?.Invoke(panel);
            //存储面板
            panels.Add(panleName, panel);

        });

    }


    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName">面板名</param>
    public void HidePanle(string panelName)
    {
        if (panels.ContainsKey(panelName))
        {
            panels[panelName].OnHidePanel();
            GameObject.Destroy(panels[panelName].gameObject);
            panels.Remove(panelName);
        }
    }

    /// <summary>
    /// 获得正在显示的面板
    /// </summary>
    /// <typeparam name="T">面板脚本</typeparam>
    /// <param name="panelName">面板名</param>
    /// <returns></returns>
    public T GetOnShowPanle<T>(string panelName) where T : PanelBase
    {
        if (panels.ContainsKey(panelName))
        {
            return panels[panelName] as T;
        }
        return null;
    }

    /// <summary>
    /// 得到层级对应的物体
    /// </summary>
    /// <returns></returns>
    public Transform GetPanelLayer(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Bottom:
                return bottom;
            case UILayer.Mid:
                return mid;
            case UILayer.Top:
                return top;
            case UILayer.System:
                return system;
        }
        return null;
    }

    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 清除所有加载了的面板
    /// </summary>
    public void Clear()
    {
        foreach (KeyValuePair<string, PanelBase> item in panels)
        {
            item.Value.gameObject.SetActive(false);
            GameObject.Destroy(item.Value.gameObject);
        }
        panels.Clear();
    }

}

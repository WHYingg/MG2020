using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// </summary>
public class PanelBase : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        FindChildrenControl<Image>();
        FindChildrenControl<Button>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
        FindChildrenControl<Scrollbar>();
    }

    /// <summary>
    /// 找到子对象对应的控件
    /// </summary>
    /// <typeparam name="T">UI组件</typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = gameObject.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; i++)
        {
            string name = controls[i].gameObject.name;
            if (controlDic.ContainsKey(name))
            {
                controlDic[name].Add(controls[i]);
            }
            else { controlDic.Add(name, new List<UIBehaviour>() { controls[i] }); }

            #region 给控件添加监听事件
            //是否为按钮控件
            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() =>
                {
                    OnButtonClick(name);
                });
            } //是否为单多选框控件
            else if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnToggleValueChanged(name, value);
                });
            }//是否为滑动条控件
            else if (controls[i] is Slider)
            {
                (controls[i] as Slider).onValueChanged.AddListener((value) =>
                {
                    OnSliderValueChanged(name, value);
                });
            }//是否为输入框控件
            else if (controls[i] is InputField)
            {
                (controls[i] as InputField).onEndEdit.AddListener((value) =>
                {
                    OnInputFieldEndEdit(name, value);
                });
                (controls[i] as InputField).onValueChanged.AddListener((value) =>
                {
                    OnInputFieldValueChanged(name, value);
                });
            }
            #endregion

        }
    }

    /// <summary>
    /// 得到脚本控件
    /// </summary>
    /// <typeparam name="T">需要得到的控件类型</typeparam>
    /// <param name="name">挂载控件的物体名</param>
    /// <returns></returns>
    protected T GetControl<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            for (int i = 0; i < controlDic[name].Count; i++)
            {
                if (controlDic[name][i] is T)
                {
                    return controlDic[name][i] as T;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 当目标显示时调用一次
    /// </summary>
    public virtual void OnShowPanel() { }

    /// <summary>
    /// 但面板隐藏时调用一次
    /// </summary>
    public virtual void OnHidePanel() { }

    protected virtual void OnButtonClick(string buttonName) { }

    protected virtual void OnToggleValueChanged(string toggleName, bool isChange) { }

    protected virtual void OnSliderValueChanged(string sliderName, float value) { }

    protected virtual void OnInputFieldEndEdit(string sliderName, string value) { }

    protected virtual void OnInputFieldValueChanged(string sliderName, string value) { }
}

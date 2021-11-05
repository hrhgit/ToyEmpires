using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

// v1.4 - context menu line animation point count slider added
public class UIC_LocalLineAnimationPointCount : MonoBehaviour, I_UIC_ContextItem, I_UIC_Object
{
    UIC_Manager _uicManager;

    public string ID => "input";
    public Color objectColor { get; set; }
    public int Priority => -10;

    Slider _slider;

    public void Action()
    {

    }

    List<UIC_Connection> _connList = new List<UIC_Connection>();

    public void OnChangeSelection()
    {
        _connList.Clear();
        bool _show = false;
        foreach (I_UIC_Selectable item in _uicManager.selectedUIObjectsList)
        {
            if (item is UIC_Connection)
            {
                _connList.Add(item as UIC_Connection);
                _show = true;
            }
        }

        gameObject.SetActive(_show);

        if (_connList.Count == 1)
            _slider.value = _connList[0].line.animation.pointCount;
    }

    void Awake()
    {
        _uicManager = GetComponentInParent<UIC_Manager>();
        _slider = GetComponentInChildren<Slider>();
    }

    private void SetPointCount(float arg0)
    {
        foreach (UIC_Connection conn in _connList)
        {
            conn.line.animation.pointCount = (int)_slider.value;
        }
    }

    void OnEnable()
    {
        _slider.onValueChanged.AddListener(SetPointCount);
    }

    void OnDisable()
    {
        _slider.onValueChanged.RemoveAllListeners();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}

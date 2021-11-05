using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

// v1.4 - context menu line animation active toggle added
public class UIC_LocalLineAnimationActive : MonoBehaviour, I_UIC_ContextItem, I_UIC_Object
{
    UIC_Manager _uicManager;
    public string ID => "input";
    public Color objectColor { get; set; }
    public int Priority => -10;

    Toggle _toggle;

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
            _toggle.isOn = _connList[0].line.animation.isActive;
    }

    void Awake()
    {
        _uicManager = GetComponentInParent<UIC_Manager>();
        _toggle = GetComponentInChildren<Toggle>();
    }

    private void SetIsOn(bool arg0)
    {
        foreach (UIC_Connection conn in _connList)
        {
            conn.line.animation.isActive = _toggle.isOn;
        }
    }

    void OnEnable()
    {
        _toggle.onValueChanged.AddListener(SetIsOn);
    }

    void OnDisable()
    {
        _toggle.onValueChanged.RemoveAllListeners();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}

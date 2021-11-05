using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIC_LocalInputLineWidth : MonoBehaviour, I_UIC_ContextItem, I_UIC_Object
{
    UIC_Manager _uicManager;
    public string ID => "input";
    public Color objectColor { get; set; }
    public int Priority => -10;

    InputField _inputField;

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

        if (_connList.Count != 1)
            _inputField.text = "-";
        else
            _inputField.text = _connList[0].line.width.ToString();
    }

    void Awake()
    {
        _uicManager = GetComponentInParent<UIC_Manager>();
        _inputField = GetComponentInChildren<InputField>();
    }

    private void SetWidth(string arg0)
    {
        if (_inputField.text != "-")
        {
            foreach (UIC_Connection conn in _connList)
            {
                float w = conn.line.width;
                if (float.TryParse(_inputField.text, NumberStyles.Float, CultureInfo.InvariantCulture, out w))
                    conn.line.width = w;
            }
        }
    }

    void OnEnable()
    {
        _inputField.onValueChanged.AddListener(SetWidth);
    }

    void OnDisable()
    {
        _inputField.onValueChanged.RemoveAllListeners();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}

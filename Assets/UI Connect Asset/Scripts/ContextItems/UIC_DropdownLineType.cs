using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_DropdownLineType : MonoBehaviour, I_UIC_ContextItem, I_UIC_Object
{
    UIC_Manager _uicManager;
    Dropdown _dropdown;

    public string ID => "dropdown";
    public Color objectColor { get; set; }
    public int Priority => -9;

    public void Action()
    {

    }

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

        SetDropdown();
    }

    List<UIC_Connection> _connList = new List<UIC_Connection>();

    private void SetLineType(int arg0)
    {
        UIC_Connection.LineTypeEnum parsed_enum = (UIC_Connection.LineTypeEnum)System.Enum.Parse(typeof(UIC_Connection.LineTypeEnum), _dropdown.options[_dropdown.value].text);

        foreach (UIC_Connection conn in _connList)
        {
            conn.lineType = parsed_enum;
            conn.UpdateLine();
        }
    }

    void SetDropdown()
    {
        if (_connList.Count == 1)
        {
            _dropdown.value = _dropdown.options.FindIndex(option => option.text == _connList[0].lineType.ToString());
        }
    }

    void OnEnable()
    {
        SetDropdown();
        _dropdown.onValueChanged.AddListener(SetLineType);
    }

    void OnDisable()
    {
        _dropdown.onValueChanged.RemoveAllListeners();
    }

    void Awake()
    {
        _uicManager = GetComponentInParent<UIC_Manager>();
        _dropdown = GetComponent<Dropdown>();
    }

    void Start()
    {
        string[] _names = System.Enum.GetNames(typeof(UIC_Connection.LineTypeEnum));
        List<string> _options = new List<string>();
        _options.AddRange(_names);

        _dropdown.options.Clear();
        _dropdown.AddOptions(_options);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}

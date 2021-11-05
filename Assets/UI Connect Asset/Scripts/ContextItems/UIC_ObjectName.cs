using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_ObjectName : MonoBehaviour, I_UIC_ContextItem
{
    UIC_Manager _uicManager;
    Text _text;

    public void Action()
    {

    }

    public void OnChangeSelection()
    {
        if (_uicManager.selectedUIObjectsList.Count <= 0)
        {
            _text.text = "--";
        }
        else if (_uicManager.selectedUIObjectsList.Count == 1)
        {
            _text.text = (_uicManager.selectedUIObjectsList[0] as I_UIC_Object).ID;
        }
        else
        {
            _text.text = string.Format("Multiple Objects ({0})", _uicManager.selectedUIObjectsList.Count);
        }

    }

    void Awake()
    {
        _uicManager = GetComponentInParent<UIC_Manager>();
        _text = transform.GetComponentInChildren<Text>();
    }
}

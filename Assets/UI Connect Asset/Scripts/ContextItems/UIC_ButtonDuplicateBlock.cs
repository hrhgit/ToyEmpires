using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_ButtonDuplicateBlock : MonoBehaviour, I_UIC_ContextItem, I_UIC_Object
{
    UIC_Manager _uicManager;

    Button _button;
    public string ID => "button";
    public Color objectColor { get; set; }
    public int Priority => -10;

    void Awake()
    {
        _uicManager = GetComponentInParent<UIC_Manager>();
        _button = GetComponent<Button>();
    }

    public void Action()
    {
        for (int i = _uicManager.selectedUIObjectsList.Count - 1; i >= 0; i--)
        {
            UIC_Entity entityToDuplicate = _uicManager.selectedUIObjectsList[i] as UIC_Entity;
            if (entityToDuplicate)
            {
                _uicManager.InstantiateEntityAtPosition(entityToDuplicate, entityToDuplicate.transform.position + new Vector3(15, -15));
            }
        }
        UIC_ContextMenu.UpdateContextMenu();
    }

    // v1.5 - fix: incorrect enabling of button "duplicate"
    public void OnChangeSelection()
    {
        int entityCount = 0;
        for (int i = _uicManager.selectedUIObjectsList.Count - 1; i >= 0; i--)
        {
            UIC_Entity entity = _uicManager.selectedUIObjectsList[i] as UIC_Entity;
            if (entity)
            {
                entityCount++;
            }
        }
        gameObject.SetActive(entityCount > 0);
    }

    void OnEnable()
    {
        _button.onClick.AddListener(Action);
    }

    void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}

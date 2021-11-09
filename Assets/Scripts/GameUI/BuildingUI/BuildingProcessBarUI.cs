using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Buildings;
using Gameplay.GameUnit;
using UnityEngine;
using UnityEngine.UI;

public class BuildingProcessBarUI : MonoBehaviour
{
    
    public static Color    blueHpColor = new Color(0.18f, 0.36f, 1f);
    public static Color    redHpColor  = new Color(1f,    0.31f, 0.42f);
    public        Building building;
    public        Image    barUI;
    public        Image    outlineUI;

    private Camera     _camera;
    private Canvas     _canvas;

    private void Awake()
    {
        _canvas        = this.GetComponent<Canvas>();
        _camera        = Camera.main;
            
    }

    private void Start()
    {
        _canvas.worldCamera = BattleGameManager.BattleGameManagerInstance.uiCamera;
        barUI.color = building.buildingUnit.UnitTeam switch
                        {
                            Team.Blue => blueHpColor,
                            Team.Red  => redHpColor,
                            _         => throw new ArgumentOutOfRangeException()
                        };
    }

    private void Update()
    {
        this.transform.forward = -_camera.transform.forward;
    }

    private void FixedUpdate()
    {
        barUI.fillAmount = (float)building.process;
        outlineUI.gameObject.SetActive(!building.IsProdutivityRequiring);
    }
    
}

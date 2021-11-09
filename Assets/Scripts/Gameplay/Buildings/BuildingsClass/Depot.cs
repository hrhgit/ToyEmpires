using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Buildings;
using UnityEngine;

public class Depot : Building
{
    private Transform oriPos;
    protected override void Start()
    {
        oriPos = this.manager.player.resourceHome[(int)this.road];
        this.OnBuildingBuilt.AddListener((b =>
                                        {
                                            this.manager.player.ChangeResourceHome(this.road,this.transform);
                                        }));
        this.OnBuildingDestroy.AddListener((b =>
                                        {
                                            this.manager.player.ChangeResourceHome(this.road, oriPos,true);
                                        }));
    }
}

using System;
using Gameplay.GameUnit.SoldierUnit;
using Pathfinding;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering
{
    /// <summary>
    ///     Sprite渲染加成
    /// </summary>
    public class SpriteRendererPlus : MonoBehaviour
    {
        private GameAIUnitPath _aiPath;
        [ExecuteAlways]
        private void Start()
        {
            GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On;
            _aiPath                                          = this.transform.parent.GetComponent<GameAIUnitPath>();
        }

        private void Update()
        {
            var v = this._aiPath.steeringTarget - this.transform.parent.transform.position;
            this.transform.right = Vector3.forward * Math.Sign(v.z);
        }
    }
}
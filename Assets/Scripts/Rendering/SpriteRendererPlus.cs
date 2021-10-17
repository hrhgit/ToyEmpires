using System;
using Gameplay;
using Gameplay.GameUnit;
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
        private Rigidbody      _rigidbody;
        private Renderer       _renderer;
        private Material       _material;
        private GameUnitBase   _gameUnit;
        [ExecuteAlways]
        private void Awake()
        {
            _gameUnit                   = this.transform.parent.GetComponent<GameUnitBase>();
            _renderer                   = this.GetComponent<SpriteRenderer>();
            _material                   = _renderer.material;
            _renderer.shadowCastingMode = ShadowCastingMode.On;
            _rigidbody                  = this.transform.parent.GetComponent<Rigidbody>();
            _aiPath                     = this.transform.parent.GetComponent<GameAIUnitPath>();
        }

        private void Update()
        {
            try
            {
                var v = this._aiPath.steeringTarget - this.transform.parent.transform.position;
                if(Mathf.Abs(v.z) < .1)return;
                this.transform.right = Vector3.forward * Math.Sign(v.z);
            }
            catch (Exception e)
            {
                // not a mover
                if(Mathf.Abs(this._rigidbody.velocity.z) < .1)return;
                this.transform.right            = Vector3.forward * Math.Sign(this._rigidbody.velocity.z);
                this.transform.localEulerAngles = new Vector3(0, 0, this.transform.localEulerAngles.z);

            }
        }

        public void ChangeColor()
        {
            _material.SetFloat("_H",_gameUnit.UnitTeam switch
                                    {
                                        Team.Blue => 0f,
                                        Team.Red  => 0.395f,
                                        _         => throw new ArgumentOutOfRangeException()
                                    });
        }
    }
}
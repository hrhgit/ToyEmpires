using System;
using Gameplay;
using Gameplay.GameUnit;
using Gameplay.GameUnit.FortificationUnit;
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
        private Vector3        _lastV;
        private Camera         _camera;
        [ExecuteAlways]
        private void Awake()
        {
            _gameUnit                   = this.transform.parent.GetComponent<GameUnitBase>();
            _renderer                   = this.GetComponent<SpriteRenderer>();
            _material                   = _renderer.material;
            _renderer.shadowCastingMode = ShadowCastingMode.On;
            _rigidbody                  = this.transform.parent.GetComponent<Rigidbody>();
            _aiPath                     = this.transform.parent.GetComponent<GameAIUnitPath>();
            _camera                     = Camera.main;
        }

        private void Start()
        {
            try
            {
                _lastV = this._aiPath.steeringTarget - this.transform.parent.transform.position;

            }
            catch
            {
                // ignored
            }
        }


        private void Update()
        {
            try
            {
                var v = this._aiPath.steeringTarget - this.transform.parent.transform.position;
                if(Mathf.Abs(v .z) > .001)
                {
                    _lastV = v;
                }
                this.transform.right = Vector3.forward * Mathf.Sign(_lastV .z);
            }
            catch (Exception e)
            {
                // not a mover
                try
                {
                    FortificationUnitBase f = (FortificationUnitBase)_gameUnit;
                    this.transform.forward = f.UnitTeam switch
                                           {
                                               Team.Blue => Vector3.right,
                                               Team.Red  => -Vector3.right,
                                               _         => throw new ArgumentOutOfRangeException()
                                           };
                }
                catch (Exception exception)
                {
                    // not a fortification
                    this.transform.forward = -_camera.transform.position;
                    // if(Mathf.Abs(this._rigidbody.velocity.z) < .1)return;
                    // this.transform.right            = this.transform.parent.forward * -Mathf.Sign(this._rigidbody.velocity.z);
                    this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, -180 - this.transform.parent.localEulerAngles.y );
                }
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
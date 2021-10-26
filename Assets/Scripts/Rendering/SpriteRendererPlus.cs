using System;
using Gameplay;
using Gameplay.GameUnit;
using Gameplay.GameUnit.FortificationUnit;
using Gameplay.GameUnit.ThrowingObject;
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
            _gameUnit                   = transform.parent.GetComponent<GameUnitBase>();
            _renderer                   = GetComponent<SpriteRenderer>();
            _material                   = _renderer.material;
            _renderer.shadowCastingMode = ShadowCastingMode.On;
            _rigidbody                  = transform.parent.GetComponent<Rigidbody>();
            _aiPath                     = transform.parent.GetComponent<GameAIUnitPath>();
            _camera                     = Camera.main;
        }

        private void Start()
        {
            try
            {
                _lastV = _aiPath.steeringTarget - transform.parent.transform.position;
                AimDirection();
            }
            catch
            {
                // ignored
            }
        }


        private void Update()
        {
            AimDirection();
        }

        private void AimDirection()
        {
            if (_aiPath != null)
            {
                var v                             = _aiPath.steeringTarget - transform.parent.transform.position;
                if (Mathf.Abs(v.z) > .001) _lastV = v;

                transform.right = Vector3.forward * Mathf.Sign(_lastV.z);
                return;
            }

            if (_gameUnit is FortificationUnitBase)
            {
                var f = (FortificationUnitBase)_gameUnit;
                transform.forward = f.UnitTeam switch
                                    {
                                        Team.Blue => Vector3.right,
                                        Team.Red  => -Vector3.right,
                                        _         => throw new ArgumentOutOfRangeException()
                                    };
                return;
            }

            if (_gameUnit is ThrowingObjectBase)
            {
                // transform.forward = -_camera.transform.position;
                // transform.right            = _gameUnit.transform.forward;
                // Vector3 viewDir = (this.transform.position - Camera.main.transform.position).normalized;
                // float   angle   = Vector3.
                // this.transform.Rotate(this.transform.forward,-angle);
                // transform.LookAt(Camera.main.transform);
                transform.forward = _camera.transform.forward;
                Vector3 v     = Vector3.Project(_camera.transform.forward, _gameUnit.transform.forward);
                float   angle = Vector3.Angle(v, transform.right);
                transform.Rotate(Vector3.forward, -angle);

                // transform.localEulerAngles = new Vector3(60, transform.localEulerAngles.y, 0);
                // transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -180 - transform.parent.localEulerAngles.y);
            }
        }

        public void ChangeColor()
        {
            _material.SetFloat("_H", _gameUnit.UnitTeam switch
                                     {
                                         Team.Blue => 0f,
                                         Team.Red  => 0.395f,
                                         _         => throw new ArgumentOutOfRangeException()
                                     });
        }
    }
}
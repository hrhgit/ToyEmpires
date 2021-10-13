using System;
using System.Collections;
using Pathfinding;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameUnit
{
   public class GameUnitMover : MonoBehaviour
   {
      
      public  UnityEvent     targetReachedEvent = new UnityEvent();
      
      private GameAIUnitPath _pathFinder;
      private Seeker         _seeker;
      private float          _maxSpeed;
      private GameUnitBase   _unitBase;
      private Vector3        _target;
      public Vector3 Target
      {
         get => _target;
         set
         {
            _target = value;
            this.Goto(_target);
         }
      }

      private void Start()
      {
         this._pathFinder          = this.GetComponent<GameAIUnitPath>();
         this._unitBase            = this.GetComponent<GameUnitBase>();
         this._seeker              = this.GetComponent<Seeker>();
         this._maxSpeed            = this._unitBase.maxSpeed;
         this._pathFinder.maxSpeed = _maxSpeed;
         this._pathFinder.OnTargetReachedEvent.AddListener(ReachTarget);

         switch (_unitBase.UnitTeam)
         {
            case Team.Blue:
               this._seeker.traversableTags = ~((1 << 1) | (1 << 2));
               break;
            case Team.Red:
               this._seeker.traversableTags = ~((1 << 3) | (1 << 4));
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
      
      

      private void Goto(Vector3 target)
      {
         this._pathFinder.destination = target;
      }

      private void ReachTarget()
      {
         targetReachedEvent.Invoke();
      }

   }
}

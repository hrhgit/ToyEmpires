using System.Linq;
using Gameplay.GameUnit;
using Gameplay.GameUnit.SoldierUnit;

namespace Gameplay.Player.AI
{
    public partial class PlayerAI
    {
        public struct UnitTmpData
        {
            public int             playerListidx ;
            public SoldierUnitBase unit          ;
            public IProduceable    produceable   ;
            public int             foodwoodMax   ;
            public int             goldMax       ;
            public UnitStatus      status        ;

            public UnitTmpData(int playerListidx, SoldierUnitBase unit, int foodwoodMax, int goldMax, UnitStatus status)
            {
                this.playerListidx = playerListidx;
                this.unit          = unit;
                this.produceable   = (IProduceable)unit;
                this.foodwoodMax   = foodwoodMax;
                this.goldMax       = goldMax;
                this.status        = status;
            }

            public override string ToString()
            {
                return "[Idx:"             + playerListidx +"\n"+
                       ", unit          :" + unit          +"\n"+
                       ", produceable   :" + produceable   +"\n"+
                       ", foodwoodMax   :" + foodwoodMax   +"\n"+
                       ", goldMax       :" + goldMax       +"\n"+
                       ", status        :" + status        + "]";
            }
        }
        
        public struct DispatchPlan
        {
            public int   playerListIdx;
            public int   count;

            public DispatchPlan(int playerListIdx, int count)
            {
                this.playerListIdx = playerListIdx;
                this.count         = count;
            }

            public override string ToString()
            {
                return "[Idx:" + playerListIdx + ", Count:" + count + "]";
            }
        }

        public struct DispatchPlanDict
        {
            public DispatchPlan[] plans;
            public float          strength;

            public DispatchPlanDict(DispatchPlan[] plans, float strength = 0)
            {
                this.plans    = plans;
                this.strength = strength;
            }

            public override string ToString()
            {
                return "{Strength:" + strength + ", Plans:" + plans.Aggregate("", ((s, plan) => s + plan.ToString())) + "}";
            }
        }
    }
}
using System.Collections.Generic;
using Gameplay.Buff;

namespace Gameplay.TechTree
{
    public delegate Technology GenerateTech();
    public static class TechGenerator
    {
        private static readonly Dictionary<int, GenerateTech> _techDict = new Dictionary<int, GenerateTech>
                                                                            {
                                                                                {0,(() =>
                                                                                    {
                                                                                        return new Technology(5,5,3,2,2,1,5)
                                                                                               {
                                                                                                   technologyID = 0,
                                                                                                   playerBuffs  = null,
                                                                                                   unitBuffs    = null,
                                                                                               };
                                                                                    })},
                                                                                {1,(() =>
                                                                                    {
                                                                                        return new Technology(5,5,3,2,2,1,5)
                                                                                               {
                                                                                                   technologyID = 1,
                                                                                                   playerBuffs  = null,
                                                                                                   unitBuffs    = null,
                                                                                               };
                                                                                    })},
                                                                                {2,(() =>
                                                                                    {
                                                                                        return new Technology(5,5,3,2,2,1,5)
                                                                                               {
                                                                                                   technologyID = 2,
                                                                                                   playerBuffs  = null,
                                                                                                   unitBuffs    = null,
                                                                                               };
                                                                                    })},
                                                                            };

        public static Technology GenerateTechnology(int techID)
        {
            return _techDict[techID]();
        }
    }
}
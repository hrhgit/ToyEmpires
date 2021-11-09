using System.Collections.Generic;
using Gameplay.Buff;
using UnityEngine;

namespace Gameplay.TechTree
{
    public delegate TechTreeNodeList GenerateTechTreeNodeListFunc(TechTree techTree);
    public static class TechTreeGenerator
    {
        private static readonly Dictionary<int, GenerateTechTreeNodeListFunc> _techTreeNodeListDict = new Dictionary<int, GenerateTechTreeNodeListFunc>
                                                                                  {
                                                                                      {0,((techTree) =>
                                                                                          {
                                                                                              return new TechTreeNodeList(new TechTreeNodeStat[]
                                                                                                                          {
                                                                                                                              new TechTreeNodeStat(techTree,
                                                                                                                                                   TechGenerator.GenerateTechnology(0)
                                                                                                                                                  ),
                                                                                                                              new TechTreeNodeStat(techTree,
                                                                                                                                                   new[] { 0 },
                                                                                                                                                   TechGenerator.GenerateTechnology(1)
                                                                                                                                                  ),
                                                                                                                              new TechTreeNodeStat(techTree,
                                                                                                                                                   new[] { 0 },
                                                                                                                                                   TechGenerator.GenerateTechnology(2)
                                                                                                                                                  ),
                                                                                                                              new TechTreeNodeStat(techTree,
                                                                                                                                                   new[] { 1 },
                                                                                                                                                   TechGenerator.GenerateTechnology(0)
                                                                                                                                                  ),
                                                                                                                              new TechTreeNodeStat(techTree,
                                                                                                                                                   new[] { 1 },
                                                                                                                                                   TechGenerator.GenerateTechnology(1)
                                                                                                                                                  ),
                                                                                                                              new TechTreeNodeStat(techTree,
                                                                                                                                                   new[] { 1, 2 },
                                                                                                                                                   TechGenerator.GenerateTechnology(3)
                                                                                                                                                  ),

                                                                                                                          });
                                                                                          })}
                                                                                  };

        public static TechTreeNodeList GenerateTechTreeNodeList(int techTreeID, TechTree techTree)
        {
            return _techTreeNodeListDict[techTreeID](techTree);
        }
    }
}
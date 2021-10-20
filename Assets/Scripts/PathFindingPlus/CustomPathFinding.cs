using UnityEngine;

namespace PathFindingPlus
{
    public class CustomPathFinding : MonoBehaviour
    {
        public AstarPath astarPath;

        public void Refresh()
        {
            var scanAsync = astarPath.ScanAsync();
        }
    }
}

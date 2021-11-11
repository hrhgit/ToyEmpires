using UnityEngine;

namespace Global
{
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager GlobalGameManagerInstance { get; private set; }
        
        public int playerCivilizationIdx;
        public int enemyCivilizationIdx;
        private void Awake()
        {
            GlobalGameManagerInstance = this;
        }
    }
}

namespace Gameplay.GameUnit
{
    public static class GameUnitHelper
    {
        public static Road ToRoad(this ResourceType r)
        {
            return new Road[] {Road.Top, Road.Mid, Road.Bottom}[(int)r];
        }
        
        public static ResourceType ToResourceType(this Road r)
        {
            return new ResourceType[] {ResourceType.Food,ResourceType.Gold,ResourceType.Wood}[(int)r];
        }
    }
}

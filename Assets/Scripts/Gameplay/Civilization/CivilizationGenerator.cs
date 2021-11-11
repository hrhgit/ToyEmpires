using System.Collections.Generic;
namespace Gameplay.Civilization {
	public delegate CivilizationBase GenerateCivilizationFunc();
	public static  class CivilizationGenerator {
		private static readonly Dictionary<int, GenerateCivilizationFunc> _civDict = new Dictionary<int, GenerateCivilizationFunc>
		                                                                             {
			                                                                             {0,(() => new CivilizationBase(0, 0)
			                                                                                       {
				                                                                                       availableUnitIds = new List<int>()
				                                                                                                          {
					                                                                                                          2000,
					                                                                                                          3000
				                                                                                                          },
				                                                                                       availablePoliciesIds = new List<int>()
				                                                                                                              {
					                                                                                                              2000,
						                                                                                                          2001
				                                                                                                              }
			                                                                                       })}
		                                                                             };
		
		public static CivilizationBase GenerateCivilization(int civID)
		{
			return _civDict[civID]();
		}
	}
}

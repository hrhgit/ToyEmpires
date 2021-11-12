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
			                                                                                       })},
			                                                                             
			                                                                             //充数的
			                                                                             {1,(() => new CivilizationBase(1, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {2,(() => new CivilizationBase(2, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {3,(() => new CivilizationBase(3, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {4,(() => new CivilizationBase(4, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {5,(() => new CivilizationBase(5, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {6,(() => new CivilizationBase(6, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {7,(() => new CivilizationBase(7, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {8,(() => new CivilizationBase(8, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {9,(() => new CivilizationBase(9, 0)
			                                                                                       {
			                                                                                       })},
			                                                                             {10,(() => new CivilizationBase(10, 0)
			                                                                                       {
			                                                                                       })},

		                                                                             };
		
		public static Dictionary<int, GenerateCivilizationFunc> CivDict => _civDict;

		public static CivilizationBase GenerateCivilization(int civID)
		{
			return CivDict[civID]();
		}
	}
}

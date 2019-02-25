using UnityEngine;

namespace Assets.Scripts.Refugees
{
    public class RefugeesSettings : MonoBehaviour
    {
        public int HungerProbability = 50;
        public int ThirstProbability = 60;
        public int ColdProbability = 40;
        public int IllnessProbability = 20;
        public int HungerItemProbability = 40;
        public int ThirstItemProbability = 40;
        public int ColdItemProbability = 30;
        public int IllnessItemProbability = 10;
        public int HungerResolvedPoints = 5;
        public int ThirstResolvedPoints = 5;
        public int ColdResolvedPoints = 4;
        public int IllnessResolvedPoints = 10;
        public int MaxDaysToStay = 4;
        public int NostalgiaResolvedPoints = 12;
        public int RandomObjectPoints = 2;
        public string[] ValidSortingLayers;
    }
}

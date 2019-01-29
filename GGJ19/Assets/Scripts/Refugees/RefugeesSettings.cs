﻿using UnityEngine;

namespace Assets.Scripts.Refugees
{
    public class RefugeesSettings : MonoBehaviour
    {
        public int IllnessProbability = 20;
        public int HungerResolvedPoints = 5;
        public int ThirstResolvedPoints = 5;
        public int IllnessResolvedPoints = 10;
        public int MaxDaysToStay = 4;
        public int NostalgiaResolvedPoints = 12;
        public string[] ValidSortingLayers;
    }
}

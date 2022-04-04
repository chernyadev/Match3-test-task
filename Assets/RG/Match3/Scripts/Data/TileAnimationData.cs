using System;
using UnityEngine;

namespace RG.Match3.Data {
    [Serializable]
    public struct TileAnimationData {
        public float removeDuration;
        public int removeVibration;
        public float removeElasticity;

        [Space]
        public float moveDuration;
    }
}
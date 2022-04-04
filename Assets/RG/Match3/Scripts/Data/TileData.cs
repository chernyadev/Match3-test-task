using System;
using UnityEngine;

namespace RG.Match3.Data {
    [Serializable]
    public struct TileData {
        public int id;
        public float size;
        public GameObject prefab;
    }
}
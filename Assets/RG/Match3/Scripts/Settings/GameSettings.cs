using UnityEngine;

namespace RG.Match3.Settings {
    [CreateAssetMenu(menuName = "Create Game Settings", fileName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject {
        [SerializeField]
        private int matchCount = 3;
        public int MatchCount => matchCount;
    }
}
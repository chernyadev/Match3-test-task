using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RG.Match3.Settings {
    [CreateAssetMenu(menuName = "Create Tiles Library", fileName = "TilesLibrary", order = 0)]
    public class TilesLibrary : ScriptableObject {
        [Space]
        [SerializeField]
        private Tile[] tiles;

        private Dictionary<int, Tile> tilesDict = new Dictionary<int, Tile>();

        #region Unity

        private void OnValidate() {
            if (!ValidateIds()) {
                return;
            }

            tilesDict = CreateTilesDictionary();
        }

        #endregion

        public Tile GetRandomTile() {
            return tiles[Random.Range(0, tiles.Length)];
        }

        public Tile GetRandomTileExcludingIds(int[] excludeIds) {
            var filteredTiles = new Dictionary<int, Tile>(tilesDict);

            for (var i = 0; i < excludeIds.Length; i++) {
                filteredTiles.Remove(excludeIds[i]);
            }

            return filteredTiles.Values.ToArray()[Random.Range(0, filteredTiles.Count)];
        }

        private bool ValidateIds() {
            var ids = new HashSet<int>();
            for (var i = 0; i < tiles.Length; i++) {
                var id = tiles[i].Id;
                if (ids.Contains(id)) {
                    Debug.LogError($"Tile ID {id} is duplicated. Tiles must have unique IDs. Fix this!");
                    return false;
                }
                else {
                    ids.Add(id);
                }
            }

            return true;
        }

        private Dictionary<int, Tile> CreateTilesDictionary() {
            var dict = new Dictionary<int, Tile>();

            for (var i = 0; i < tiles.Length; i++) {
                var tile = tiles[i];
                dict[tile.Id] = tile;
            }

            return dict;
        }
    }
}
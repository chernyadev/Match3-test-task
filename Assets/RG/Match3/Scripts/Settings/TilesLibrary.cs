using System.Collections.Generic;
using System.Linq;
using RG.Match3.Data;
using UnityEngine;

namespace RG.Match3.Settings {
    [CreateAssetMenu(menuName = "Create Tiles Library", fileName = "TilesLibrary", order = 0)]
    public class TilesLibrary : ScriptableObject {
        [SerializeField]
        private TileData[] tiles;

        [SerializeField]
        private TileAnimationData sharedAnimationData;

        private Dictionary<int, TileData> tilesDict = new Dictionary<int, TileData>();

        #region Unity

        private void OnValidate() {
            if (!ValidateIds()) {
                return;
            }

            tilesDict = CreateTilesDictionary();
        }

        #endregion

        public Tile GetRandomTile() {
            var data = tiles[Random.Range(0, tiles.Length)];
            return BuildTile(data);
        }

        public Tile GetRandomTileExcludingIds(int[] excludeIds) {
            var filteredTiles = new Dictionary<int, TileData>(tilesDict);

            for (var i = 0; i < excludeIds.Length; i++) {
                filteredTiles.Remove(excludeIds[i]);
            }

            var data = filteredTiles.Values.ToArray()[Random.Range(0, filteredTiles.Count)];
            return BuildTile(data);
        }

        private Tile BuildTile(TileData tileData) {
            var tile = Instantiate(tileData.prefab).AddComponent<Tile>();
            tile.Init(tileData, sharedAnimationData);
            return tile;
        }

        private bool ValidateIds() {
            var ids = new HashSet<int>();
            for (var i = 0; i < tiles.Length; i++) {
                var id = tiles[i].id;
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

        private Dictionary<int, TileData> CreateTilesDictionary() {
            var dict = new Dictionary<int, TileData>();

            for (var i = 0; i < tiles.Length; i++) {
                var tileData = tiles[i];
                dict[tileData.id] = tileData;
            }

            return dict;
        }
    }
}
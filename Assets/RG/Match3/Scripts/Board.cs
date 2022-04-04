using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RG.Match3 {
    public class Board : MonoBehaviour {
        [SerializeField]
        private Vector2Int size;
        [SerializeField]
        private int matchCount = 3;

        [Space]
        [SerializeField]
        private Tile[] tilesLibrary;

        private Tile[,] board;

        private bool isHandlingTileClick;

        #region Unity

        private void Start() {
            Fill();
        }

        private void OnValidate() {
            var ids = new HashSet<int>();

            for (var i = 0; i < tilesLibrary.Length; i++) {
                var id = tilesLibrary[i].Id;
                if (ids.Contains(id)) {
                    Debug.LogError($"Tile ID {id} is duplicated. Tiles must have unique IDs. Fix this!");
                }
                else {
                    ids.Add(id);
                }
            }
        }

        #endregion

        private async void OnTileClickedHandler(Tile tileToRemove) {
            if (isHandlingTileClick) {
                return;
            }

            isHandlingTileClick = true;

            RemoveTile(tileToRemove);
            await ShiftDown();

            while (CheckMatches() > 0) {
                await ShiftDown();
            }

            isHandlingTileClick = false;
        }

        private void RemoveTile(Tile tile) {
            var tileCoords = tile.BoardCoords;
            board[tileCoords.x, tileCoords.y] = null;
            tile.Remove();
        }

        private async Task ShiftDown() {
            Task moveTask = null;

            for (var y = 1; y < size.y; y++) {
                for (var x = 0; x < size.x; x++) {
                    var curTile = board[x, y];

                    if (curTile == null) {
                        continue;
                    }

                    var offset = 0;

                    do {
                        offset++;
                    } while (y - offset >= 0 && board[x, y - offset] == null);

                    if (board[x, y - offset + 1] != null) continue;

                    board[x, y] = null;
                    board[x, y - offset + 1] = curTile;
                    moveTask = curTile.Move(new Vector2Int(x, y - offset + 1));
                }
            }

            if (moveTask != null) {
                await moveTask;
            }
        }

        private int CheckMatches() {
            while (true) {
                var tilesToRemove = new List<Tile>();

                for (var y = 0; y < size.y; y++) {
                    var sequence = new HashSet<Tile>();
                    for (var x = 0; x < size.x; x++) {
                        var curTile = board[x, y];

                        if (curTile != null && sequence.Count == 0) {
                            sequence.Add(curTile);
                            continue;
                        }

                        if (curTile != null && curTile.Id == sequence.Last().Id) {
                            sequence.Add(curTile);
                        }
                        else {
                            if (sequence.Count >= matchCount) {
                                tilesToRemove.AddRange(sequence);
                            }

                            sequence = new HashSet<Tile>();

                            if (curTile != null) {
                                sequence.Add(curTile);
                            }
                        }
                    }

                    if (sequence.Count >= matchCount) {
                        tilesToRemove.AddRange(sequence);
                    }
                }

                for (var i = 0; i < tilesToRemove.Count; i++) {
                    RemoveTile(tilesToRemove[i]);
                }

                return tilesToRemove.Count;
            }
        }

        private void Fill() {
            board = new Tile[size.x, size.y];
            
            var tilesRoot = new GameObject("TilesRoot").transform;
            tilesRoot.SetParent(transform);
            tilesRoot.localPosition = new Vector2(size.x, size.y) * -0.5f;

            for (var y = 0; y < size.y; y++) {
                var prevTileId = int.MinValue;
                var repeatedTilesCount = 1;

                for (var x = 0; x < size.x; x++) {
                    var tileId = Random.Range(0, tilesLibrary.Length);

                    if (tileId == prevTileId) {
                        repeatedTilesCount++;
                    }
                    else {
                        repeatedTilesCount = 1;
                    }

                    if (repeatedTilesCount >= matchCount) {
                        var availableIds = Enumerable.Range(0, tilesLibrary.Length).ToList();
                        availableIds.Remove(tileId);
                        tileId = availableIds[Random.Range(0, availableIds.Count)];
                    }

                    var tile = Instantiate(tilesLibrary[tileId], tilesRoot);
                    tile.onTileClicked += OnTileClickedHandler;
                    tile.BoardCoords = new Vector2Int(x, y);
                    board[x, y] = tile;

                    prevTileId = tileId;
                }
            }
        }
    }
}
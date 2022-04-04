using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RG.Match3.Settings;
using UnityEngine;

namespace RG.Match3 {
    public class Board : MonoBehaviour {
        [SerializeField]
        private Vector2Int size;

        [Space]
        [Header("Configs")]
        [SerializeField]
        private GameSettings gameSettings;
        [SerializeField]
        private TilesLibrary tilesLibrary;

        private Tile[,] board;

        private bool isHandlingTileClick;

        #region Unity

        private void Start() {
            Fill();
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
                            if (sequence.Count >= gameSettings.MatchCount) {
                                tilesToRemove.AddRange(sequence);
                            }

                            sequence = new HashSet<Tile>();

                            if (curTile != null) {
                                sequence.Add(curTile);
                            }
                        }
                    }

                    if (sequence.Count >= gameSettings.MatchCount) {
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
                    var nextTile = repeatedTilesCount == gameSettings.MatchCount - 1
                        ? tilesLibrary.GetRandomTileExcludingIds(new[] {prevTileId})
                        : tilesLibrary.GetRandomTile();

                    if (nextTile.Id == prevTileId) {
                        repeatedTilesCount++;
                    }
                    else {
                        repeatedTilesCount = 1;
                    }

                    var tile = Instantiate(nextTile, tilesRoot);
                    tile.onTileClicked += OnTileClickedHandler;
                    tile.BoardCoords = new Vector2Int(x, y);
                    board[x, y] = tile;

                    prevTileId = nextTile.Id;
                }
            }
        }
    }
}
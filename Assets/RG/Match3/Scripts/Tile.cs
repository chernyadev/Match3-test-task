using System;
using System.Threading.Tasks;
using DG.Tweening;
using RG.Match3.Data;
using UnityEngine;

namespace RG.Match3 {
    public class Tile : MonoBehaviour {
        public event Action<Tile> onTileClicked;

        private TileData data;
        private TileAnimationData animationData;

        private Vector2Int boardCoords = Vector2Int.zero;
        public Vector2Int BoardCoords {
            get => boardCoords;
            set {
                boardCoords = value;
                transform.localPosition = BoardCoordsToWorldSpacePosition(boardCoords);
            }
        }
        public int Id => data.id;

        private void OnMouseDown() {
            onTileClicked?.Invoke(this);
        }

        public void Init(TileData data, TileAnimationData animationData) {
            this.data = data;
            this.animationData = animationData;
        }

        public void Remove() {
            transform
                .DOPunchScale(Vector3.one, animationData.removeDuration, animationData.removeVibration,
                    animationData.removeElasticity)
                .OnComplete(() => Destroy(gameObject));
        }

        public Task Move(Vector2Int targetCoords) {
            boardCoords = targetCoords;
            return transform.DOLocalMove(BoardCoordsToWorldSpacePosition(targetCoords), animationData.moveDuration)
                .SetEase(Ease.OutBounce)
                .AsyncWaitForCompletion();
        }

        private Vector3 BoardCoordsToWorldSpacePosition(Vector2 coord) {
            return new Vector2(coord.x, coord.y) * data.size + Vector2.one * (data.size * 0.5f);
        }
    }
}
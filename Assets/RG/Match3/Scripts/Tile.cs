using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace RG.Match3 {
    public class Tile : MonoBehaviour {
        public event Action<Tile> onTileClicked;

        [SerializeField]
        private int id;

        [SerializeField]
        private float size = 1f;

        [Header("Animations")]
        [SerializeField]
        private float removeAnimationDuration = 0.2f;
        [SerializeField]
        private int removeAnimationVibration = 10;
        [SerializeField]
        private float removeAnimationElasticity = 0.1f;
        [SerializeField]
        private float moveAnimationDuration = 0.2f;

        private Vector2Int boardCoords = Vector2Int.zero;
        public Vector2Int BoardCoords {
            get => boardCoords;
            set {
                boardCoords = value;
                transform.localPosition = BoardCoordsToWorldSpacePosition(boardCoords);
            }
        }
        public int Id => id;

        private void OnMouseDown() {
            onTileClicked?.Invoke(this);
        }

        public void Remove() {
            transform
                .DOPunchScale(Vector3.one, removeAnimationDuration, removeAnimationVibration, removeAnimationElasticity)
                .OnComplete(() => Destroy(gameObject));
        }

        public Task Move(Vector2Int targetCoords) {
            boardCoords = targetCoords;
            return transform.DOLocalMove(BoardCoordsToWorldSpacePosition(targetCoords), moveAnimationDuration)
                .SetEase(Ease.OutBounce)
                .AsyncWaitForCompletion();
        }

        private Vector3 BoardCoordsToWorldSpacePosition(Vector2 coord) {
            return new Vector2(coord.x, coord.y) * size + Vector2.one * (size * 0.5f);
        }
    }
}
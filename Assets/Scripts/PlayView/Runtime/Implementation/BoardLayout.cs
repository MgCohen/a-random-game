using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.PlayView
{
    public class BoardLayout : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayoutGroup;

        private void Awake()
        {
            if (gridLayoutGroup == null)
                gridLayoutGroup = GetComponent<GridLayoutGroup>();
        }

        public void Configure(int rows, int columns)
        {
            if (gridLayoutGroup == null || rows <= 0 || columns <= 0) return;

            SetConstraint(columns);
            float cellSide = ComputeSquareCellSize(rows, columns);
            ApplyCellSize(cellSide);
        }

        private void SetConstraint(int columns)
        {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;
        }

        private float ComputeSquareCellSize(int rows, int columns)
        {
            RectTransform rectTransform = gridLayoutGroup.GetComponent<RectTransform>();
            if (rectTransform == null) return 0f;

            RefreshLayout(rectTransform);
            Rect rect = rectTransform.rect;
            Vector2 available = GetAvailableSize(rect, rows, columns);

            return GetSquareCellSide(available, rows, columns);
        }

        private static void RefreshLayout(RectTransform rectTransform)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private Vector2 GetAvailableSize(Rect rect, int rows, int columns)
        {
            RectOffset padding = gridLayoutGroup.padding;
            Vector2 spacing = gridLayoutGroup.spacing;
            float width = rect.width - padding.horizontal - (columns - 1) * spacing.x;
            float height = rect.height - padding.vertical - (rows - 1) * spacing.y;
            return new Vector2(width, height);
        }

        private static float GetSquareCellSide(Vector2 availableSize, int rows, int columns)
        {
            float cellWidth = columns > 0 ? availableSize.x / columns : 0f;
            float cellHeight = rows > 0 ? availableSize.y / rows : 0f;
            return Mathf.Max(0f, Mathf.Min(cellWidth, cellHeight));
        }

        private void ApplyCellSize(float cellSide)
        {
            gridLayoutGroup.cellSize = new Vector2(cellSide, cellSide);
        }
    }
}

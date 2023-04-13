using System;
using UnityEngine;
using CollapseBlast.Enums;
using System.Collections.Generic;
using System.Linq;
using CollapseBlast.Manager;

namespace CollapseBlast
{
    public class Board : MonoBehaviour
    {
        public Cell CellPrefab;
        public Transform CellsParent;
        public Transform ItemsParent;
        public SpriteRenderer BoardOutsideSprite;
        public SpriteRenderer BoardInsideSprite;
        public SpriteMask BoardMask;
        [HideInInspector] public List<Cell> Cells;
        MatchFinder _matchFinder;

        private int _columns, _rows;

        public void Init()
        {
            _columns = GameManager.Instance.Level.Columns;
            _rows = GameManager.Instance.Level.Rows;
            _matchFinder = new MatchFinder();

            CreateCells();
            InitCells();
            ArrangeBoardPosition();
            ArrangeBoardScale();
        }

        void ArrangeBoardPosition()
        {
            var xPos = (_columns / -2f) + .5f;
            var yPos = (_rows / -2f) + .5f;
            transform.localPosition = new Vector2(xPos, yPos);
        }

        void ArrangeBoardScale()
        {
            var camera = Camera.main;
            float aspect = camera.aspect;
            float worldHeight = camera.orthographicSize * 2;
            float worldWidth = worldHeight * aspect;
            var columnUnitWidth = worldWidth / _columns;

            BoardOutsideSprite.transform.localScale = new Vector2(columnUnitWidth, columnUnitWidth);

            var ItemEdgeUnit = Screen.width / _columns; 
            float boardHeight = ItemEdgeUnit * _rows;
            float boardWidth = ItemEdgeUnit * _columns;
            while (boardHeight > Screen.height * .48f || boardWidth > Screen.width * .9f)
            {
                var scale = BoardOutsideSprite.transform.localScale.x;
                BoardOutsideSprite.transform.localScale = new Vector2(scale - scale * .05f, scale - scale * .05f);
                boardHeight -= (boardHeight * .05f);
                boardWidth -= (boardWidth * .05f);
            }

            BoardOutsideSprite.size = new Vector2(_columns + .8f, _rows + .8f);
            BoardInsideSprite.size = new Vector2(_columns + .5f, _rows + .5f);
            var boardEdgeOffset = .15f;
            BoardMask.transform.localScale = BoardInsideSprite.size - new Vector2(boardEdgeOffset, boardEdgeOffset);
        }

        private void CreateCells()
        {
            var i = 0;
            for (var y = 0; y < _rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, CellsParent);
                    cell.X = x;
                    cell.Y = y;
                    Cells.Add(cell);
                    i++;
                }
            }
        }

        private void InitCells()
        {
            var i = 0;
            for (var y = 0; y < _rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    Cells[i].Init(x, y);
                    i++;
                }
            }
        }

        public void ClearElements()
        {
            foreach (var cell in Cells)
            {
                if(cell.Item != null) Destroy(cell.Item.gameObject);
                Destroy(cell.gameObject);
            }
            Cells.Clear();
        }

        public void ClearItems()
        {
            foreach (var cell in Cells)
            {
                if (cell.Item != null) Destroy(cell.Item.gameObject);
            }
        }

        public void CellTapped(Cell cell)
        {
            if (cell == null || cell.Item == null) return;

            var tappedCellIsBooster = cell.Item.IsBooster;
            var tappedCellTypeIndex = cell.Item.TypeIndex;
            DestroyMatchedItems(cell);

            if (!tappedCellIsBooster && tappedCellTypeIndex > 0) //create booster
            {
                cell.Item = GameManager.Instance.ItemManager.CreateItem(ItemType.Booster, cell.transform.localPosition, tappedCellTypeIndex - 1);
            }
        }

        private void DestroyMatchedItems(Cell cell)
        {
            var itemType = cell.Item.ItemType;
            if (itemType == ItemType.Booster)
            {
                return;
            }

            var partOfMatchedCells = _matchFinder.FindMatch(cell, itemType);

            if (partOfMatchedCells == null) return;

            GameManager.Instance.Level.UpdateLevelStats(itemType, partOfMatchedCells.Count);

            foreach (var matchedCell in partOfMatchedCells)
            {
                matchedCell.Item.Destroy();
            }
        }

        public Cell GetNeighbourWithDirection(Cell cell, Direction direction)
        {
            var x = cell.X;
            var y = cell.Y;

            switch (direction)
            {
                case Direction.Up:
                    y += 1;
                    break;
                case Direction.Down:
                    y -= 1;
                    break;
                case Direction.Right:
                    x += 1;
                    break;                
                case Direction.Left:
                    x -= 1;
                    break;
            }

            if (x >= _columns || x < 0 || y >= _rows || y < 0) return null;

            return GetCell(x, y);
        }

        Cell GetCell(int x, int y)
        {
            return Cells.Single(cell => cell.X == x && cell.Y == y);
        }
    }
}

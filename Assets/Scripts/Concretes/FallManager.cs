using CollapseBlast.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollapseBlast.Manager
{
    public class FallManager
    {
        private int _rows, _columns;
        private ItemManager _itemManager;
        private Board _board;
        private List<Cell> _topRowCells;
        private List<ItemType> _itemTypes;

        public void Init()
        {
            var gameManager = GameManager.Instance;
            _itemManager = gameManager.ItemManager;
            _board = gameManager.Board;
            _rows = gameManager.Level.Rows;
            _columns = gameManager.Level.Columns;
            _itemTypes = gameManager.Level.CurrentLevelData.ItemTypes;
            FindTopRowCells();
        }

        void FindTopRowCells()
        {
            _topRowCells = _board.Cells.Where(cell => cell.Y == GameManager.Instance.Level.Rows - 1).ToList();
        }

        private void FallExistentItems()
        {
            var i = 0;
            for (var y = 0; y < _rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    var cell = _board.Cells[i];
                    if (cell.Item != null && cell.FallStopPosition != null && cell.FallStopPosition.Item == null)
                    {
                        cell.Item.Fall();
                    }
                    i++;
                }
            }
        }

        private void FallNonExistentItems()
        {
            for (var i = 0; i < _topRowCells.Count; i++)
            {
                var cell = _topRowCells[i];
                if (cell.Item == null)
                {
                    var rndItemType = _itemTypes[Random.Range(0, _itemTypes.Count)];
                    cell.Item = _itemManager.CreateItem(rndItemType);
                    var fallStopPosition = cell.GetFallTargetCell().FallStopPosition;        
                    var offsetY = fallStopPosition != null ? fallStopPosition.Item.transform.localPosition.y + 1 : 0f;
                    var fallStartPosition = cell.transform.localPosition + Vector3.up;
                    fallStartPosition.y = Mathf.Max(fallStartPosition.y, offsetY);
                    cell.Item.transform.localPosition = fallStartPosition;
                    cell.Item.Fall();
                }
            }
        }

        public void TickUpdate()
        {
            FallExistentItems();
            FallNonExistentItems();
        }
    }
}

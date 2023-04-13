using System.Collections.Generic;
using CollapseBlast.Enums;
using CollapseBlast.ScriptableObjects;
using UnityEngine;

namespace CollapseBlast.Manager
{
    public class HintManager
    {
        private int _columns, _rows;
        private Board _board;
        private LevelDataSO _currentLevelData;

        public void Init()
        {
            _board = GameManager.Instance.Board;
            _currentLevelData = GameManager.Instance.Level.CurrentLevelData;
            _columns = _currentLevelData.Columns;
            _rows = _currentLevelData.Rows;
        }

        public void TickUpdate()
        {
            ArrangeItemIcon();
        }

        public void ArrangeItemIcon()
        {
            int rows = _rows;
            int cols = _columns;
            var cells = _board.Cells;

            var matchedCellInfo = GetMatchedCellInfos();

            var i = 0;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var item = cells[i].Item;
                    i++;

                    if (item == null || item.ItemType == ItemType.Booster) continue;

                    item.ArrangeSorting();

                    if (matchedCellInfo[x, y] >= _currentLevelData.ThirdSpecialIconTypeThreshold)
                    {
                        item.TypeIndex = 3;
                    }
                    else if (matchedCellInfo[x, y] >= _currentLevelData.SecondSpecialIconTypeThreshold)
                    {
                        item.TypeIndex = 2;
                    }
                    else if (matchedCellInfo[x, y] >= _currentLevelData.FirstSpecialIconTypeThreshold)
                    {
                        item.TypeIndex = 1;
                    }
                    else
                    {
                        item.TypeIndex = 0;
                    }

                    item.ChangeSprite(item.TypeIndex);
                }
            }
        }

        private int[,] GetMatchedCellInfos()
        {
            int rows = _rows;
            int cols = _columns;
            var cells = _board.Cells;

            var i = 0;
            int[,] matchedCellInfos = new int[cols, rows];
            for (i = 0; i < cols * rows; i++)
            {
                matchedCellInfos[i % cols, i / cols] = -1; // set all matchedCellInfos elements to -1
            }

            i = 0;
            var matchFinder = new MatchFinder();
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var cell = cells[i];
                    if (matchedCellInfos[x, y] == -1 && cell.Item != null)
                    {
                        var partOfMatchedCells = matchFinder.FindMatch(cell, cell.Item.ItemType);
                        FillMatchedCellInfos(partOfMatchedCells, matchedCellInfos);
                    }
                    i++;
                }
            }

            return matchedCellInfos;
        }

        private void FillMatchedCellInfos(List<Cell> partOfMatchedCells, int[,] matchedCellInfos)
        {
            if (partOfMatchedCells == null) return;
            var size = partOfMatchedCells.Count;
            foreach (var cell in partOfMatchedCells)
            {
                matchedCellInfos[cell.X, cell.Y] = size;
            }
        }
    }
}



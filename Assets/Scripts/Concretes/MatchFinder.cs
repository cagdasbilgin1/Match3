using CollapseBlast.Enums;
using CollapseBlast.Manager;
using System.Collections.Generic;

namespace CollapseBlast
{
	public class MatchFinder
	{
        private int _rows, _columns;
		private int _minimumBlastableMatch;
        bool[,] _visitedCells;

		public MatchFinder()
		{
			var level = GameManager.Instance.Level;
            _rows = level.Rows;
            _columns = level.Columns;
			_minimumBlastableMatch = level.MinimumBlastableMatch;
        }

		public List<Cell> FindMatch(Cell cell, ItemType itemType)
		{
            _visitedCells = new bool[_columns, _rows];
            for (int i = 0; i < _columns * _rows; i++)
            {
                _visitedCells[i % _columns, i / _columns] = false; // set all _visitedCell elements to false
            }

            var partOfMatchedCells = new List<Cell>();
			FindMatches(cell, itemType, partOfMatchedCells);
			return partOfMatchedCells.Count >= _minimumBlastableMatch ? partOfMatchedCells : null;
		}

		private void FindMatches(Cell cell, ItemType itemType, List<Cell> resultCells)
		{
			if (cell == null) return;
			
			var x = cell.X;
			var y = cell.Y;
			if (_visitedCells[x, y]) return;
			
			_visitedCells[x, y] = true;

			if (cell.Item != null && cell.Item.ItemType == itemType)
			{
				resultCells.Add(cell);
			
				var neighbours = cell.Neighbours;
				if (neighbours.Count == 0) return;	

				foreach (var neighbour in neighbours)
				{
					FindMatches(neighbour, itemType, resultCells);
				}
			}		
		}
	}
}

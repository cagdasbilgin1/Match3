using CollapseBlast.Enums;
using CollapseBlast.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CollapseBlast.Manager
{
    public class LevelManager : MonoBehaviour
    {
        const string keyLevelIndex = "LevelIndex";
        public List<LevelDataSO> levels;

        int _rows, _columns;
        int _levelIndex;
        int _minimumBlastableMatch;
        int _goalCount;
        int _moveCount;
        LevelDataSO _currentLevelData;
        Board _board;
        ItemManager _itemManager;
        List<ItemType> _itemTypes;
        List<LevelItem> levelItemDatas;        

        public int GoalCount => _goalCount;
        public int MoveCount => _moveCount;
        public int Rows => _rows;
        public int LevelIndex => _levelIndex;
        public int Columns => _columns;
        public int MinimumBlastableMatch => _minimumBlastableMatch;
        public LevelDataSO CurrentLevelData => _currentLevelData;

        public event Action OnLevelUpEvent;
        public event Action OnGameOverEvent;
        public event Action OnLevelStatsUpdateEvent;

        public void Init()
        {
            _levelIndex = PlayerPrefs.GetInt(keyLevelIndex, 0);

            var gameManager = GameManager.Instance;
            _board = gameManager.Board;
            _itemManager = gameManager.ItemManager;
            _currentLevelData = levels[_levelIndex];
            _rows = _currentLevelData.Rows;
            _columns = _currentLevelData.Columns;
            _minimumBlastableMatch = _currentLevelData.MinimumBlastableCell;
            _itemTypes = _currentLevelData.ItemTypes;

            LoadJsonDatas();

            gameManager.metaSceneOpenedEvent += ResetLevel;

            _goalCount = _currentLevelData.GoalCount;
            _moveCount = _currentLevelData.MovesCount;
        }

        public void LevelUp()
        {
            _levelIndex++;
            _levelIndex = _levelIndex < levels.Count ? _levelIndex : 0;
            _currentLevelData = levels[_levelIndex];
            PlayerPrefs.SetInt(keyLevelIndex, _levelIndex);
            OnLevelUpEvent?.Invoke();
        }

        public void GameOver()
        {
            OnGameOverEvent?.Invoke();
        }

        public void ResetLevel()
        {
            _goalCount = _currentLevelData.GoalCount;
            _moveCount = _currentLevelData.MovesCount;

            _board.ClearItems();
            FillBoard();
        }

        public void UpdateLevelStats(ItemType blastedItemsType, int blastedItemCount)
        {
            _moveCount--;
            if (_currentLevelData.GoalItemType == blastedItemsType)
            {
                _goalCount -= blastedItemCount;
            }

            if (_goalCount <= 0)
            {
                _goalCount = 0;
                LevelUp();
            }else if (_moveCount <= 0)
            {
                _moveCount = 0;
                GameOver();
            }

            OnLevelStatsUpdateEvent?.Invoke();
        }

        void LoadJsonDatas()
        {
            levelItemDatas = new List<LevelItem>();
            if (_currentLevelData.LevelJson != null)
            {
                var levelDataJson = JsonUtility.FromJson<LevelDataJson>(_currentLevelData.LevelJson.text);
                levelItemDatas = levelDataJson?.levelItems;
            }
        }

        public void FillBoard()
        {
            var i = 0;
            for (var y = 0; y < _rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    var itemType = DefineItemType(i);
                    var cell = _board.Cells[i];
                    cell.Item = _itemManager.CreateItem(itemType, cell.transform.localPosition);
                    i++;
                }
            }
        }

        ItemType DefineItemType(int i)
        {
            if (levelItemDatas.Any())
            {
                i = levelItemDatas[i].colorIndex;
            }
            else
            {
                i = UnityEngine.Random.Range(0, _itemTypes.Count);
            }

            return _itemTypes[i];
        }
    }
}

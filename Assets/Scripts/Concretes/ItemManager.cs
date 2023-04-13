using UnityEngine;
using CollapseBlast.Enums;
using CollapseBlast.ScriptableObjects;
using CollapseBlast.Controller;
using System.Collections.Generic;

namespace CollapseBlast.Manager
{
    public class FallAnimData
    {
        public FallAnimData(float startVelocity, float acceleration, float maxVelocity)
        {
            StartVelocity = startVelocity;
            Acceleration = acceleration;
            MaxVelocity = maxVelocity;
        }

        public float StartVelocity;
        public float Acceleration;
        public float MaxVelocity;
    }
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] float _startVelocity = 0.4f;
        [SerializeField] float _acceleration = 0.3f;
        [SerializeField] float _maxVelocity = 12f;
        [SerializeField] ItemTypesData _itemTypesData;
        [SerializeField] ItemController _itemPrefab;
        Transform _itemsParent;
        LevelManager _levelManager;
        Board _board;

        [HideInInspector] public FallAnimData FallAnimData;

        public void Init()
        {
            var gameManager = GameManager.Instance;
            _itemsParent = gameManager.Board.ItemsParent;
            _levelManager = gameManager.Level;
            _board = gameManager.Board;
            FallAnimData = new FallAnimData(_startVelocity, _acceleration, _maxVelocity);
        }

        public Sprite GetItemSprite(ItemType itemType, int index)
        {
            switch (itemType)
            {
                case ItemType.RedBox:
                    return _itemTypesData.RedBoxes[index];
                case ItemType.GreenBox:
                    return _itemTypesData.GreenBoxes[index];
                case ItemType.BlueBox:
                    return _itemTypesData.BlueBoxes[index];
                case ItemType.YellowBox:
                    return _itemTypesData.YellowBoxes[index];
                case ItemType.PurpleBox:
                    return _itemTypesData.PurpleBoxes[index];
                case ItemType.PinkBox:
                    return _itemTypesData.PinkBoxes[index];
                case ItemType.Booster:
                    return _itemTypesData.Boosters[index];
            }
            return null;
        }
        
        public ItemController CreateItem(ItemType itemType, Vector3 itemSpawnPos, int boosterTypeIndex = 0)
        {
            var item = Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity, _itemsParent).GetComponent<ItemController>();
            item.Init(itemType, itemSpawnPos, boosterTypeIndex);
            return item;
        }

        public void ExecuteBooster(int boosterIndex, Cell cell)
        {
            var goalItemType = _levelManager.CurrentLevelData.GoalItemType;
            var blastedGoalItem = 0;
            var cellsToExplode = new List<Cell>() { cell };


            switch (boosterIndex)
            {
                case 0:
                    var leftCell = _board.GetNeighbourWithDirection(cell, Direction.Left);
                    var rightCell = _board.GetNeighbourWithDirection(cell, Direction.Right);
                    while (true)
                    {
                        if (leftCell != null)
                        {
                            cellsToExplode.Add(leftCell);
                            leftCell = _board.GetNeighbourWithDirection(leftCell, Direction.Left);
                        }
                        if (rightCell != null)
                        {
                            cellsToExplode.Add(rightCell);
                            rightCell = _board.GetNeighbourWithDirection(rightCell, Direction.Right);
                        }

                        if (leftCell == null && rightCell == null) break;
                    }

                    break;

                case 1:
                    cellsToExplode.AddRange(cell.CellsInTheBombBoosterArea());
                    break;

                case 2:                    
                    var rndItemType = _board.GetRandomCellAtBoard().Item.ItemType;
                    cellsToExplode.AddRange(_board.GetCellsWithItemType(rndItemType));
                    break;
            }


            foreach (var cellToExplode in cellsToExplode)
            {
                if (cellToExplode.Item.ItemType == goalItemType)
                {
                    blastedGoalItem++;
                }

                Debug.Log(cellToExplode.X + ":" + cellToExplode.Y);
                if (cellToExplode.Item != null) cellToExplode.Item.Destroy();

            }

            if (blastedGoalItem > 0)
            {
                _levelManager.UpdateLevelStats(goalItemType, blastedGoalItem);
            }
        }
    }
}



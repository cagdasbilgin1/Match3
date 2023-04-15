using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using CollapseBlast.Enums;
using CollapseBlast.Manager;
using CollapseBlast.Abstracts;

namespace CollapseBlast.Controller
{
    public class BoosterRocketAnim : MonoBehaviour, IBoosterAnim
    {
        [SerializeField] Transform piece1, piece2;
        [SerializeField] ParticleSystem particle1, particle2;
        Board _board;
        LevelManager _level;
        Camera _camera;
        Vector3 piece1Pos, piece2Pos;
        bool _isHorizontal;
        bool _isPlaying;
        int xOffset = Screen.width * 2;
        int yOffset = Screen.height;
        List<Cell> _cellsToExplode = new List<Cell>();
        int _blastedGoalItemCount;
        bool _piece1OutOfScreen => piece1Pos.x < -xOffset || piece1Pos.x > Screen.width + xOffset || piece1Pos.y < -yOffset || piece1Pos.y > Screen.height + yOffset;
        bool _piece2OutOfScreen => piece2Pos.x < -xOffset || piece2Pos.x > Screen.width + xOffset || piece2Pos.y < -yOffset || piece2Pos.y > Screen.height + yOffset;

        public void ExecuteAnim(Cell boosterCell, LevelManager level)
        {
            _level = level;
            var goalItemType = _level.CurrentLevelData.GoalItemType;
            FindCells(boosterCell, out _isHorizontal);
            DestroyCellItems(goalItemType);
            UpdateGoalChart(goalItemType);
            PlayRocketAnim(_isHorizontal);
        }

        void Update()
        {
            if (!_isPlaying) return;

            piece1Pos = _camera.WorldToScreenPoint(piece1.position);
            piece2Pos = _camera.WorldToScreenPoint(piece2.position);

            if (_piece1OutOfScreen && _piece2OutOfScreen)
            {
                Destroy(gameObject);
            }
        }

        void FindCells(Cell boosterCell, out bool isHorizontalRocket)
        {
            _camera = Camera.main;
            _board = GameManager.Instance.Board;
            isHorizontalRocket = boosterCell.Item.IsHorizontalRocketBooster;

            var direction1 = isHorizontalRocket ? Direction.Left : Direction.Up;
            var direction2 = isHorizontalRocket ? Direction.Right : Direction.Down;
            var cell1 = _board.GetNeighbourWithDirection(boosterCell, direction1);
            var cell2 = _board.GetNeighbourWithDirection(boosterCell, direction2);
            _cellsToExplode.Add(boosterCell);

            while (true)
            {
                if (cell1 != null)
                {
                    _cellsToExplode.Add(cell1);
                    cell1 = _board.GetNeighbourWithDirection(cell1, direction1);
                }
                if (cell2 != null)
                {
                    _cellsToExplode.Add(cell2);
                    cell2 = _board.GetNeighbourWithDirection(cell2, direction2);
                }

                if (cell1 == null && cell2 == null) break;
            }
        }

        void DestroyCellItems(ItemType goalItemType)
        {
            foreach (var cell in _cellsToExplode)
            {
                var item = cell.Item;
                if (item == null) continue;
                if (item.ItemType == goalItemType)
                {
                    _blastedGoalItemCount++;
                }
                item.Destroy();
            }
        }

        void PlayRocketAnim(bool isHorizontal)
        {
            if (isHorizontal)
            {
                transform.Rotate(0, 0, 90f, Space.Self);
                piece1.DOMoveX(piece1.position.x - 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                piece2.DOMoveX(piece1.position.x + 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            }
            else
            {
                piece1.DOMoveY(piece1.position.y + 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                piece2.DOMoveY(piece1.position.y - 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            }

            _isPlaying = true;
        }

        void UpdateGoalChart(ItemType goalItemType)
        {
            _level.UpdateLevelStats(goalItemType, _blastedGoalItemCount);
        }

        void OnDestroy()
        {
            piece1.DOKill();
            piece2.DOKill();
        }
    }
}
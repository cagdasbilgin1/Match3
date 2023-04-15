using CollapseBlast.Abstracts;
using CollapseBlast.Enums;
using CollapseBlast.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.Controller
{
    public class BoosterTntAnim : MonoBehaviour, IBoosterAnim
    {
        [SerializeField] Animator _animator;
        LevelManager _level;
        List<Cell> _cellsToExplode = new List<Cell>();
        int _blastedGoalItemCount;

        public void ExecuteAnim(Cell boosterCell, LevelManager level)
        {
            _level = level;
            var goalItemType = _level.CurrentLevelData.GoalItemType;
            FindCells(boosterCell);
            DestroyCellItems(goalItemType);
            UpdateGoalChart(goalItemType);
            StartCoroutine(DestroyAnimOnFinish());
        }

        void FindCells(Cell boosterCell)
        {
            _cellsToExplode = boosterCell.CellsInTheBombBoosterArea();
            _cellsToExplode.Add(boosterCell);
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

        void UpdateGoalChart(ItemType goalItemType)
        {
            _level.UpdateLevelStats(goalItemType, _blastedGoalItemCount);
        }

        IEnumerator DestroyAnimOnFinish()
        {
            yield return new WaitForSeconds(_animator.runtimeAnimatorController.animationClips.Length);

            Destroy(gameObject);
        }
    }
}
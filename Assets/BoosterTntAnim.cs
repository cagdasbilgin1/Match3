using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.Controller
{
    public class BoosterTntAnim : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        List<Cell> _cellsToExplode = new List<Cell>();
        public void ExecuteAnim(Cell boosterCell)
        {
            FindCells(boosterCell);
            DestroyCellItems();
            StartCoroutine(DestroyAnimOnFinish());
        }

        void FindCells(Cell boosterCell)
        {
            _cellsToExplode = boosterCell.CellsInTheBombBoosterArea();
            _cellsToExplode.Add(boosterCell);
        }

        void DestroyCellItems()
        {
            foreach (var cell in _cellsToExplode)
            {
                cell.Item?.Destroy();
            }
        }

        IEnumerator DestroyAnimOnFinish()
        {
            yield return new WaitForSeconds(_animator.runtimeAnimatorController.animationClips.Length);

            Destroy(gameObject);
        }
    }
}
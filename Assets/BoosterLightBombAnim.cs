using CollapseBlast;
using CollapseBlast.Controller;
using CollapseBlast.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterLightBombAnim : MonoBehaviour
{
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] GameObject glowVFXPrefab;
    [SerializeField] float destroyDelay = .5f;

    List<LineRenderer> _lines = new List<LineRenderer>();
    List<GameObject> _particles = new List<GameObject>();
    List<Cell> _cellsToExplode = new List<Cell>();
    List<ItemController> _itemsToDestroy = new List<ItemController>();
    Transform _particleParent;

    public void ExecuteAnim(Cell boosterCell)
    {
        FindCells(boosterCell);
        PlayLightBombAnim(boosterCell);
        StartCoroutine(FinishAnimDestroyItems());
    }

    IEnumerator FinishAnimDestroyItems()
    {
        yield return new WaitForSeconds(destroyDelay);

        foreach (var line in _lines)
        {
            Destroy(line.gameObject);
        }
        foreach (var particle in _particles)
        {
            Destroy(particle);
        }
        foreach (var item in _itemsToDestroy)
        {
            item?.Destroy();
        }
        Destroy(gameObject);
    }

    void FindCells(Cell boosterCell)
    {
        transform.position = Vector2.zero;
        boosterCell.Item.ArrangeClickable(false);
        var board = GameManager.Instance.Board;
        var rndItemType = board.GetRandomCellAtBoard().Item.ItemType;
        _cellsToExplode = board.GetCellsWithItemType(rndItemType);
        _cellsToExplode.Add(boosterCell);
        _particleParent = board.ParticlesAnimationsParent;
    }

    void PlayLightBombAnim(Cell boosterCell)
    {
        foreach (var cell in _cellsToExplode)
        {
            var line = Instantiate(linePrefab, _particleParent);
            var glowVFX = Instantiate(glowVFXPrefab, _particleParent);
            _lines.Add(line);
            _particles.Add(glowVFX);
            _itemsToDestroy.Add(cell.Item);
            line.SetPosition(0, boosterCell.transform.localPosition);
            line.SetPosition(1, cell.transform.localPosition);
            glowVFX.transform.localPosition = cell.transform.localPosition;
        }
    }
}

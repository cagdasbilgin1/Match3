using CollapseBlast.Manager;
using CollapseBlast.ScriptableObjects;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CollapseBlast.Canvas
{
    public class GamePlayCanvas : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _levelGoalCounterText;
        [SerializeField] TextMeshProUGUI _levelMovesCounterText;
        [SerializeField] Image _levelGoalItemTypeImage;
        [SerializeField] GameObject _levelUpUI;
        [SerializeField] GameObject _gameOverUI;
        LevelManager _level;
        ItemManager _itemManager;
        LevelDataSO _levelData;

        public event Action OnBackToMetaButtonClickEvent;

        private void Awake()
        {
            var gameManager = GameManager.Instance;
            _itemManager = gameManager.ItemManager;
            _level = gameManager.Level;
            _levelData = gameManager.Level.CurrentLevelData;

            _level.OnLevelUpEvent += UpdateLevelData;
            _level.OnLevelUpEvent += ShowLevelUpUI;
            _level.OnGameOverEvent += ShowGameOverUI;
            _level.OnLevelStatsUpdateEvent += UpdateLevelStatsUI;
            gameManager.CanvasManager.MetaCanvas.OnLevelButtonClickEvent += ResetUIs;
        }

        public void ResetUIs()
        {
            _levelGoalCounterText.text = _levelData.GoalCount.ToString();
            _levelMovesCounterText.text = _levelData.MovesCount.ToString();
            _levelGoalItemTypeImage.sprite = _itemManager.GetItemSprite(_levelData.GoalItemType, 0);
        }

        public void UpdateLevelData()
        {
            _levelData = GameManager.Instance.Level.CurrentLevelData;
        }

        public void OnBackToMetaButtonClick()
        {
            OnBackToMetaButtonClickEvent?.Invoke();
        }

        public void UpdateLevelStatsUI()
        {
            _levelGoalCounterText.text = _level.GoalCount.ToString();
            _levelMovesCounterText.text = _level.MoveCount.ToString();
        }

        public void ShowLevelUpUI()
        {
            _levelUpUI.SetActive(true);
        }

        public void ShowGameOverUI()
        {
            _gameOverUI.SetActive(true);
        }

        public void OnClaimButtonClick()
        {
            var gameManager = GameManager.Instance;
            gameManager.Board.ClearElements();
            gameManager.InitGame();
        }
    }
}
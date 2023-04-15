using CollapseBlast.Manager;
using CollapseBlast.ScriptableObjects;
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
            gameManager.gamePlaySceneOpenedEvent += ResetUIs;
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
            GameManager.Instance.ToggleScene();
        }

        public void UpdateLevelStatsUI()
        {
            _levelGoalCounterText.text = _level.GoalCount.ToString();
            _levelMovesCounterText.text = _level.MoveCount.ToString();
        }

        void ShowLevelUpUI()
        {
            var soundManager = GameManager.Instance.SoundManager;
            soundManager.PlayMusic(soundManager.GameSounds.LevelWin, 1, false);
            _levelUpUI.SetActive(true);
        }

        void DismissLevelUpUI()
        {
            _levelUpUI.SetActive(false);
        }

        void ShowGameOverUI()
        {
            var soundManager = GameManager.Instance.SoundManager;
            soundManager.PlayMusic(soundManager.GameSounds.LevelLose, 1, false);
            _gameOverUI.SetActive(true);
        }

        void DismissGameOverUI()
        {
            _gameOverUI.SetActive(false);
        }

        public void OnClaimButtonClick()
        {
            GameManager.Instance.ToggleScene();

            DismissLevelUpUI();
            
            var gameManager = GameManager.Instance;
            gameManager.Board.ClearElements();
            gameManager.InitGame();
        }

        public void OnGoHomeButtonClick()
        {
            GameManager.Instance.ToggleScene();

            DismissGameOverUI();

            var gameManager = GameManager.Instance;
            gameManager.Board.ClearElements();
            gameManager.InitGame();
        }
    }
}
using CollapseBlast.Manager;
using System;
using TMPro;
using UnityEngine;

namespace CollapseBlast.Canvas
{
    public class MetaCanvas : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI levelButtonText;

        public event Action OnLevelButtonClickEvent;

        private void Awake()
        {
            var gameManager = GameManager.Instance;
            UpdateLevelButtonText();

            gameManager.Level.OnLevelUpEvent += UpdateLevelButtonText;
        }

        public void UpdateLevelButtonText()
        {
            levelButtonText.text = $"Level {GameManager.Instance.Level.LevelIndex + 1}";
        }

        public void OnLevelButtonClick()
        {
            OnLevelButtonClickEvent?.Invoke();
        }
    }
}
using CollapseBlast.Manager;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CollapseBlast.Canvas
{
    public class MetaCanvas : MonoBehaviour
    {
        [SerializeField] List<GameObject> GameplaySceneElements;
        [SerializeField] TextMeshProUGUI levelButtonText;

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
            GameManager.Instance.ToggleScene();
        }
    }
}
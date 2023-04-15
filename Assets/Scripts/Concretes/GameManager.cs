using CollapseBlast.Abstracts;
using CollapseBlast.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.Manager
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public Board Board;
        public event Action gamePlaySceneOpenedEvent;
        public event Action metaSceneOpenedEvent;

        [SerializeField] List<GameObject> _gamePlaySceneElements;
        [SerializeField] List<GameObject> _metaSceneElements;        

        [HideInInspector] public LevelManager Level;
        [HideInInspector] public ItemManager ItemManager;
        [HideInInspector] public FallManager FallManager;
        [HideInInspector] public TouchManager TouchManager;
        [HideInInspector] public HintManager HintManager;
        [HideInInspector] public CanvasManager CanvasManager;

        void Awake()
        {
            SetSingletonThisGameObject(this);

            Application.targetFrameRate = 120;

            HintManager = new HintManager();
            FallManager = new FallManager();
            Level = GetComponent<LevelManager>();
            ItemManager = GetComponent<ItemManager>();
            TouchManager = GetComponent<TouchManager>();
            CanvasManager = GetComponent<CanvasManager>();
            InitGame();

            Level.OnLevelUpEvent += DisableInput;
            Level.OnGameOverEvent += DisableInput;
        }

        public void InitGame()
        {
            Level.Init();
            Board.Init();
            ItemManager.Init();
            HintManager.Init();
            FallManager.Init();
            Level.FillBoard();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Level.LevelUp();
                Board.ClearElements();
                InitGame();
            }

            FallManager.TickUpdate();
            HintManager.TickUpdate();
        }

        public void EnableInput()
        {
            //only affects item touches
            TouchManager.enabled = true;
        }

        public void DisableInput()
        {
            //only affects item touches
            TouchManager.enabled = false;
        }

        public void ToggleScene()
        {
            var isGameplayActive = false;
            foreach (var gameplaySceneElement in _gamePlaySceneElements)
            {
                gameplaySceneElement.SetActive(!gameplaySceneElement.activeInHierarchy);
                isGameplayActive = gameplaySceneElement.activeInHierarchy;
            }

            if (isGameplayActive)
            {
                EnableInput();
                gamePlaySceneOpenedEvent?.Invoke();
            }
            else
            {
                DisableInput();
                metaSceneOpenedEvent?.Invoke();
            }
        }
    }
}
using CollapseBlast.Abstracts;
using CollapseBlast.Utilities;
using UnityEngine;

namespace CollapseBlast.Manager
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public Board Board;

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
            CanvasManager.GamePlayCanvas.OnBackToMetaButtonClickEvent += DisableInput;
            CanvasManager.MetaCanvas.OnLevelButtonClickEvent += EnableInput;
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
            TouchManager.enabled = true;
        }

        public void DisableInput()
        {
            TouchManager.enabled = false;
        }
    }
}
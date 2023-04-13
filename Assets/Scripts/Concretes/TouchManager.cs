using CollapseBlast.Manager;
using UnityEngine;

namespace CollapseBlast.Utilities
{
	public class TouchManager : MonoBehaviour
	{
		const string CellCollider = "CellCollider";
	
		Camera _camera;
		Board _board;

        void Awake()
        {
            var gameManager = GameManager.Instance;
            _camera = Camera.main;
			_board = gameManager.Board;
        }

        void Update () {
		
#if UNITY_EDITOR
			GetTouchEditor();
#else
		GetTouchMobile();
		#endif
		}

		void GetTouchEditor()
		{
			if (Input.GetMouseButtonUp(0))
			{
				ExecuteTouch(Input.mousePosition);
			}
		}

		void GetTouchMobile()
		{
			var touch = Input.GetTouch(0);
			switch (touch.phase)
			{
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					ExecuteTouch(touch.position);
					break;
			}
		}

		void ExecuteTouch(Vector3 pos)
		{
			var hit = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(pos)) as BoxCollider2D;
		
			if (hit != null && hit.CompareTag(CellCollider))
			{
				_board.CellTapped(hit.gameObject.GetComponent<Cell>());
			}
		}
	}
}

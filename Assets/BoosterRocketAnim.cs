using UnityEngine;
using DG.Tweening;

namespace CollapseBlast.Controller
{
    public class BoosterRocketAnim : MonoBehaviour
    {
        [SerializeField] Transform piece1, piece2;
        [SerializeField] ParticleSystem particle1, particle2;
        Camera _camera;
        Vector3 piece1Pos, piece2Pos;
        bool _isPlaying;
        int xOffset = Screen.width;
        int yOffset = Screen.height;
        bool _piece1OutOfScreen => piece1Pos.x < -xOffset || piece1Pos.x > Screen.width + xOffset || piece1Pos.y < -yOffset || piece1Pos.y > Screen.height + yOffset;
        bool _piece2OutOfScreen => piece2Pos.x < -xOffset || piece2Pos.x > Screen.width + xOffset || piece2Pos.y < -yOffset || piece2Pos.y > Screen.height + yOffset;

        void Start()
        {
            _camera = Camera.main;
        }

        public void ExecuteRocketAnim(bool isHorizontalRocket)
        {
            if (isHorizontalRocket)
            {
                transform.Rotate(0, 0, 90f, Space.Self);
                piece1.DOMoveX(piece1.position.x - 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                piece2.DOMoveX(piece1.position.x + 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            }
            else
            {
                piece1.DOMoveY(piece1.position.y + 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                piece2.DOMoveY(piece1.position.y - 10f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            }
            
            _isPlaying = true;
        }

        void Update()
        {
            if (!_isPlaying) return;

            piece1Pos = _camera.WorldToScreenPoint(piece1.position);
            piece2Pos = _camera.WorldToScreenPoint(piece2.position);

            if (_piece1OutOfScreen && _piece2OutOfScreen)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            piece1.DOKill();
            piece2.DOKill();
        }
    }
}
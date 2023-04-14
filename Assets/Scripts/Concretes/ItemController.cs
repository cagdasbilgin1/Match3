using UnityEngine;
using CollapseBlast.Enums;
using CollapseBlast.Manager;

namespace CollapseBlast.Controller
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Animator _animator;
        ItemManager _itemManager;
        Cell _cell;
        FallAnimation _fallAnimation;
        ItemType _itemType;
        int _typeIndex;

        public ItemType ItemType => _itemType;
        public bool IsBooster => _itemType == ItemType.Booster;
        public int TypeIndex { get { return _typeIndex; } set { _typeIndex = value; } }


        public Cell Cell
        {
            get { return _cell; }
            set
            {
                if (_cell == value) return;

                var oldCell = _cell;
                _cell = value;

                if (oldCell != null && oldCell.Item == this)
                {
                    oldCell.Item = null;
                }

                if (value != null)
                {
                    value.Item = this;
                    gameObject.name = $"Item {_cell.X}:{_cell.Y}";
                }
            }
        }

        public void Init(ItemType itemType, Vector3 pos, int boosterTypeIndex)
        {
            _itemType = itemType;
            _itemManager = GameManager.Instance.ItemManager;
            _fallAnimation = new FallAnimation(_itemManager.FallAnimData, this);
            transform.localPosition = pos;

            if(itemType == ItemType.Booster)
            {
                _typeIndex = boosterTypeIndex;
                ChangeSprite(_typeIndex);
            }
        }

        public void ChangeSprite(int typeIndex)
        {
            //if (ItemType == ItemType.Booster) return;
            _spriteRenderer.sprite = _itemManager.GetItemSprite(_itemType, typeIndex);
        }

        public void Destroy()
        {
            _cell.Item = null;
            _cell = null;
            Destroy(gameObject);
        }

        public void Fall()
        {
            _fallAnimation.FallTo(_cell.GetFallTargetCell());
        }

        public void ArrangeSorting()
        {
            _spriteRenderer.sortingOrder = _cell.Y;
        }

        private void Update()
        {
            _fallAnimation.TickUpdate();
        }
    }
}

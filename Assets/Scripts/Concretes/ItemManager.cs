using UnityEngine;
using CollapseBlast.Enums;
using CollapseBlast.ScriptableObjects;
using CollapseBlast.Controller;

namespace CollapseBlast.Manager
{
    public class FallAnimData
    {
        public FallAnimData(float startVelocity, float acceleration, float maxVelocity)
        {
            StartVelocity = startVelocity;
            Acceleration = acceleration;
            MaxVelocity = maxVelocity;
        }

        public float StartVelocity;
        public float Acceleration;
        public float MaxVelocity;
    }
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] float _startVelocity = 0.4f;
        [SerializeField] float _acceleration = 0.3f;
        [SerializeField] float _maxVelocity = 12f;
        [SerializeField] ItemTypesData _itemTypesData;
        [SerializeField] ItemController _itemPrefab;
        Transform _itemsParent;

        [HideInInspector] public FallAnimData FallAnimData;

        public void Init()
        {
            _itemsParent = GameManager.Instance.Board.ItemsParent;
            FallAnimData = new FallAnimData(_startVelocity, _acceleration, _maxVelocity);
        }

        public Sprite GetItemSprite(ItemType itemType, int index)
        {
            switch (itemType)
            {
                case ItemType.RedBox:
                    return _itemTypesData.RedBoxes[index];
                case ItemType.GreenBox:
                    return _itemTypesData.GreenBoxes[index];
                case ItemType.BlueBox:
                    return _itemTypesData.BlueBoxes[index];
                case ItemType.YellowBox:
                    return _itemTypesData.YellowBoxes[index];
                case ItemType.PurpleBox:
                    return _itemTypesData.PurpleBoxes[index];
                case ItemType.PinkBox:
                    return _itemTypesData.PinkBoxes[index];
            }
            return null;
        }
        
        public ItemController CreateItem(ItemType itemType)
        {
            var item = Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity, _itemsParent).GetComponent<ItemController>();
            item.Init(itemType);
            return item;
        }
    }
}



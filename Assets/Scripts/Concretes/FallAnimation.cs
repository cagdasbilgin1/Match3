using CollapseBlast.Controller;
using CollapseBlast.Manager;
using UnityEngine;

namespace CollapseBlast
{
	public class FallAnimation
	{        
        float _startVelocity;
		float _acceleration;
		float _maxVelocity;		
		float _currentVelocity;
        bool _isFalling;
		Vector3 _targetPosition;
        Vector3 _newPosition;
        ItemController _item;
        Cell _targetCell;

        public FallAnimation(FallAnimData fallAnimData, ItemController item)
        {
            _startVelocity = fallAnimData.StartVelocity;
			_acceleration = fallAnimData.Acceleration;
			_maxVelocity = fallAnimData.MaxVelocity;
			_item = item;
        }

        public void FallTo(Cell targetCell)
		{
			if (_targetCell != null && targetCell.Y >= _targetCell.Y) return;
			_targetCell = targetCell;
			_item.Cell = targetCell;
			_targetPosition = targetCell.transform.position;
			_isFalling = true;
			_currentVelocity = _startVelocity;
			_newPosition = _item.transform.position;
        }

		public void TickUpdate()
		{
			if(!_isFalling) return;
			_currentVelocity += _acceleration;
            _currentVelocity = Mathf.Min(_currentVelocity, _maxVelocity);
			_newPosition.y -= _currentVelocity * Time.deltaTime;
			if (_newPosition.y <= _targetPosition.y)
			{
				_isFalling = false;
				_newPosition.y = _targetPosition.y;
				_currentVelocity = _startVelocity;
			}
			_item.transform.position = _newPosition;
		}		
	}
}

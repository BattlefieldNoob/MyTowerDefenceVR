using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public class GameFieldPositionableService : IGameFieldPositionableActuator, IGameFieldPositionableCollector,
        ITickable
    {
        private Vector3[] _enemyInRange;
        private bool _attack;
        private Vector3 _attackTarget;
        private Quaternion _baseRotation = Quaternion.identity;
        private bool _docked;
        private Vector3 _basePosition;
        private float rotationSpeedDegree = 45;

        public Quaternion GetBaseRotation() => _baseRotation;

        public bool ShouldAttack() => _attack;

        public Vector3 GetTargetAttack() => _attackTarget;

        public void UpdateEnemyInRange(Vector3[] targets) => _enemyInRange = targets;

        public void DockedInSlot(Vector3 slotPosition)
        {
            _docked = true;
            _basePosition = slotPosition;
        }

        public void Undocked()
        {
            _docked = false;
            _attack = false;
        }

        public void Tick()
        {
            if (!_docked)
                return;

            if (_enemyInRange != null && _enemyInRange.Length != 0)
            {
                _attackTarget = _enemyInRange[0];
                _attack = true;
                var targetWithoutY = new Vector3(_attackTarget.x, _basePosition.y, _attackTarget.z);
                var direction = (targetWithoutY - _basePosition).normalized;
                var targetRotation = Quaternion.LookRotation(direction);
                var angle = Quaternion.Angle(_baseRotation, targetRotation);
                var angleAmount = Mathf.Clamp(angle, 0, rotationSpeedDegree);

                _baseRotation = Quaternion.RotateTowards(_baseRotation, targetRotation, angleAmount * Time.deltaTime);
            }
            else
            {
                _attack = false;
            }
        }
    }
}
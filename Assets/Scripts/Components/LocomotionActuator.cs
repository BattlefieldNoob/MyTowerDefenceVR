using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using VContainer;

namespace Components
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class LocomotionActuator : MonoBehaviour
    {
        private ILocomotionActuator _actuator;

        [Inject]
        void Construct(ILocomotionActuator actuator)
        {
            _actuator = actuator;
        }

        void Update()
        {
            transform.position = _actuator.GetPosition();
            var scale = _actuator.GetScale();
            transform.localScale.Set(scale, scale, scale);
        }
    }
}
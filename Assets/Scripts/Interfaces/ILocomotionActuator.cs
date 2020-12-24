
using UnityEngine;

namespace Components
{
    public interface ILocomotionActuator
    {
        Vector3 GetPosition();
        float GetScale();
    }
}
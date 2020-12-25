using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameFieldPositionableActuator
{
    Quaternion GetBaseRotation();
    
    bool ShouldAttack();

    Vector3 GetTargetAttack();

}

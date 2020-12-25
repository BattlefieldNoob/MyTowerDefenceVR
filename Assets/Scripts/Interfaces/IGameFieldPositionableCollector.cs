using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameFieldPositionableCollector
{
    void UpdateEnemyInRange(Vector3[] targets);

    void DockedInSlot(Vector3 slotPosition);

    void Undocked();
}

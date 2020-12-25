using System.Linq;
using NUnit.Framework;
using Services;
using UnityEngine;
using VContainer.Unity;

public class GameFieldPositionableTests
{
    private GameFieldPositionableService _service;

    [SetUp]
    public void GameFieldPositionableSetup()
    {
        _service = new GameFieldPositionableService();
    }

    [Test]
    public void GameFieldPositionable_Not_Docked()
    {
        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        _service.UpdateEnemyInRange(Enumerable.Repeat(Random.insideUnitSphere, 3).ToArray());

        DoRandomFrames(_service);

        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));
    }


    [Test]
    public void GameFieldPositionable_Docked()
    {
        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        _service.DockedInSlot(Random.insideUnitSphere);

        DoRandomFrames(_service);

        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));
    }

    [Test]
    public void GameFieldPositionable_Docked_WithEnemy()
    {
        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        _service.DockedInSlot(Random.insideUnitSphere);

        DoRandomFrames(_service);

        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        var enemies = Enumerable.Repeat(Random.insideUnitSphere, 3).ToArray();

        _service.UpdateEnemyInRange(enemies);

        DoRandomFrames(_service);

        Assert.That(_service.GetBaseRotation(), Is.Not.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.True);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(enemies[0]));
    }

    [Test]
    public void GameFieldPositionable_Docked_WithEnemy_Undock()
    {
        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        _service.DockedInSlot(Random.insideUnitSphere);

        DoRandomFrames(_service);

        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        var enemies = Enumerable.Repeat(Random.insideUnitSphere, 3).ToArray();

        _service.UpdateEnemyInRange(enemies);

        DoRandomFrames(_service);

        var rotationBeforeUndock = _service.GetBaseRotation();
        var targetAttackBeforeUndock = _service.GetTargetAttack();

        Assert.That(rotationBeforeUndock, Is.Not.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.True);
        Assert.That(targetAttackBeforeUndock, Is.EqualTo(enemies[0]));

        _service.Undocked();

        DoRandomFrames(_service);

        Assert.That(_service.GetBaseRotation(), Is.EqualTo(rotationBeforeUndock));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(targetAttackBeforeUndock));
    }

    [Test]
    public void GameFieldPositionable_Docked_WithEnemy_CheckRotation()
    {
        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        var slotPosition = Random.insideUnitSphere;
        _service.DockedInSlot(slotPosition);

        DoRandomFrames(_service);

        Assert.That(_service.GetBaseRotation(), Is.EqualTo(Quaternion.identity));
        Assert.That(_service.ShouldAttack(), Is.False);
        Assert.That(_service.GetTargetAttack(), Is.EqualTo(Vector3.zero));

        var enemies = Enumerable.Repeat(Random.insideUnitSphere, 3).ToArray();

        _service.UpdateEnemyInRange(enemies);

        var direction = (new Vector3(enemies[0].x, slotPosition.y, enemies[0].z) - slotPosition).normalized;
        var targetRotation = Quaternion.LookRotation(direction);
        var angleDifference = Quaternion.Angle(targetRotation, _service.GetBaseRotation());

        _service.Tick();

        var baseRotation = _service.GetBaseRotation();
        var newAngleDifference = Quaternion.Angle(targetRotation, baseRotation);

        Assert.That(newAngleDifference, Is.LessThan(angleDifference));
        Assert.That(_service.ShouldAttack(), Is.True);
        angleDifference = newAngleDifference;

        while (angleDifference > 0.1f)
        {
            DoRandomFrames(_service);

            newAngleDifference = Quaternion.Angle(targetRotation, _service.GetBaseRotation());
            Assert.That(newAngleDifference, Is.LessThan(angleDifference));
            Assert.That(_service.ShouldAttack(), Is.True);
            angleDifference = newAngleDifference;
        }
    }

    private static void DoRandomFrames(ITickable tickable)
    {
        foreach (var unused in Enumerable.Range(0, Random.Range(4, 10)))
        {
            tickable.Tick();
        }
    }
}
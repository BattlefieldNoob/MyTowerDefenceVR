using System.Linq;
using NUnit.Framework;
using Services;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using VContainer.Unity;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
public class LocomotionServiceTests
{
    private static float[] _values = {1, 0.5f, 0.25f};

    private const float Tollerance = 1e-6f;

    private static readonly Vector3EqualityComparer Comparer = new Vector3EqualityComparer(1e-6f);

    private LocomotionService _service;

    private Vector3 _initialOffset;
    private Vector3 _startLeftHand;
    private Vector3 _startRightHand;
    private Vector3 _offsetPosition;
    private float _offsetScale;

    [SetUp]
    public void LocomotionSetup()
    {
        _service = new LocomotionService();

        _initialOffset = (Random.value + 0.05f) * new Vector3(1, 0, 1);
        _startLeftHand = Random.onUnitSphere;
        _startRightHand = _startLeftHand + _initialOffset;
        _offsetPosition = (Random.value + 0.05f) * new Vector3(1.5f, 0, 1.5f);
        _offsetScale = (Random.value + 0.05f);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void LocomotionService_NoPitch()
    {
        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());
        var state = new LocomotionService.InputState() {isPitch = false, position = Random.onUnitSphere};
        _service.UpdateLeftHandState(state);
        _service.UpdateRightHandState(state);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());
    }

    [Test]
    public void LocomotionService_OnlyOneHandPitch()
    {
        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        var leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand};
        var rightState = new LocomotionService.InputState() {isPitch = false, position = _startRightHand};


        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        leftState = new LocomotionService.InputState() {isPitch = false, position = _startLeftHand};
        rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());
    }

    [Test]
    public void LocomotionService_TwoPitch_ZeroOffset()
    {
        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        var state = new LocomotionService.InputState() {isPitch = false, position = _offsetPosition};

        _service.UpdateLeftHandState(state);
        _service.UpdateRightHandState(state);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());
    }

    [Test]
    public void LocomotionService_SetPosition()
    {
        var leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand};
        var rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        _service.SetPosition(_offsetPosition);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(_offsetPosition, _service.GetPosition());

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(_offsetPosition, _service.GetPosition());
    }

    [Test]
    public void LocomotionService_SetScale()
    {
        var leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand};
        var rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());


        _service.SetScale(_offsetScale);

        Assert.AreEqual(_offsetScale, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        DoRandomFrames(_service);

        Assert.AreEqual(_offsetScale, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());
    }

    [Test]
    public void LocomotionService_TwoPitch_Position([ValueSource(nameof(_values))] float multiplier)
    {
        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        var leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand};
        var rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        var offset = _offsetPosition * multiplier;

        leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand + offset};
        rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand + offset};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        //we don't move on y axis
        var expectedDirection = Vector3.Scale(offset, new Vector3(1, 0, 1)).normalized;

        Assert.AreEqual(0, _service.GetScale(), Tollerance);

        var position = _service.GetPosition();

        var direction = position.normalized;

        Assert.That(direction, Is.EqualTo(expectedDirection).Using(Comparer));
    }

    [Test]
    public void LocomotionService_TwoPitch_Scale([ValueSource(nameof(_values))] float multiplier)
    {
        var zeroScale = _service.GetScale();
        Assert.AreEqual(0, zeroScale);
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        var leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand};
        var rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        var offset = _offsetPosition * multiplier;

        leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand - offset};
        rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand + offset};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        var positiveScale = _service.GetScale();

        //check scale grown
        Assert.Greater(positiveScale, zeroScale);
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand};
        rightState = new LocomotionService.InputState()
            {isPitch = true, position = _startRightHand - _initialOffset * 0.5f};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        //check scale shrunk
        Assert.Less(_service.GetScale(), positiveScale);
        Assert.Less(_service.GetScale(), zeroScale);
        Assert.AreEqual(Vector3.zero, _service.GetPosition());
    }

    [Test]
    public void LocomotionService_Reset()
    {
        var leftState = new LocomotionService.InputState() {isPitch = true, position = _startLeftHand};
        var rightState = new LocomotionService.InputState() {isPitch = true, position = _startRightHand};

        _service.UpdateLeftHandState(leftState);
        _service.UpdateRightHandState(rightState);

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());


        _service.SetScale(_offsetScale);
        _service.SetPosition(_offsetPosition);

        DoRandomFrames(_service);

        Assert.AreNotEqual(0, _service.GetScale());
        Assert.AreNotEqual(Vector3.zero, _service.GetPosition());

        _service.Reset();

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());

        DoRandomFrames(_service);

        Assert.AreEqual(0, _service.GetScale());
        Assert.AreEqual(Vector3.zero, _service.GetPosition());
    }

    private static void DoRandomFrames(ITickable tickable)
    {
        foreach (var unused in Enumerable.Range(0, Random.Range(4, 10)))
        {
            tickable.Tick();
        }
    }
}
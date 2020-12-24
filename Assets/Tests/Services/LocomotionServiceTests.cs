using System.Collections;
using System.Linq;
using Components;
using NUnit.Framework;
using Services;
using Tests.Services;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;
using UnityEngine.TestTools.Utils;

// ReSharper disable once CheckNamespace
public class LocomotionServiceTests
{
    static float[] values = { 1, 0.1f, 0.05f, 0.025f };
    
    private static readonly Vector3EqualityComparer Comparer = new Vector3EqualityComparer(1e-2f);


    private GameObject _testObject;
    private LifetimeScope _lifetimeScope;
    private TestLocomotionCollector _collector;
    private IObjectResolver _objectResolver;
    private ILocomotion _locomotion;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _lifetimeScope = new GameObject("LifetimeScope").AddComponent<LifetimeScope>();
        
        _testObject = new GameObject("TestingObject"); 
        _collector = _testObject.AddComponent<TestLocomotionCollector>();
        _testObject.AddComponent<LocomotionActuator>();
        
        var container = new ContainerBuilder() {ApplicationOrigin = _lifetimeScope};
        
        container.RegisterComponentInHierarchy<TestLocomotionCollector>();
        container.RegisterComponentInHierarchy<LocomotionActuator>();
        container.RegisterEntryPoint<LocomotionService>(Lifetime.Singleton);

        _objectResolver = container.Build();

        _locomotion = _objectResolver.Resolve<ILocomotion>();
    }


    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _testObject.transform.position=Vector3.zero;
        _testObject.transform.localScale=Vector3.one;
        _locomotion.Reset();
        yield return null;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator LocomotionService_NoPitch()
    {
        var service = _objectResolver.Resolve<ITickable>();
        var a=PlayerLoop.GetCurrentPlayerLoop();
        a.updateDelegate += () => Debug.Log("MOOOOOOOOO");

        _collector.LeftHand = new LocomotionService.InputState();
        _collector.RightHand = new LocomotionService.InputState();
        
        //do random frames
        yield return DoRandomFrames(service);
        
        var objectTransform = _testObject.transform;
        Assert.That(objectTransform.position,Is.EqualTo(Vector3.zero));
        Assert.That(objectTransform.localScale,Is.EqualTo(Vector3.one));

    }
    
    [UnityTest]
    public IEnumerator LocomotionService_OnlyOnePitch()
    {
        var service = _objectResolver.Resolve<ITickable>();
        _collector.LeftHand = new LocomotionService.InputState() {isPitch = true};
        _collector.RightHand = new LocomotionService.InputState();
        
        //do random frames
        yield return DoRandomFrames(service);
        
        var objectTransform = _testObject.transform;
        Assert.That(objectTransform.position,Is.EqualTo(Vector3.zero));
        Assert.That(objectTransform.localScale,Is.EqualTo(Vector3.one));

        _collector.LeftHand = new LocomotionService.InputState();
        _collector.RightHand = new LocomotionService.InputState() {isPitch = true};
        
        yield return DoRandomFrames(service);
        
        Assert.That(objectTransform.position,Is.EqualTo(Vector3.zero));
        Assert.That(objectTransform.localScale,Is.EqualTo(Vector3.one));
    }
    
    
    [UnityTest]
    public IEnumerator LocomotionService_TwoPitchNoOffset()
    {
        var service = _objectResolver.Resolve<ITickable>();
        _collector.LeftHand = new LocomotionService.InputState() {isPitch = true};
        _collector.RightHand = new LocomotionService.InputState() {isPitch = true};
        
        //do random frames
        yield return DoRandomFrames(service);
        
        var objectTransform = _testObject.transform;
        Assert.That(objectTransform.position,Is.EqualTo(Vector3.zero));
        Assert.That(objectTransform.localScale,Is.EqualTo(Vector3.one));
    }
    
    [UnityTest]
    public IEnumerator LocomotionService_TwoPitchPosition()
    {

        var service = _objectResolver.Resolve<ITickable>();
        
        var startLeftHand = Random.insideUnitSphere;
        var initialOffset = Random.insideUnitSphere;
        var startRightHand = startLeftHand + initialOffset;

        var offset = Random.insideUnitSphere;
        
        _collector.LeftHand = new LocomotionService.InputState() {isPitch = true,position = startLeftHand};
        _collector.RightHand = new LocomotionService.InputState() {isPitch = true,position = startRightHand};
        
        //do random frames
        yield return DoRandomFrames(service);

        var objectTransform = _testObject.transform;
        Debug.Log($"{objectTransform.position.x:E} {objectTransform.position.y:E} {objectTransform.position.z:E}");

        Assert.That(objectTransform.position,Is.EqualTo(Vector3.zero).Using(Comparer));
        Assert.That(objectTransform.localScale,Is.EqualTo(Vector3.one).Using(Comparer));
        
        _collector.LeftHand = new LocomotionService.InputState() {isPitch = true,position = startLeftHand+offset};
        _collector.RightHand = new LocomotionService.InputState() {isPitch = true,position = startRightHand+offset};

        //do random frames
        yield return DoRandomFrames(service);
        //we don't move on y axis
        var expectedDirection = Vector3.Scale(offset,new Vector3(1,0,1)).normalized;
        
        Debug.Log($"{expectedDirection.x:E} {expectedDirection.y:E} {expectedDirection.z:E}");

        var actualDirection = objectTransform.position.normalized;
        Debug.Log($"{actualDirection.x:E} {actualDirection.y:E} {actualDirection.z:E}");

        Assert.That(actualDirection,Is.EqualTo(expectedDirection).Using(Comparer),"Testing Direction");
        Assert.That(objectTransform.localScale,Is.EqualTo(Vector3.one).Using(Comparer),"Testing Scale");
    }

    private static IEnumerator DoRandomFrames(ITickable tickable)
    {
        foreach (var i in Enumerable.Range(0, Random.Range(4, 10)))
        {
            tickable.Tick();
            yield return null;
        }
    }
}

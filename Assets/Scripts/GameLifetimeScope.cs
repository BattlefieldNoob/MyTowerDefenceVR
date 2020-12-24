using Components;
using Services;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
#if UNITY_EDITOR
        builder.RegisterComponentInHierarchy<EditorLocomotionCollector>();
#else
        builder.RegisterComponentInHierarchy<LocomotionCollector>();
#endif
        builder.RegisterComponentInHierarchy<LocomotionActuator>();
        builder.RegisterEntryPoint<LocomotionService>(Lifetime.Singleton);
    }
}

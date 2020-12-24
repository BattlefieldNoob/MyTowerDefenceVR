using Services;

namespace Interfaces
{
    public interface ILocomotionCollector
    {
        void UpdateLeftHandState(LocomotionService.InputState state);
        void UpdateRightHandState(LocomotionService.InputState state);
    }
}
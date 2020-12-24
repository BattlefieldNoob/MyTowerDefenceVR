using Services;

namespace Utils
{
    public static class Extensions
    {
        public static LocomotionService.InputState ToInputState(this OVRHand hand)
        {
            return new LocomotionService.InputState()
                {isPitch = hand.GetFingerIsPinching(OVRHand.HandFinger.Index), position = hand.transform.localPosition};
        }
    }
}
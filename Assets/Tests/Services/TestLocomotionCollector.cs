using Interfaces;
using Services;
using UnityEngine;
using VContainer;

namespace Tests.Services
{
    public class TestLocomotionCollector : MonoBehaviour
    {
        public LocomotionService.InputState LeftHand;
        public LocomotionService.InputState RightHand;

        private ILocomotionCollector _collector;

        [Inject]
        void Construct(ILocomotionCollector collector)
        {
            _collector = collector;
        }

        private void Update()
        {
            _collector.UpdateLeftHandState(LeftHand);
            _collector.UpdateRightHandState(RightHand);
        }
    }
}
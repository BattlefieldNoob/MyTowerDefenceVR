using System.Diagnostics.CodeAnalysis;
using Interfaces;
using UnityEngine;
using Utils;
using VContainer;

namespace Components
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class LocomotionCollector : MonoBehaviour
    {
        [SerializeField] private OVRHand LeftHand;
        [SerializeField] private OVRHand RightHand;

        private ILocomotionCollector _collector;

        [Inject]
        void Construct(ILocomotionCollector collector)
        {
            _collector = collector;
        }

        private void Update()
        {
            _collector.UpdateLeftHandState(LeftHand.ToInputState());
            _collector.UpdateRightHandState(RightHand.ToInputState());
        }
    }
}
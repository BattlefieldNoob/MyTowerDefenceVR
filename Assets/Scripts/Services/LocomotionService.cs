using System;
using Components;
using Interfaces;
using UnityEngine;
using VContainer.Unity;

namespace Services
{
    public interface ILocomotion
    {
        void SetScale(float scale);
        void SetPosition(Vector3 position);
        void Reset();
    }

    public class LocomotionService : ILocomotionCollector, ILocomotionActuator, ILocomotion, ITickable
    {
        public struct InputState
        {
            public bool isPitch;
            public Vector3 position;
        }

        struct State
        {
            public Vector3 position;
            public float scale;
        }

        enum LocomotionState
        {
            Waiting,
            Updating
        }

        private InputState RightHand;
        private InputState LeftHand;

        private State output;

        private LocomotionState _state;

        private float OldDistance;

        private Vector3 OldRightPosition;

        private Vector3 OldLeftPosition;

        public void UpdateLeftHandState(InputState state) => LeftHand = state;

        public void UpdateRightHandState(InputState state) => RightHand = state;

        public Vector3 GetPosition()
        {
            //Debug.Log("Returning:" + output.position);
            return output.position;
        }

        public float GetScale() => output.scale;

        private void Initialize()
        {
            //Debug.Log("Initialize!");
            OldDistance = Vector3.Distance(RightHand.position, LeftHand.position);
            OldLeftPosition = LeftHand.position;
            OldRightPosition = RightHand.position;
        }

        private void CalculateOutput()
        {
            var distance = Vector3.Distance(RightHand.position, LeftHand.position);

            //Debug.Log($"Right: {RightHand.position} Left:{LeftHand.position} distance:{distance:E}");
            //Debug.Log($"OldDistance:{OldDistance:E}");

            var signedDelta = distance - OldDistance;
            var delta = Mathf.Abs(signedDelta);

            var leftHandOffset = LeftHand.position-OldLeftPosition;
            var rightHandOffset = RightHand.position-OldRightPosition;

            
            //Debug.Log($"Delta:{delta:E}");
            //Debug.Log($"LeftHand:{leftHandOffset.magnitude:E}");
            //Debug.Log($"RightHand:{rightHandOffset.magnitude:E}");
            if (delta != 0 || leftHandOffset.magnitude > HandOffsetTollerance || rightHandOffset.magnitude > HandOffsetTollerance)
            {
                if (delta < Tollerance && leftHandOffset.magnitude > HandOffsetTollerance &&
                    rightHandOffset.magnitude > HandOffsetTollerance)
                {
                    //Debug.Log("Position!!");
                    //Debug.Log($"Position, Delta:{delta:E}, centerMagnitude:{centrePointMagnitude:E}");
                    //Debug.Log("Position");
                    //In change position mode
                    var difference = (leftHandOffset + rightHandOffset) / 2;
                    //Debug.Log($"Difference:{difference:E}");
                    //Debug.Log($"Output Position Before:{output.position:E}");
                    output.position += new Vector3(difference.x, 0, difference.z) * Time.deltaTime;
                    //Debug.Log($"Output Position:{output.position:E}");
                }
                else
                {
                    //Debug.Log($"Scale, Delta:{delta:E}, centerMagnitude:{centrePointMagnitude:E}");

                    //Debug.Log("Scale");
                    //In change scale mode
                    output.scale += signedDelta * Time.deltaTime;
                }
            }

            OldLeftPosition = LeftHand.position;
            OldRightPosition = RightHand.position;
            OldDistance = distance;
        }

        public double Tollerance { get; } = 0.1f;

        public double HandOffsetTollerance = 0.03f;

        public void Tick()
        {
            if (!RightHand.isPitch || !LeftHand.isPitch)
            {
                //Debug.Log("No Pitch!");
                _state = LocomotionState.Waiting;
                return;
            }

            switch (_state)
            {
                case LocomotionState.Waiting:
                    Initialize();
                    _state = LocomotionState.Updating;
                    break;
                case LocomotionState.Updating:
                    CalculateOutput();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetScale(float scale) => output.scale = scale;

        public void SetPosition(Vector3 position) => output.position = position;

        public void Reset()
        {
            output = new State();
            RightHand = new InputState();
            LeftHand = new InputState();
            OldDistance = 0;
            OldLeftPosition = OldRightPosition = Vector3.zero;
        }
    }
}
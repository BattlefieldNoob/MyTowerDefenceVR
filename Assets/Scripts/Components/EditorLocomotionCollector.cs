using Interfaces;
using Services;
using UnityEngine;
using VContainer;

namespace Components
{
    public class EditorLocomotionCollector : MonoBehaviour
    {
        private ILocomotionCollector _collector;

        [Inject]
        // ReSharper disable once UnusedMember.Local
        void Construct(ILocomotionCollector collector)
        {
            _collector = collector;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                //testing Position
                var mousepos = Input.mousePosition;
                var state = new LocomotionService.InputState()
                    {isPitch = true, position = new Vector3(mousepos.x, 0, mousepos.y)};
                _collector.UpdateLeftHandState(state);
                _collector.UpdateRightHandState(state);
            }
            else if (Input.GetMouseButton(1))
            {
                //testing Scale
                var mousepos = Input.mousePosition;
                var vector3MousePos = new Vector3(mousepos.x, 0, mousepos.y);

                _collector.UpdateLeftHandState(new LocomotionService.InputState()
                    {isPitch = true, position = -vector3MousePos});
                _collector.UpdateRightHandState(new LocomotionService.InputState()
                    {isPitch = true, position = vector3MousePos});
            }
            else
            {
                _collector.UpdateLeftHandState(new LocomotionService.InputState());
                _collector.UpdateRightHandState(new LocomotionService.InputState());
            }
        }
    }
}
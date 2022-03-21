using System.Collections.Generic;
using System.Threading.Tasks;
using RG.EscapeRoom.Interaction.Scripts;
using RG.EscapeRoom.Wiring;
using UnityEngine;
using UnityEngine.XR;

namespace RG.EscapeRoom.Interaction
{
    public class ControllerButtonData : IControllerButtonData
    {
        public Dictionary<IControllerButtonData.Controller, HashSet<IControllerButtonData.Button>> pressedButtons =
            new Dictionary<IControllerButtonData.Controller, HashSet<IControllerButtonData.Button>>();

        public bool IsButtonPressed(IControllerButtonData.Controller controller, IControllerButtonData.Button button)
        {
            return GetControllersPressedButtons(controller).Contains(button);
        }

        public void NotifyButtonPressed(IControllerButtonData.Controller controller,
            IControllerButtonData.Button button)
        {
            GetControllersPressedButtons(controller).Add(button);
            Debug.Log($"Pressed {controller}, {button}");
        }

        public void NotifyButtonReleased(IControllerButtonData.Controller controller,
            IControllerButtonData.Button button)
        {
            GetControllersPressedButtons(controller).Remove(button);
            Debug.Log($"Released {controller}, {button}");
        }

        private HashSet<IControllerButtonData.Button> GetControllersPressedButtons(
            IControllerButtonData.Controller controller)
        {
            if (!pressedButtons.ContainsKey(controller))
                pressedButtons[controller] = new HashSet<IControllerButtonData.Button>();
            return pressedButtons[controller];
        }
    }
}

namespace RG.EscapeRoom.Interaction.Scripts
{
    public interface IControllerButtonData
    {
        public enum Button
        {
            Grip,
            Trigger
        }

        public enum Controller
        {
            Left,
            Right
        }

        public bool IsButtonPressed(Controller controller, Button button);
    }


    public class ControllerButtonListener : ITickable
    {
        private readonly ControllerButtonData controllerButtonData;
        private List<InputDevice> leftHandInputDevices;
        private List<InputDevice> rightHandInputDevices;

        public ControllerButtonListener(ControllerButtonData controllerButtonData)
        {
            this.controllerButtonData = controllerButtonData;
        }

        public void Tick()
        {
            for (var i = 0; i < leftHandInputDevices.Count; i++)
            {
                ReadButton(leftHandInputDevices[i], CommonUsages.triggerButton, IControllerButtonData.Controller.Left,
                    IControllerButtonData.Button.Trigger);
                ReadButton(leftHandInputDevices[i], CommonUsages.gripButton, IControllerButtonData.Controller.Left,
                    IControllerButtonData.Button.Grip);
            }

            for (var i = 0; i < rightHandInputDevices.Count; i++)
            {
                ReadButton(rightHandInputDevices[i], CommonUsages.triggerButton, IControllerButtonData.Controller.Right,
                    IControllerButtonData.Button.Trigger);
                ReadButton(rightHandInputDevices[i], CommonUsages.gripButton, IControllerButtonData.Controller.Right,
                    IControllerButtonData.Button.Grip);
            }
        }

        public async Task Initialize()
        {
            var initialized = false;
            while (!initialized)
            {
                leftHandInputDevices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, leftHandInputDevices);
                rightHandInputDevices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(
                    InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, rightHandInputDevices);
                if (leftHandInputDevices.Count > 0)
                    initialized = true;
                else
                    await Task.Yield();
            }
        }

        private void ReadButton(InputDevice device, InputFeatureUsage<bool> xrButtonId,
            IControllerButtonData.Controller controller, IControllerButtonData.Button internalButtonId)
        {
            bool buttonValue;
            if (device.TryGetFeatureValue(xrButtonId, out buttonValue))
            {
                if (buttonValue)
                    controllerButtonData.NotifyButtonPressed(controller, internalButtonId);
                else
                    controllerButtonData.NotifyButtonReleased(controller, internalButtonId);
            }
        }
    }
}
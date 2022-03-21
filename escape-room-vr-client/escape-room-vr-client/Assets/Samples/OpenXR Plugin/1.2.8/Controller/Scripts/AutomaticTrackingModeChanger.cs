﻿using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample
{
    public class AutomaticTrackingModeChanger : MonoBehaviour
    {
        private static List<XRInputSubsystem> s_InputSubsystems = new List<XRInputSubsystem>();

        private static readonly List<TrackingOriginModeFlags> s_SupportedTrackingOriginModes =
            new List<TrackingOriginModeFlags>();

        [SerializeField] private float m_ChangeInterval = 5.0f;

        private float m_TimeRemainingTillChange;

        private void Update()
        {
            m_TimeRemainingTillChange -= Time.deltaTime;
            if (m_TimeRemainingTillChange <= 0.0f)
            {
                var inputSubsystems = new List<XRInputSubsystem>();
                SubsystemManager.GetInstances(inputSubsystems);
                var subsystem = inputSubsystems?[0];
                if (subsystem != null)
                {
                    UpdateSupportedTrackingOriginModes(subsystem);
                    SetToNextMode(subsystem);
                }

                m_TimeRemainingTillChange += m_ChangeInterval;
            }
        }

        private void OnEnable()
        {
            m_TimeRemainingTillChange = m_ChangeInterval;
        }

        private void UpdateSupportedTrackingOriginModes(XRInputSubsystem subsystem)
        {
            var supportedOriginModes = subsystem.GetSupportedTrackingOriginModes();
            s_SupportedTrackingOriginModes.Clear();
            for (var i = 0; i < 31; i++)
            {
                var modeToCheck = 1u << i;
                if ((modeToCheck & (uint) supportedOriginModes) != 0)
                    s_SupportedTrackingOriginModes.Add((TrackingOriginModeFlags) modeToCheck);
            }
        }

        private void SetToNextMode(XRInputSubsystem subsystem)
        {
            var currentOriginMode = subsystem.GetTrackingOriginMode();
            for (var i = 0; i < s_SupportedTrackingOriginModes.Count; i++)
                if (currentOriginMode == s_SupportedTrackingOriginModes[i])
                {
                    var nextModeIndex = (i + 1) % s_SupportedTrackingOriginModes.Count;
                    subsystem.TrySetTrackingOriginMode(s_SupportedTrackingOriginModes[nextModeIndex]);
                    break;
                }
        }
    }
}
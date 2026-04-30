using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.XR;

#if UNITY_XR_MANAGEMENT
using UnityEngine.XR.Management;
#endif

namespace FrameVR.Player
{
    public class PlayerRigSelector : MonoBehaviour
    {
        [Title("Rigs")]
        [SerializeField] private GameObject devFpVrRig;
        [SerializeField] private GameObject xrOriginRig;

        [Title("Settings")]
        [SerializeField] private float xrDetectionDelay = 0.25f;

        [Title("Debug")]
        [SerializeField, ReadOnly] private bool xrAvailable;
        [SerializeField, ReadOnly] private string selectedRig;

        private IEnumerator Start()
        {
            devFpVrRig.SetActive(false);
            xrOriginRig.SetActive(false);

            yield return new WaitForSeconds(xrDetectionDelay);

            xrAvailable = IsVrPresent();

            if (xrAvailable)
                ActivateXr();
            else
                ActivateDev();
        }

        private bool IsVrPresent()
        {
#if UNITY_XR_MANAGEMENT
            var manager = XRGeneralSettings.Instance?.Manager;
            if (manager == null || manager.activeLoader == null)
                return false;
#endif

            List<InputDevice> devices = new();
            InputDevices.GetDevicesWithCharacteristics(
                InputDeviceCharacteristics.HeadMounted,
                devices
            );

            foreach (InputDevice device in devices)
            {
                if (device.isValid)
                    return true;
            }

            return false;
        }

        private void ActivateXr()
        {
            xrOriginRig.SetActive(true);
            devFpVrRig.SetActive(false);
            selectedRig = "XR Origin";
        }

        private void ActivateDev()
        {
            devFpVrRig.SetActive(true);
            xrOriginRig.SetActive(false);
            selectedRig = "DevFP-VR";
        }
    }
}
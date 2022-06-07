using System.Collections;
using System.Linq;
using Neurorehab.Scripts.Devices.Abstracts;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Represents a <see cref="GenericDeviceData"/> Id in the GUI
    /// </summary>
    public class GenericDeviceIdMapper : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="DevicePrefabManager"/> used by this class
        /// </summary>
        [SerializeField]
        private DevicePrefabManager _devicePrefabManager;

        /// <summary>
        /// The <see cref="DevicePrefabManager"/> used by this class
        /// </summary>
        public DevicePrefabManager DevicePrefabManager 
        {
            get { return _devicePrefabManager; }
            set { _devicePrefabManager = value; }
        }
        
        private void Start()
        {
            StartCoroutine(UpdateGenericDeviceData());
        }

        /// <summary>
        /// Class <see cref="UpdateOrRemoveGenericDeviceData"/> once per frame, as long as the <see cref="GenericDeviceData"/> is null or if it has no values.
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateGenericDeviceData()
        {
            while (true)
            {
                if (DevicePrefabManager.GenericDeviceData == null || DevicePrefabManager.GenericDeviceData.HasAnyValue() == false)
                {
                    UpdateOrRemoveGenericDeviceData();
                }
                yield return null;
            }
        }

        /// <summary>
        /// Clears all the <see cref="SingleInputGui"/> objects that belong to the <see cref="DevicePrefabManager"/> -> <see cref="Gui.DevicePrefabManager"/> and removes them from the <see cref="MapperManager.DeviceInputConnections"/>
        /// </summary>
        private void ClearAllSingleInputsConnections()
        {
            if (DevicePrefabManager.CheckIfIsLastDeviceOfItsType())
                GenericDeviceGuiManager.Instance.RemoveDeviceFromList(DevicePrefabManager.GenericDeviceName);

            foreach (var deviceSingleInput in DevicePrefabManager.DeviceSingleInputs)
            {
                MapperManager.ClearSingleInputConnections(deviceSingleInput);
            }
        }

        /// <summary>
        /// Called from Unity GUI. Toggles its DeviceUnityGo Prefab
        /// </summary>
        public void UpdateShowTarget()
        {
            DevicePrefabManager.gameObject.SetActive(!DevicePrefabManager.gameObject.activeSelf);
        }

        /// <summary>
        /// Gets another <see cref="GenericDeviceData"/> from <see cref="DevicesController"/> List. If there is one, Updates all <see cref="SingleInputGui"/> with the new <see cref="GenericDeviceData"/>.
        /// <para>If there is none, Removes all <see cref="SingleInputGui"/>'s <see cref="GenericDeviceData"/> from the <see cref="Gui.DevicePrefabManager"/></para>
        /// </summary>
        public void UpdateOrRemoveGenericDeviceData()
        {
            var oldGddName = DevicePrefabManager.GenericDeviceName;
            GenericDeviceData newGdd = null;

            foreach (var genericDeviceData in DevicesController.Instance.DevicesData.Values.ToList().OrderByDescending(g => g.LastUpdate))
            {
                if (genericDeviceData.DeviceName != oldGddName) continue;
                newGdd = genericDeviceData;
                break;
            }

            if (newGdd != null)
                DevicePrefabManager.UpdateGenericDeviceDataGui(newGdd);
            else
                DevicePrefabManager.RemoveGenericDeviceData();
        }
    }
}
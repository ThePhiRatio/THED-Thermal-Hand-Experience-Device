using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Udp;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper
{
    public class DevicesController : GenericDeviceController
    {
        public static DevicesController Instance { get; set; }

        /// <summary>
        /// The name of all the different devices.
        /// </summary>
        internal List<string> Devices { get; set; }

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
            Devices = new List<string>();
        }

        protected override void Start()
        {
            StartCoroutine(WaitForObjectToBeEnabled());
        }

        /// <summary>
        /// Called when using the prefab interface to wait until the canvas is enabled
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForObjectToBeEnabled()
        {
            //wwait for prefab to activate canvas
            while (DeviceMapper.Instance.OverlayManager.OverlayCanvas.gameObject.activeSelf == false)
            {
                yield return null;
            }

            StartCoroutine(CheckIfUnityObjectExists());
        }
        
        /// <summary>
        /// A Coroutine used to destroy outdated GameObjects and instantiate new GameObjects. Runs every <see cref="GenericDeviceController.RefreshRate"/> seconds.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator CheckIfUnityObjectExists()
        {
            while (true)
            {
                //print("looking for stuff");
                DestroyOld();

                foreach (var genericDevice in UdpGenericTranslator.DevicesValues)
                {
                    if (Devices.Contains(genericDevice.DeviceName) == false)
                        Devices.Add(genericDevice.DeviceName);
                }
                
                foreach (var deviceName in Devices)
                {
                    if (UdpGenericTranslator.Devices.ContainsKey(deviceName))
                    {
                        var genericDevice = UdpGenericTranslator.Devices[deviceName];
                        //print("instantiating new stuff");
                        CreateNewUnityObject(genericDevice);
                    }

                    var remove = new List<string>();
                    foreach (var devicesDataValue in DevicesData)
                    {
                        if (devicesDataValue.Value.HasAnyValue() == false)
                        {
                            //print("removing key: " + devicesDataValue.Key);
                            remove.Add(devicesDataValue.Key);
                        }
                    }

                    foreach (var key in remove)
                    {
                        DevicesData.Remove(key);
                    }
                }
                yield return new WaitForSecondsRealtime(1/60f);
                //yield return null;
            }
        }

        /// <summary>
        /// If <see cref="GenericDeviceGuiManager.Instance"/> is null, returns.
        /// <para>If not, then calls <see cref="GenericDeviceGuiManager.PopulateDeviceGui"/></para>
        /// </summary>
        /// <param name="genericDeviceData">The <see cref="GenericDeviceData"/> containing the data for this device.</param>
        /// <param name="index">Not used in this implementation</param>
        protected override void InstantiateUnityObject(GenericDeviceData genericDeviceData, int index = 0)
        {
            //not showing the prefab window
            if (GenericDeviceGuiManager.Instance == null) return;

            //Debug.Log("creating new: " + genericDeviceData.DeviceName + " " + genericDeviceData.Id);
            GenericDeviceGuiManager.Instance.PopulateDeviceGui(genericDeviceData);
        }

        /// <summary>
        /// Destroys the GameObjects associated with <see cref="GenericDeviceData"/> that are older than <see cref="GenericDeviceController.TimeToLive"/>.
        /// </summary>
        protected override void DestroyOld()
        {
            foreach (var device in DevicesData)
            {
                //if ((DateTime.Now - device.Value.LastUpdate).TotalSeconds < TimeToLive) continue;
                //if (UdpGenericTranslator.Devices.ContainsKey(device.Value.DeviceName))
                //{
                //    var allIds = UdpGenericTranslator.Devices[device.Value.DeviceName].GetDeviceIds();
                //    //if ((DateTime.Now - device.Value.LastUpdate).TotalSeconds < TimeToLive) continue;
                //    if (allIds.Contains(device.Value.Id)) continue;
                //}
                if (device.Value.HasAnyValue()) continue;

                //apagar device da lista
                var deviceCopy = device;
                if (DevicesData.Values.Count(d => d.DeviceName == deviceCopy.Value.DeviceName) == 0)
                {
                    Devices.Remove(device.Value.DeviceName);
                }

                if(DeviceMapper.Instance.UsingMultiplayer)
                    DestroyGenericDeviceDataUnityObj(device.Value);
                else
                    RemoveGenericDeviceDataFromUnityObj(device.Value);
            }
        }
        
        /// <summary>
        /// Support function, usually called from <see cref="M:Neurorehab.Scripts.DeviceMapper.DevicesController.CheckIfUnityObjectExists" />. First it creates a new <see cref="T:Neurorehab.Scripts.Devices.Abstracts.GenericDeviceData" /> according to the <see cref="T:Neurorehab.Scripts.Udp.GenericDevice" /> received as a parameter. Then, it instantiate a Unity object for each new detection for devices of this type.
        /// </summary>
        /// <param name="genericDevice">The device being checked.</param>
        protected override void CreateNewUnityObject(GenericDevice genericDevice)
        {
            //foreach (var deviceId in genericDevice.GetDeviceIds())
            //{
            //    var values = genericDevice.GetLastReceivedStringValues(deviceId);
            //    var genericDeviceData = CreateGenericDeviceData(genericDevice, values);
            //    AddDeviceDataToList(values.Id, genericDeviceData);

            //    InstantiateUnityObject(genericDeviceData);
            //}

            foreach (var values in genericDevice.GetNewDetections(DevicesData.Keys.ToList()))
            {
                var genericDeviceData = CreateGenericDeviceData(genericDevice.DeviceName, values);
                genericDeviceData.ProcessData(values);
                AddDeviceDataToList(values.Id, genericDeviceData);

                InstantiateUnityObject(genericDeviceData);
                //Debug.Log("new detection " + genericDevice.DeviceName + " " + values.Id + " time: " + values.LastTimeReceived);
            }

            var devicePrefabManager = GenericDeviceGuiManager.Instance.AllPrefabManagers.FirstOrDefault(pref =>
                pref.GenericDeviceName == genericDevice.DeviceName);

            if (devicePrefabManager != null && devicePrefabManager.GenericDeviceData != null && devicePrefabManager.GenericDeviceData.IsReceiving == false)
            {
                //Debug.Log("looking for new gdd");
                var gdd = DevicesData.Values.FirstOrDefault(dd => dd.IsReceiving && dd.DeviceName == devicePrefabManager.GenericDeviceName);
                
                if (gdd == null) return;


                //Debug.Log("found new gdd " + devicePrefabManager.GenericDeviceName);
                devicePrefabManager.UpdateGenericDeviceDataGui(gdd);

                //RemoveGenericDeviceDataFromUnityObj(devicePrefabManager.GenericDeviceData);
                //InstantiateUnityObject(gdd);
            }
        }

        /// <summary>
        /// Destroys the UnityObject that is currently being mapped by the <c>GenericDeviceData</c> and destroys everything that is connected to it, including The <c>SingleInput</c> connections
        /// </summary>
        /// <param name="device">The specific <see cref="GenericDeviceData"/> that has to destroys its GUI Gameobjects</param>
        private void DestroyGenericDeviceDataUnityObj(GenericDeviceData device)
        {
            Debug.Log("destroying " + device.UnityObject.name);
            DestroyImmediate(device.UnityObject);
        }

        /// <summary>
        /// Removes the <see cref="GenericDeviceData"/> from its UnityObject but soes not destroy its connections between its <c>SingleInputs</c> and Unity Gameobjects
        /// </summary>
        /// <param name="device">The specific <see cref="GenericDeviceData"/> that has to be removed from its GUI gameobjects</param>
        private void RemoveGenericDeviceDataFromUnityObj(GenericDeviceData device)
        {
            //Debug.Log("removing " + device.Id);
            if (device == device.UnityObject.GetComponent<GenericDeviceIdMapper>().DevicePrefabManager.GenericDeviceData)
                device.UnityObject.GetComponent<GenericDeviceIdMapper>().DevicePrefabManager.RemoveGenericDeviceData(); 
        }
    }
}
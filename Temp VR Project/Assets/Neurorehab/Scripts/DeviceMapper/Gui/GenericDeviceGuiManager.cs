using System.Collections.Generic;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Devices.Data;
using Neurorehab.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Resposible for managing the GenericDevices in the GUI that are being received by UDP. It is also responsible for showing the Currently Selected GenericDevice Panel
    /// </summary>
    public class GenericDeviceGuiManager : MonoBehaviour
    {
        [Header("DeviceMenu Panel")]
        public GameObject AvailabelDevicesPanel;

        [Header("Devices Panels")]
        public GameObject GenericPanel;
        public GameObject GenericParentPanel;
        public GameObject BitalinoPanel;
        public GameObject BiopluxPanel;
        public GameObject KinectPanel;
        public GameObject LeapMotionPanel;
        public GameObject NeuroskyPanel;
        public GameObject TobiiPanel;
        public GameObject EmotivPanel;
        public GameObject OcculusPanel;

        [Header("Device Prefabs")]
        public GameObject GenericDevicePrefab;
        public GameObject BitalinoDevicePrefab;
        public GameObject BiopluxDevicePrefab;
        public GameObject KinectDevicePrefab;
        public GameObject LeapMotionDevicePrefabLeft;
        public GameObject LeapMotionDevicePrefabRight;
        public GameObject NeuroskyDevicePrefab;
        public GameObject TobiiDevicePrefab;
        public GameObject EmotivDevicePrefab;
        public GameObject OcculusDevicePrefab;

        [Header("Other Prefabs")]
        public GameObject IdBtnPrefab;
        public GameObject GenericSingleInputPrefab;
        public GameObject AvailabelDeviceBtnPrefab;


        public static GenericDeviceGuiManager Instance;
        /// <summary>
        /// Reference to the last shown panel in the GUI. This variable is changed inside the<see cref="ShowPanel"/> function
        /// </summary>
        private GameObject _lastShownPanel;

        /// <summary>
        /// The name  current selected device
        /// </summary>
        public string CurrentSelectedDeviceName { get; set; }
        /// <summary>
        /// List of all prefab managers already instantiated
        /// </summary>
        public List<DevicePrefabManager> AllPrefabManagers { get; set; }

        private void Awake()
        {
            Instance = this;

            AllPrefabManagers = new List<DevicePrefabManager>();
            BiopluxPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            BitalinoPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            NeuroskyPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            KinectPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            LeapMotionPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            EmotivPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            TobiiPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            OcculusPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            GenericParentPanel.GetComponent<RectTransform>().localScale = Vector3.zero;

            BiopluxPanel.transform.Find("List").DestroyChildren();
            BiopluxPanel.transform.Find("DeviceList").DestroyChildren();
            BitalinoPanel.transform.Find("List").DestroyChildren();
            BitalinoPanel.transform.Find("DeviceList").DestroyChildren();
            NeuroskyPanel.transform.Find("List").DestroyChildren();
            NeuroskyPanel.transform.Find("DeviceList").DestroyChildren();
            KinectPanel.transform.Find("List").DestroyChildren();
            KinectPanel.transform.Find("DeviceList").DestroyChildren();
            LeapMotionPanel.transform.Find("List").DestroyChildren();
            LeapMotionPanel.transform.Find("DeviceList").DestroyChildren();
            EmotivPanel.transform.Find("List").DestroyChildren();
            EmotivPanel.transform.Find("DeviceList").DestroyChildren();
            TobiiPanel.transform.Find("List").DestroyChildren();
            TobiiPanel.transform.Find("DeviceList").DestroyChildren();
            OcculusPanel.transform.Find("List").DestroyChildren();
            OcculusPanel.transform.Find("DeviceList").DestroyChildren();

            GenericParentPanel.transform.DestroyChildren();
            AvailabelDevicesPanel.transform.DestroyChildren();
        }

        /// <summary>
        /// Shows the device Panel
        /// </summary>
        /// <param name="deviceName">Name of the device to show</param>
        public void ShowDeviceInformationPanel(string deviceName)
        {
            CurrentSelectedDeviceName = deviceName;

            GenericParentPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
            //show device info panel
            if (deviceName == Enums.Devices.neurosky.ToString())
                ShowPanel(NeuroskyPanel);
            else if (deviceName == Enums.Devices.leapmotion.ToString())
                ShowPanel(LeapMotionPanel);
            else if (deviceName == Enums.Devices.tobiieyex.ToString())
                ShowPanel(TobiiPanel);
            else if (deviceName == Enums.Devices.kinect.ToString())
                ShowPanel(KinectPanel);
            else if (deviceName == Enums.Devices.bioplux.ToString())
                ShowPanel(BiopluxPanel);
            else if (deviceName == Enums.Devices.oculus.ToString())
                ShowPanel(OcculusPanel);
            else if (deviceName == Enums.Devices.emotiv.ToString())
                ShowPanel(EmotivPanel);
            else if (deviceName == Enums.Devices.bitalino.ToString())
                ShowPanel(BitalinoPanel);
            else
            {
                GenericParentPanel.GetComponent<RectTransform>().localScale = Vector3.one;
                //GenericParentPanel.SetActive(true);
                ShowPanel(GetUnknownDevicePanel(deviceName));
            }
            

        }
        

        /// <summary>
        /// Returns a list of all the devices that are alredy being shown in the top menu
        /// </summary>
        /// <returns></returns>
        public List<string> GetAlreadyShowingDevices()
        {
            var availableDevs = new List<string>();

            for (var i = 0; i < AvailabelDevicesPanel.transform.childCount; i++)
            {
                availableDevs.Add(AvailabelDevicesPanel.transform.GetChild(i).name);
            }
            return availableDevs;
        }

        /// <summary>
        /// Removes the device from the GUI List
        /// </summary>
        public void RemoveDeviceFromList(string deviceName)
        {
            Transform buttonToDestroy = null;

            foreach (Transform child in AvailabelDevicesPanel.transform)
            {
                if (child.name != deviceName) continue;
                buttonToDestroy = child;
            }

            if (buttonToDestroy != null)
                Destroy(buttonToDestroy.gameObject);
        }

        /// <summary>
        /// Populates the <see cref="GenericDeviceData"/> in the GUI. Creates the prefab with all the <see cref="SingleInputGui"/> belonging to it.
        /// </summary>
        /// <param name="gdd"></param>
        public void PopulateIdsList(GenericDeviceData gdd)
        {
            var prefabIdParent = GetDevicePanel(gdd.DeviceName).transform.Find("List");

            var deviceListParent = GetDevicePanel(gdd.DeviceName).transform.Find("DeviceList");
            
            if (DeviceMapper.Instance.UsingMultiplayer || prefabIdParent.childCount == 0)
            {
                //add device prefab
                var device = Instantiate(GetDevicePrefab(gdd));
                var prefabManager = device.GetComponent<DevicePrefabManager>();

                AllPrefabManagers.Add(prefabManager);
                
                prefabManager.UpdateGenericDeviceDataGui(gdd);
                device.transform.SetParent(deviceListParent, false);
                
                //add id button
                var idBtnPrefab = Instantiate(IdBtnPrefab);
                gdd.UnityObject = idBtnPrefab;

                idBtnPrefab.name = gdd.Id;
                idBtnPrefab.GetComponent<GenericDeviceIdMapper>().DevicePrefabManager =
                    device.GetComponent<DevicePrefabManager>();
                idBtnPrefab.transform.SetParent(prefabIdParent, false);

                device.SetActive(device.transform.GetSiblingIndex() == 1);
                idBtnPrefab.GetComponentInChildren<Text>().text = prefabIdParent.childCount.ToString();
                idBtnPrefab.GetComponent<Toggle>().isOn = prefabIdParent.childCount == 1;
                idBtnPrefab.GetComponent<Toggle>().group = prefabIdParent.GetComponent<ToggleGroup>();
            }
            else
            {
                var idBtn = prefabIdParent.GetChild(0);
                gdd.UnityObject = idBtn.gameObject;
                idBtn.name = gdd.Id;

                if (idBtn.GetComponent<GenericDeviceIdMapper>().DevicePrefabManager.GenericDeviceData == null || idBtn.GetComponent<GenericDeviceIdMapper>().DevicePrefabManager.GenericDeviceData.IsReceiving == false)
                {
                    idBtn.GetComponent<GenericDeviceIdMapper>().UpdateOrRemoveGenericDeviceData();
                }
            }
        }

        /// <summary>
        /// Returns the correct Device panel according to its name
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        private GameObject GetDevicePanel(string deviceName)
        {
            if (deviceName == Enums.Devices.neurosky.ToString())
                return NeuroskyPanel;
            if (deviceName == Enums.Devices.leapmotion.ToString())
                return LeapMotionPanel;
            if (deviceName == Enums.Devices.tobiieyex.ToString())
                return TobiiPanel;
            if (deviceName == Enums.Devices.kinect.ToString())
                return KinectPanel;
            if (deviceName == Enums.Devices.bioplux.ToString())
                return BiopluxPanel;
            if (deviceName == Enums.Devices.oculus.ToString())
                return OcculusPanel;
            if (deviceName == Enums.Devices.emotiv.ToString())
                return EmotivPanel;
            if (deviceName == Enums.Devices.bitalino.ToString())
                return BitalinoPanel;

            return GetUnknownDevicePanel(deviceName);
        }

        /// <summary>
        /// Returns the Device panel with the specified deviceName. If there is none, it creates a new panel and returns it
        /// </summary>
        /// <param name="deviceName">The device name</param>
        /// <returns>Returns the Device panel</returns>
        private GameObject GetUnknownDevicePanel(string deviceName)
        {
            foreach (Transform child in GenericParentPanel.transform)
            {
                if (child.name != deviceName) continue;
                return child.gameObject;
            }

            return CreateNewUnknowPanel(deviceName);
        }

        /// <summary>
        /// Creates a new Generic Panel and adds it to the generic parent panel
        /// </summary>
        /// <param name="deviceName">The name of the Device</param>
        /// <returns></returns>
        private GameObject CreateNewUnknowPanel(string deviceName)
        {
            var panel = Instantiate(GenericPanel);
            panel.name = deviceName;

            panel.transform.SetParent(GenericParentPanel.transform, false);

            return panel;
        }

        /// <summary>
        /// Returns the correct prefab according to the DeviceName given.
        /// </summary>
        /// <param name="gdd">The <see cref="GenericDeviceData"/> to be used to get the prefab.</param>
        /// <returns></returns>
        private GameObject GetDevicePrefab(GenericDeviceData gdd)
        {
            if (gdd.DeviceName == Enums.Devices.neurosky.ToString())
                return NeuroskyDevicePrefab;
            if (gdd.DeviceName == Enums.Devices.leapmotion.ToString())
                return GetLeapmotionHand(gdd);
            if (gdd.DeviceName == Enums.Devices.tobiieyex.ToString())
                return TobiiDevicePrefab;
            if (gdd.DeviceName == Enums.Devices.kinect.ToString())
                return KinectDevicePrefab;
            if (gdd.DeviceName == Enums.Devices.bioplux.ToString())
                return BiopluxDevicePrefab;
            if (gdd.DeviceName == Enums.Devices.oculus.ToString())
                return OcculusDevicePrefab;
            if (gdd.DeviceName == Enums.Devices.emotiv.ToString())
                return EmotivDevicePrefab;
            if (gdd.DeviceName == Enums.Devices.bitalino.ToString())
                return BitalinoDevicePrefab;

            return GenericDevicePrefab;
        }

        /// <summary>
        /// Returns the leapmotion panel according to the hand side
        /// </summary>
        private GameObject GetLeapmotionHand(GenericDeviceData gdd)
        {
            return ((LeapMotionData)gdd).LeftHanded ? LeapMotionDevicePrefabLeft : LeapMotionDevicePrefabRight;
        }

        /// <summary>
        /// Shows the given panel. Hides the <see cref="_lastShownPanel"/> as well.
        /// </summary>
        /// <param name="panelToShow">Panel that is going to be show</param>
        private void ShowPanel(GameObject panelToShow)
        {
            if (panelToShow == _lastShownPanel) return;

            if (_lastShownPanel != null)
                _lastShownPanel.GetComponent<RectTransform>().localScale = Vector3.zero;

            panelToShow.GetComponent<RectTransform>().localScale = Vector3.one;
            _lastShownPanel = panelToShow;

        }

        /// <summary>
        /// It instantiates a Unity Object in the GUI. If it is the first device of its type, creates a DeviceButton on the top menu of the GUI. It then updates the device GUI according to its id and data received
        /// </summary>
        /// <param name="genericDeviceData">The <see cref="GenericDeviceData"/> containing the data for this device.</param>
        public void PopulateDeviceGui(GenericDeviceData genericDeviceData)
        {
            //criates the device button in the available devices menu (top)
            if (GetAlreadyShowingDevices().Contains(genericDeviceData.DeviceName) == false)
            {
                var deviceBtn = Instantiate(AvailabelDeviceBtnPrefab);
                deviceBtn.name = genericDeviceData.DeviceName;
                deviceBtn.GetComponentInChildren<Text>().text = genericDeviceData.DeviceName.ToString();
                deviceBtn.GetComponent<Toggle>().group = AvailabelDevicesPanel
                    .GetComponent<ToggleGroup>();

                deviceBtn.transform.SetParent(AvailabelDevicesPanel.transform, false);

                //if it is the first device being received, shows the gui for it
                if (CurrentSelectedDeviceName == null)
                {
                    AvailabelDevicesPanel.transform.Find(genericDeviceData.DeviceName).GetComponent<Toggle>()
                        .isOn = true;
                }
            }
            //populate ids
            PopulateIdsList(genericDeviceData);
        }
    }
}
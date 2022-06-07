using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Represents a <see cref="GenericDeviceData"/> Data in the GUI and knows all its <see cref="SingleInputGui"/>
    /// </summary>
    public class DevicePrefabManager : MonoBehaviour
    {
        /// <summary>
        /// The backfield of the <see cref="GenericDeviceData"/>
        /// </summary>
        private GenericDeviceData _genericDeviceData;

        /// <summary>
        /// The <see cref="SingleInputGui"/> inside the <see cref="DevicePrefabManager"/> gameobject
        /// </summary>
        public List<SingleInputGui> DeviceSingleInputs;
        /// <summary>
        /// A reference to the DataPanel in the Hierarchy of the <see cref="DevicePrefabManager"/> gameobject
        /// </summary>
        public GameObject DataPanel;
        /// <summary>
        /// A reference to the DataListParent in the Hierarchy of the <see cref="DevicePrefabManager"/> gameobject where it will list all the <see cref="SingleInputGui"/> not yet in the <see cref="DeviceSingleInputs"/>
        /// </summary>
        public GameObject DataListParent;

        /// <summary>
        /// The <see cref="GenericDeviceData"/> to which it belongs to. 
        /// </summary>
        public GenericDeviceData GenericDeviceData
        {
            get { return _genericDeviceData; }
            set
            {
                _genericDeviceData = value;

                if (_genericDeviceData != null)
                    GenericDeviceName = _genericDeviceData.DeviceName;
            }
        }
        
        /// <summary>
        /// Returns the prefabs <see cref="Devices.Abstracts.GenericDeviceData"/> Name
        /// </summary>
        /// <returns></returns>
        public string GenericDeviceName { get; set; }

        private void Update()
        {
            if (DeviceSingleInputs.Count == 0) return;
            UpdateConnections();
        }

        private void Awake()
        {
            DataListParent.transform.DestroyChildren(); 
        }
        private void Start()
        {
            UpdateConnections();
            StartCoroutine(UpdateSingleInputList());
        }

        /// <summary>
        /// Updates the <see cref="Devices.Abstracts.GenericDeviceData"/> of all <see cref="SingleInputGui"/> in the list <see cref="DeviceSingleInputs"/>
        /// </summary>
        /// <param name="gdd"></param>
        public void UpdateGenericDeviceDataGui(GenericDeviceData gdd)
        {
            GenericDeviceData = gdd;

            foreach (var deviceSingleInput in DeviceSingleInputs)
            {
                deviceSingleInput.SingleInput.GenericDeviceData = gdd;
            }
        }
        
        /// <summary>
        /// Checks if it is the last prefab that exists of its own type. True if it is the last
        /// </summary>
        /// <returns></returns>
        public bool CheckIfIsLastDeviceOfItsType()
        {
            //return DevicesController.Instance.Devices.Count(dv => dv == GenericDeviceData.DeviceName) == 1;
            return DevicesController.Instance.DevicesData.Values.Count(dv => dv.DeviceName == GenericDeviceData.DeviceName) == 1;
        }
        
        /// <summary>
        /// Sets the singleInput gameobject active or desactive. Checks if it is receiving it by UDP
        /// </summary>
        public void UpdateConnections()
        {
            foreach (var singleInput in DeviceSingleInputs)
            {
                if(singleInput == null) continue;
                singleInput.ShowOrHideGameobject(); 
            }
        }

        /// <summary>
        /// Removes the GenericDeviceData from all the SIngleInputs
        /// </summary>
        public void RemoveGenericDeviceData()
        {
            GenericDeviceData = null;

            foreach (var deviceSingleInput in DeviceSingleInputs)
            {
                deviceSingleInput.SingleInput.GenericDeviceData = null;
            }
        }
        
        /// <summary>
        /// Updates the data List for this device every .3 seconds
        /// </summary>
        private IEnumerator UpdateSingleInputList()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(.3f);
                if (GenericDeviceData == null) continue;

                AddAllSingleInputs();
            }
        }

        /// <summary>
        /// Add all SingleInputs for this <see cref="GenericDeviceData"/>
        /// </summary>
        private void AddAllSingleInputs()
        {
            PopulateDeviceDataSingleInputGuis(GenericDeviceData.GetLabelsList());
        }

        /// <summary>
        /// Add all SingleInputs in the received <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="inputs">All the names of the <see cref="SingleInput"/> to instantiate</param>
        public void PopulateDeviceDataSingleInputGuis(IEnumerable<string> inputs)
        {
            foreach (var inputLabel in inputs)
            {
                if (DeviceSingleInputs.Any(si => si.Label == inputLabel)) continue;

                AddNewSingleInput(inputLabel);
            }
        }

        /// <summary>
        /// Adds a new generic <see cref="SingleInputGui"/> to the Unity Gui Device Data List
        /// </summary>
        /// <param name="label">Label of the <see cref="SingleInputGui"/></param>
        public void AddNewSingleInput(string label)
        {
            //creates the prefab
            var genericSingleInp = Instantiate(GenericDeviceGuiManager.Instance.GenericSingleInputPrefab);

            var singleInputGui = genericSingleInp.GetComponentInChildren<SingleInputGui>();
            singleInputGui.SingleInput.GenericDeviceData = GenericDeviceData;
            singleInputGui.SingleInput.Label = label;

            //print("correting label for: " + label);

            genericSingleInp.GetComponentInChildren<Text>().text = singleInputGui.GetComponentInChildren<Text>().text = label;
            
            genericSingleInp.transform.SetParent(DataListParent.transform, false);

            //adds the SIngleINput to the list
            DeviceSingleInputs.Add(singleInputGui);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Udp;
using Neurorehab.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.CpDebugger.Gui
{
    public class CpDebuggerGuiManager : MonoBehaviour
    {
        /// <summary>
        /// The name of the last Device selected in the GUI
        /// </summary>
        private string _lastDeviceSelected;
        /// <summary>
        /// The name of the last Id selected in the GUI
        /// </summary>
        private string _lastIdSelected;
        /// <summary>
        /// The name of the last Category selected in the GUI
        /// </summary>
        private string _lastCategorySelected;
        /// <summary>
        /// The name of the last Label selected in the GUI
        /// </summary>
        private string _lastLabelSelected;
        /// <summary>
        /// The name of the last Type selected in the GUI
        /// </summary>
        private string _lastTypeSelected;
        

        [Header("Prefabs")]
        public GameObject DeviceBtn;
        public GameObject IdBtn;
        public GameObject LabelBtn;
        public GameObject TypeBtn;
        public GameObject CategoryBtn;

        [Header("Panels")]
        public GameObject DevicesPanel;
        public GameObject IdsPanel;
        public GameObject CategoriesPanel;
        public GameObject LabelsPanel;
        public GameObject ParamsTextPanel;
        public GameObject TypesPanel;
        public GameObject ValuesTextPanel;
        public GameObject ValuesText;

        public static CpDebuggerGuiManager Instance { get; set; }

        /// <summary>
        /// If the Debugger pannel is hidden or not
        /// </summary>
        public bool IsHidden;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Clears all Panels and starts the <see cref="UpdateLists"/> coroutine
        /// </summary>
        private void Start()
        {
            DevicesPanel.transform.DestroyChildren();
            IdsPanel.transform.DestroyChildren();
            CategoriesPanel.transform.DestroyChildren();
            LabelsPanel.transform.DestroyChildren();
            TypesPanel.transform.DestroyChildren();
            ValuesTextPanel.transform.DestroyChildren();
            ParamsTextPanel.transform.DestroyChildren();

            StartCoroutine(UpdateLists());
        }

        /// <summary>
        /// A coroutine that updates the GUI every .3 seconds with the UDP data being received according to what columns are selected
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateLists()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1 / 3f);

                if (IsHidden) continue;

                PopulateDevices();

                if (_lastDeviceSelected != null && UdpGenericTranslator.Devices.ContainsKey(_lastDeviceSelected) == false)
                {
                    _lastDeviceSelected = null;
                    _lastIdSelected = null;
                    _lastCategorySelected = null;
                    _lastLabelSelected = null;
                    _lastTypeSelected = null;
                }

                if (_lastDeviceSelected != null)
                    PopulateIds(_lastDeviceSelected);
                if (_lastIdSelected != null)
                    PopulateCategories(_lastIdSelected);
                if (_lastCategorySelected != null)
                    PopulateLabels(_lastCategorySelected);
                if (_lastLabelSelected != null)
                    PopulateType(_lastLabelSelected);
                if (_lastTypeSelected != null)
                    PopulateValues(_lastTypeSelected);
                //Debug.Log("teste");
            }
        }

        /// <summary>
        /// Gets all the devices being received by UDP from the <see cref="UdpGenericTranslator.GetAllDevices"/> and Updates the GUI accordingly using <see cref="UpdateGuiColumn"/> function
        /// </summary>
        private void PopulateDevices()
        {
            var deviceList = UdpGenericTranslator.GetAllDevices();

            UpdateGuiColumn(DevicesPanel, DeviceBtn, deviceList);
        }

        /// <summary>
        /// If the <see cref="_lastDeviceSelected"/> is not null and differente from the <see cref="device"/> received, Calls the <see cref="DeleteInCascade"/> function.
        /// <para><see cref="_lastDeviceSelected"/> is now the <see cref="device"/> received.</para>
        /// <para>Gets all the ids from UDP according to the <see cref="device"/> parameter using the <see cref="GenericDevice.GetDeviceIds"/> and Updates the GUI accordingly using <see cref="UpdateGuiColumn"/> function</para>
        /// </summary>
        /// <param name="device">The device selected</param>
        public void PopulateIds(string device)
        {
            if (_lastDeviceSelected != null && _lastDeviceSelected != device)
                DeleteInCascade(DevicesPanel);

            _lastDeviceSelected = device;

            var idList = UdpGenericTranslator.Devices[device].GetDeviceIds();

            UpdateGuiColumn(IdsPanel, IdBtn, idList);
        }

        /// <summary>
        /// If the <see cref="_lastIdSelected"/> is not null and differente from the <see cref="id"/> received, Calls the <see cref="DeleteInCascade"/> function.
        /// <para><see cref="_lastIdSelected"/> is now the <see cref="id"/> received.</para>
        /// <para>Gets all the categories from UDP according to the <see cref="id"/> parameter and the <see cref="_lastDeviceSelected"/> using the <see cref="GenericDevice.GetCategoryById"/> and Updates the GUI accordingly using <see cref="UpdateGuiColumn"/> function</para>
        /// </summary>
        /// <param name="id">The id selected</param>
        public void PopulateCategories(string id)
        {
            if (_lastIdSelected != null && _lastIdSelected != id)
                DeleteInCascade(IdsPanel);

            _lastIdSelected = id;

            var categoryList = UdpGenericTranslator.Devices[_lastDeviceSelected].GetCategoryById(id);

            UpdateGuiColumn(CategoriesPanel, CategoryBtn, categoryList);
        }


        /// <summary>
        /// If the <see cref="_lastCategorySelected"/> is not null and differente from the <see cref="category"/> received, Calls the <see cref="DeleteInCascade"/> function.
        /// <para><see cref="_lastCategorySelected"/> is now the <see cref="category"/> received.</para>
        /// <para>Gets all the labels from UDP according to the <see cref="category"/> parameter, the <see cref="_lastDeviceSelected"/> and the <see cref="_lastIdSelected"/> using the <see cref="GenericDevice.GetLabel"/> and Updates the GUI accordingly using <see cref="UpdateGuiColumn"/> function</para>
        /// </summary>
        /// <param name="category">The category selected</param>
        public void PopulateLabels(string category)
        {
            if (_lastCategorySelected != null && _lastCategorySelected != category)
                DeleteInCascade(CategoriesPanel);

            _lastCategorySelected = category;

            var labelList = UdpGenericTranslator.Devices[_lastDeviceSelected].GetLabel(_lastIdSelected, category);

            UpdateGuiColumn(LabelsPanel, LabelBtn, labelList);
        }

        /// <summary>
        /// If the <see cref="_lastLabelSelected"/> is not null and differente from the <see cref="label"/> received, Calls the <see cref="DeleteInCascade"/> function.
        /// <para><see cref="_lastLabelSelected"/> is now the <see cref="label"/> received.</para>
        /// <para>Gets all the types from UDP according to the <see cref="label"/> parameter, the <see cref="_lastDeviceSelected"/>, the <see cref="_lastIdSelected"/> and the <see cref="_lastCategorySelected"/> using the <see cref="GenericDevice.GetTypeList"/> and Updates the GUI accordingly using <see cref="UpdateGuiColumn"/> function</para>
        /// </summary>
        /// <param name="label">The label selected</param>
        public void PopulateType(string label)
        {
            if (_lastLabelSelected != null && _lastLabelSelected != label)
                DeleteInCascade(LabelsPanel);

            _lastLabelSelected = label;

            var typelist = UdpGenericTranslator.Devices[_lastDeviceSelected]
                .GetTypeList(_lastIdSelected, _lastCategorySelected, label);

            UpdateGuiColumn(TypesPanel, TypeBtn, typelist);
        }

        /// <summary>
        /// If the <see cref="_lastTypeSelected"/> is not null and differente from the <see cref="type"/> received, Calls the <see cref="DeleteInCascade"/> function.
        /// <para><see cref="_lastTypeSelected"/> is now the <see cref="type"/> received.</para>
        /// <para>Gets all the values from UDP according to the <see cref="type"/> parameter, the <see cref="_lastDeviceSelected"/>, the <see cref="_lastIdSelected"/>, the <see cref="_lastCategorySelected"/> and the <see cref="_lastLabelSelected"/> using the <see cref="GenericDevice.GetTypeList"/> and Updates the GUI accordingly using <see cref="UpdateGuiColumn"/> function</para>
        /// </summary>
        /// <param name="type">The type selected</param>
        public void PopulateValues(string type)
        {
            if (_lastTypeSelected != null && _lastTypeSelected != type)
                DeleteInCascade(TypesPanel);

            _lastTypeSelected = type;

            var valuesList = UdpGenericTranslator.Devices[_lastDeviceSelected]
                .GetValuesList(_lastIdSelected, _lastCategorySelected, _lastLabelSelected, type);

            UpdateGuiColumn(ValuesTextPanel, ValuesText, valuesList);

            PopulateParameters(type);
        }

        /// <summary>
        /// If the <see cref="_lastTypeSelected"/> is not null and differente from the <see cref="type"/> received, Calls the <see cref="DeleteInCascade"/> function.
        /// <para><see cref="_lastTypeSelected"/> is now the <see cref="type"/> received.</para>
        /// <para>Gets all the parameters from UDP according to the <see cref="type"/> parameter, the <see cref="_lastDeviceSelected"/>, the <see cref="_lastIdSelected"/>, the <see cref="_lastCategorySelected"/> and the <see cref="_lastLabelSelected"/> using the <see cref="GenericDevice.GetTypeList"/> and Updates the GUI accordingly using <see cref="UpdateGuiColumn"/> function</para>
        /// </summary>
        /// <param name="type">The type selected</param>
        private void PopulateParameters(string type)
        {
            if (_lastTypeSelected != null && _lastTypeSelected != type)
                DeleteInCascade(TypesPanel);

            _lastTypeSelected = type;

            var valuesList = UdpGenericTranslator.Devices[_lastDeviceSelected]
                .GetParameterList(_lastIdSelected, _lastCategorySelected, _lastLabelSelected, type);

            UpdateParamColumn(ParamsTextPanel, ValuesText, valuesList);
        }

        /// <summary>
        /// Unselects the button clicked and calls the <see cref="DeleteInCascade"/> function in the end
        /// </summary>
        /// <param name="column">The <see cref="DebuggerColumns"/> that is being unselected in hte GUI (device, id, category, etc)</param>
        public void UnselectOptions(DebuggerColumns column)
        {
            GameObject parentPanel;

            switch (column)
            {
                case DebuggerColumns.device:
                    _lastDeviceSelected = null;
                    parentPanel = DevicesPanel;
                    break;
                case DebuggerColumns.id:
                    _lastIdSelected = null;
                    parentPanel = IdsPanel;
                    break;
                case DebuggerColumns.category:
                    _lastCategorySelected = null;
                    parentPanel = CategoriesPanel;
                    break;
                case DebuggerColumns.label:
                    _lastLabelSelected = null;
                    parentPanel = LabelsPanel;
                    break;
                case DebuggerColumns.type:
                    _lastTypeSelected = null;
                    parentPanel = TypesPanel;
                    break;
                case DebuggerColumns.value:
                    parentPanel = ValuesTextPanel;
                    break;
                case DebuggerColumns.parameter:
                    parentPanel = ParamsTextPanel;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("column", column, null);
            }

            DeleteInCascade(parentPanel);
        }

        /// <summary>
        /// Updates the GUI column according to the information received
        /// </summary>
        /// <param name="parentPanel">The Columns Panel to be updated</param>
        /// <param name="prefab">The prefab to instatiate and put as a child of the <see cref="parentPanel"/></param>
        /// <param name="list">The list of strings to be used to populate each <see cref="prefab"/> text value</param>
        private void UpdateGuiColumn(GameObject parentPanel, GameObject prefab, List<string> list)
        {
            var childToKill = new List<GameObject>();

            if (parentPanel == ValuesTextPanel || parentPanel == ParamsTextPanel)
            {
                foreach (Transform child in parentPanel.transform)
                {
                    childToKill.Add(child.gameObject);
                   // DeleteInCascade(parentPanel);
                }
            }
            else
            {
                foreach (Transform child in parentPanel.transform)
                {
                    if (list.Contains(child.name) == false)
                    {
                        childToKill.Add(child.gameObject);
                        DeleteInCascade(parentPanel);
                    }
                }
            }
            foreach (var c in childToKill)
                DestroyImmediate(c);


            foreach (var device in list)
            {
                if (parentPanel.transform.Find(device) != null) continue;
                var deviceBtn = Instantiate(prefab, parentPanel.transform, false);
                deviceBtn.name = device;


                if (parentPanel == ValuesTextPanel || parentPanel == ParamsTextPanel)
                {
                    deviceBtn.GetComponent<Text>().text = device;
                }
                else
                {
                    deviceBtn.transform.GetComponentInChildren<Text>().text = device;
                    deviceBtn.GetComponent<Toggle>().group = parentPanel.GetComponent<ToggleGroup>();
                }
            }
        }

        /// <summary>
        /// Updates the GUI column according to the information received
        /// </summary>
        /// <param name="parentPanel">The Columns Panel to be updated</param>
        /// <param name="prefab">The prefab to instatiate and put as a child of the <see cref="parentPanel"/></param>
        /// <param name="list">The list of strings to be used to populate each <see cref="prefab"/> text value</param>
        private void UpdateParamColumn(GameObject parentPanel, GameObject prefab, Dictionary<string, string> list)
        {
            var childToKill = new List<GameObject>();

            
            foreach (Transform child in parentPanel.transform)
            {
                childToKill.Add(child.gameObject);
                // DeleteInCascade(parentPanel);
            }
            
            foreach (var c in childToKill)
                DestroyImmediate(c);


            foreach (var data in list)
            {
                if (parentPanel.transform.Find(data.Key) != null) continue;
                var deviceBtn = Instantiate(prefab, parentPanel.transform, false);
                deviceBtn.name = data.Key;

                
                    deviceBtn.GetComponent<Text>().text = data.Key + " - " + data.Value;
            }
        }


        /// <summary>
        /// Clears all the columns to the right of the <see cref="parentPanel"/> in the GUI
        /// </summary>
        /// <param name="parentPanel">Gui column used to see which columns are on the left.</param>
        private void DeleteInCascade(GameObject parentPanel)
        {
            if (parentPanel == DevicesPanel)
            {
                _lastIdSelected = null;
                _lastCategorySelected = null;
                _lastLabelSelected = null;
                _lastTypeSelected = null;
                IdsPanel.transform.DestroyChildren();
                CategoriesPanel.transform.DestroyChildren();
                LabelsPanel.transform.DestroyChildren();
                TypesPanel.transform.DestroyChildren();
            }
            else if (parentPanel == IdsPanel)
            {
                _lastCategorySelected = null;
                _lastLabelSelected = null;
                _lastTypeSelected = null;
                CategoriesPanel.transform.DestroyChildren();
                LabelsPanel.transform.DestroyChildren();
                TypesPanel.transform.DestroyChildren();
            }
            else if (parentPanel == CategoriesPanel)
            {
                _lastLabelSelected = null;
                _lastTypeSelected = null;
                LabelsPanel.transform.DestroyChildren();
                TypesPanel.transform.DestroyChildren();
            }
            else if (parentPanel == LabelsPanel)
            {
                _lastTypeSelected = null;
                TypesPanel.transform.DestroyChildren();
            }
            ValuesTextPanel.transform.DestroyChildren();
            ParamsTextPanel.transform.DestroyChildren();
        }
    }
}
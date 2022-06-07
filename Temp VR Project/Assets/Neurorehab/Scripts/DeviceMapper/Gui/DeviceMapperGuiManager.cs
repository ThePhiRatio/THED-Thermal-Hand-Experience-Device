using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Converters;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for showing and populating the gui. Lists all available objects that can be mapped, 
    /// </summary>
    public class DeviceMapperGuiManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject SingleInputGameobjectsPanel;
        public GameObject AvailableGameobjectsPanel;
        public GameObject SelectedGameobjectPropertiesPanel;
        public GameObject SelectedGameobjectWindow;
        public GameObject SelectedGameobjectWindowTitle;
        //public GameObject SingleInputInformationTypesPanel;
        //public GameObject SingleInputInformationTypesTitle;
        public GameObject SelectedGameobjectPropertiesTogglerWindow;
        public GameObject SelectedGameobjectPropertiesTogglerPanel;

        [Header("Other")]
        public Button AddPropBtn;

        public Toggle PauseBtn;

        [Header("Prefabs")]
        public GameObject SingleInputConnectionPrefab;
        public GameObject AvailableObjectPrefab;
        public GameObject SelectedGameobjectPropertyPrefab;
        //public GameObject SingleInputInformationTypesPrefab;
        public GameObject CameraWindowPrefab;
        public GameObject SelectedGameobjectPropertyTogglePrefab;


        public static DeviceMapperGuiManager Instance
        {
            get { return _instance; }
            set
            {
                if (_instance != null)
                {
                    Destroy(value);
                    return;
                }
                _instance = value;
            }
        }

        /// <summary>
        /// Indicates if the DeviceMapper is visible or not
        /// </summary>
        public bool IsShowing;

        private List<int> _lastLayers;

        /// <summary>
        /// The Current Selected Gameobject showing in the Preview Window
        /// </summary>
        public GameObjectInformation CurrentSelectedObject
        {
            get { return _currentSelectedObject; }
            set
            {
                _currentSelectedObject = value;
                AddPropBtn.interactable = _currentSelectedObject != null;
            }
        }

        /// <summary>
        /// A list of all the Available Gameobjects that can be mapped. It is filled with all the gameobjects in the scene that have the <see cref="Tags.AvailableToMap"/>
        /// </summary>
        internal List<GameObject> AvailableGameobjects { get; set; }

        /// <summary>
        /// Saves the previously layer of the <see cref="CurrentSelectedObject"/>. 
        /// </summary>
        private int _selectedObjectLastLayer;

        /// <summary>
        /// The Camera used to show the tagged gameobject in the preview window
        /// </summary>
        private GameObject _previewCam;

        /// <summary>
        /// Backing field of the <see cref="CurrentSelectedObject"/>
        /// </summary>
        private GameObjectInformation _currentSelectedObject;

        private static DeviceMapperGuiManager _instance;

        private void Awake()
        {
            Instance = this;
            _lastLayers = new List<int>();
        }
        
        private GameObject PreviewCamera
        {
            get
            {
                if (_previewCam == null)
                {
                    _previewCam = InstantiatePreviewCamera();
                }

                return _previewCam;
            }
        }


        private void Start()
        {
            SelectedGameobjectPropertiesPanel.transform.DestroyChildren();
            SingleInputGameobjectsPanel.transform.DestroyChildren();
            SelectedGameobjectPropertiesTogglerPanel.transform.DestroyChildren();

            SelectedGameobjectPropertiesTogglerWindow.SetActive(false);
            AddPropBtn.interactable = false;

            AvailableGameobjectsPanel.transform.DestroyChildren();
            //get all objects with tag availableToMap
            StartCoroutine(ListAllAvailabelGameobjects());
        }

        public void Update()
        {
            PauseBtn.isOn = Time.timeScale == 0;
        }

        /// <summary>
        /// Populates the GUI with all the <see cref="GameObjectProperty"/> belonging to the CurrentSelectedSingleInput
        /// </summary>
        public void ListAllSingleInputConections()
        {
            var singleInputGameobjects = MapperManager.GetCurrentSingleInputConections();

            SingleInputGameobjectsPanel.transform.DestroyChildren();

            if (singleInputGameobjects == null) return; //id there are no connections, returns.

            foreach (var objectProp in singleInputGameobjects.ObjectProperties)
            {
                //create prefab and add to SingleInputGameobjectsPanel
                var prefab = Instantiate(SingleInputConnectionPrefab);
                var prefabPropGui = prefab.GetComponent<GameObjectPropertyGui>();

                prefabPropGui.GameObjectProperty = objectProp;

                prefab.transform.GetChild(0).gameObject.GetComponent<Text>().text = objectProp.Target.name + " - " + objectProp.InfoType;

                prefab.transform.SetParent(SingleInputGameobjectsPanel.transform, false);
            }
        }

        /// <summary>
        /// Instantiates and returns the preview camera
        /// </summary>
        /// <returns></returns>
        private GameObject InstantiatePreviewCamera()
        {
            var preview = Instantiate(CameraWindowPrefab, DeviceMapper.Instance.transform, false);
            var layerNumber = (int)Mathf.Log(DeviceMapper.Instance.CameraLayer.value, 2);
            preview.layer = layerNumber;

            //add the layer to the camera's culling mask
            preview.GetComponentInChildren<Camera>().cullingMask |= 1 << layerNumber;

            foreach (Transform child in preview.transform)
            {
                child.gameObject.layer = layerNumber;
            }

            return preview;
        }

        /// <summary>
        /// Populates the right side bar list with all the gameobjects that have the specified Tag from <see cref="DeviceMapper"/>
        /// </summary>
        public IEnumerator ListAllAvailabelGameobjects()
        {
            while (true)
            {
                UpdateAvailableGameobjectsList();

                yield return new WaitForSecondsRealtime(1f);
            }
        }

        public void UpdateAvailableGameobjectsList(bool redoAll = false)
        {
            //get all objects with tag availableToMap
            AvailableGameobjects = GameObject.FindGameObjectsWithTag(Tags.AvailableToMap.ToString()).ToList();

            if (AvailableGameobjects == null)
            {
                AvailableGameobjectsPanel.transform.DestroyChildren();
                return;
            }

            var goToDestroy = new List<GameObject>();

            //get all gameobjects that are not availableToMap anymore
            foreach (Transform childGo in AvailableGameobjectsPanel.transform)
            {
                if (AvailableGameobjects.Find(c => c.name + "GOI" == childGo.name) != null && redoAll == false)
                {
                    continue;
                }
                goToDestroy.Add(childGo.gameObject);
            }

            //destroy them
            for (var i = 0; i < goToDestroy.Count; i++)
            {
                DestroyImmediate(goToDestroy[i]);
            }

            //add the new objects
            foreach (var go in AvailableGameobjects)
            {
                if (go == false || go == null) continue;

                if (AvailableGameobjectsPanel.transform.Find(go.name + "GOI") != null && redoAll == false)
                {
                    return;
                }

                //creates prefab and adds to AvailableGameobjectsPanel parent
                var availableGoPrefbab = Instantiate(AvailableObjectPrefab);
                var info = availableGoPrefbab.GetComponent<GameObjectInformation>();
                info.Target = go;
                info.InitProperties();
                availableGoPrefbab.GetComponentInChildren<Text>().text = go.name;
                availableGoPrefbab.name = go.name + "GOI";
                availableGoPrefbab.GetComponent<Toggle>().group = AvailableGameobjectsPanel.GetComponent<ToggleGroup>();
                availableGoPrefbab.transform.SetParent(AvailableGameobjectsPanel.transform, false);
            }
        }

        /// <summary>
        /// Lists all the properties of the selected Gameobject. (rotation, position, value, etc)
        /// </summary>
        public void ListAllSelectedGameobjectProperties()
        {
            SelectedGameobjectPropertiesPanel.transform.DestroyChildren();
            if (CurrentSelectedObject == null) return;
            //todo nao pode fechar os axis que tao abertos

            foreach (var objectProp in CurrentSelectedObject.ObjectProperties)
            {
                //if it is not active, continue
                if(objectProp.Active == false) continue; 

                //creates prefab and adds to selectedGameobjectpropertiesPanel parent
                var prefab = Instantiate(SelectedGameobjectPropertyPrefab);
                var prefabGoPropertyComponent = prefab.GetComponentInChildren<GameObjectPropertyGui>();

                prefabGoPropertyComponent.Init(objectProp);

                prefab.transform.SetParent(SelectedGameobjectPropertiesPanel.transform, false);
            }
        }

        /// <summary>
        /// Populates in the GUI the current selected gameobject in the Gameobject Window
        /// </summary>
        /// <param name="go"></param>
        public void PopulateSelectedGameobjectWindow(GameObject go)
        {
            ResetGameobjectLayer();
            
            //get current layer
            _selectedObjectLastLayer = go.layer;
            CurrentSelectedObject = go.GetComponent<GameObjectInformation>();

            for (var i = 0; i < CurrentSelectedObject.Target.transform.childCount; i++)
            {
                _lastLayers.Add(CurrentSelectedObject.Target.transform.GetChild(i).gameObject.layer);
            }

            //change to camera layer
            CurrentSelectedObject.Target.layer = (int)Mathf.Log(DeviceMapper.Instance.CameraLayer.value, 2);
            
            foreach (Transform child in CurrentSelectedObject.Target.transform)
            {
                child.gameObject.layer = (int)Mathf.Log(DeviceMapper.Instance.CameraLayer.value, 2);
            }

            //populate window with object - add prefab camera to object parent
            PreviewCamera.SetActive(true);
            PreviewCamera.transform.SetParent(CurrentSelectedObject.Target.transform.parent, false);
            PreviewCamera.transform.position = CurrentSelectedObject.Target.transform.position;
            PreviewCamera.transform.Find("Camera").LookAt(CurrentSelectedObject.Target.transform);
            
            //populates properties 
            ListAllSelectedGameobjectProperties();

            //change window title color
            SelectedGameobjectWindowTitle.GetComponent<Image>().color = new Color32(255, 136, 0, 255);
        }

        public void RemoveSelectedObject()
        {
            ResetGameobjectLayer();
            CurrentSelectedObject = null;
            //populates properties 
            SelectedGameobjectPropertiesPanel.transform.DestroyChildren();

            //change window title color
            SelectedGameobjectWindowTitle.GetComponent<Image>().color = new Color32(87, 140, 167, 255);

        }

        /// <summary>
        /// Resets the <see cref="CurrentSelectedObject"/> layer
        /// </summary>
        public void ResetGameobjectLayer()
        {
            //change currentobjectselected to its previous layer
            if (CurrentSelectedObject != null)
            {
                CurrentSelectedObject.Target.layer = _selectedObjectLastLayer;

                for (int i = 0; i < CurrentSelectedObject.Target.transform.childCount; i++)
                {
                    CurrentSelectedObject.Target.transform.GetChild(i).gameObject.layer = _lastLayers[i];
                }
            }
        }

        
        /// <summary>
        /// Clears the SingleInputConnections Panel in the GUI
        /// </summary>
        public void ClearSingleInputConnectionsList()
        {
            SingleInputGameobjectsPanel.transform.DestroyChildren();
        }

        /// <summary>
        /// Shows the Edit/Add Property Window in the GUI. If the property is active  then the toggle will be on
        /// </summary>
        public void ShowAddPropertyWindow()
        {
            SelectedGameobjectPropertiesTogglerWindow.SetActive(true);
            SelectedGameobjectPropertiesTogglerPanel.transform.DestroyChildren();

            //instantiate properties
            foreach (var gameObjectProperty in CurrentSelectedObject.ObjectProperties)
            {
                //if (gameObjectProperty.InfoType == InformationType.sample) continue;

                var propToggle = Instantiate(SelectedGameobjectPropertyTogglePrefab,
                    SelectedGameobjectPropertiesTogglerPanel.transform, false);
                propToggle.GetComponentInChildren<Text>().text = gameObjectProperty.InfoType.ToString().ToUpper();

                propToggle.GetComponent<Toggle>().isOn = gameObjectProperty.Active;
            }
        }

        /// <summary>
        /// Adds the interpreter to the <see cref="CurrentSelectedObject"/> according to the <see cref="InformationType"/> receive
        /// </summary>
        /// <param name="infoType">Which <see cref="InformationType"/> has the <see cref="GameObjectProperty"/></param>
        public void AddGameobjectProperty(GameObjectInformation goi, InformationType infoType)
        {
            var gop = goi.ObjectProperties.First(goprop => goprop.InfoType == infoType);

            if (goi.Target == false || goi.Target == null) return;

            gop.Active = true;

            switch (infoType)
            {
                case InformationType.rotation:
                    if (goi.Target.GetComponent<RotationInterpreter>() == null)
                    {
                        var rotationInterpreter = goi.Target.AddComponent<RotationInterpreter>();
                        rotationInterpreter.GameObjectProperty = gop;
                        rotationInterpreter.CalibrationValues = goi.Target.AddComponent<CalibrationValues>();
                        rotationInterpreter.InitializeConverters(goi.Target.AddComponent<AbsoluteRotationConverter>(), goi.Target.AddComponent<AdditiveRotationConverter>());

                    }
                    break;
                case InformationType.position:
                    if (goi.Target.GetComponent<PositionInterpreter>() == null)
                    {
                        var posInterpreter = goi.Target.AddComponent<PositionInterpreter>();
                        posInterpreter.GameObjectProperty = gop;
                        posInterpreter.CalibrationValues = goi.Target.AddComponent<CalibrationValues>();
                        posInterpreter.InitializeConverters(goi.Target.AddComponent<AbsolutePositionConverter>(), goi.Target.AddComponent<AdditivePositionConverter>());
                    }
                    break;
                case InformationType.@bool:
                    if (goi.Target.GetComponent<BooleanInterpreter>() == null)
                    {
                        var boolInterpreter = goi.Target.AddComponent<BooleanInterpreter>();
                        boolInterpreter.GameObjectProperty = gop;
                        boolInterpreter.CalibrationValues = goi.Target.AddComponent<CalibrationValues>();
                    }
                    break;
                case InformationType.value:
                    if (goi.Target.GetComponent<ValueInterpreter>() == null)
                    {
                        var valueInterpreter = goi.Target.AddComponent<ValueInterpreter>();
                        valueInterpreter.GameObjectProperty = gop;
                        valueInterpreter.CalibrationValues = goi.Target.AddComponent<CalibrationValues>();
                        valueInterpreter.InitializeConverters(goi.Target.AddComponent<AbsoluteValueConverter>(), goi.Target.AddComponent<AdditiveValueConverter>());
                    }
                    break;
                case InformationType.sample:
                    if (goi.Target.GetComponent<SampleInterpreter>() == null)
                    {
                        var sampleInterpreter = goi.Target.AddComponent<SampleInterpreter>();
                        sampleInterpreter.GameObjectProperty = gop;
                        sampleInterpreter.CalibrationValues = goi.Target.AddComponent<CalibrationValues>();
                    }
                    break;
                default:
                    Debug.Log("Desconhecido: " + infoType);
                    break;
            }

            ListAllSelectedGameobjectProperties();
        }

        /// <summary>
        /// Removes the interpreter to the <see cref="CurrentSelectedObject"/> according to the <see cref="InformationType"/> receive
        /// </summary>
        /// <param name="infoType">Which <see cref="InformationType"/> has the <see cref="GameObjectProperty"/></param>
        public void RemoveGameobjectProperty(GameObjectInformation goi, InformationType infoType)
        {
            var gop = goi.ObjectProperties.First(goprop => goprop.InfoType == infoType);
            //if (CurrentSelectedObject.Target == null) return;

            gop.Active = false;

            MapperManager.RemoveAllGameobjectPropertyMappings(gop);

            switch (infoType)
            {
                case InformationType.rotation:
                    var rotIntepreter = goi.Target.GetComponent<RotationInterpreter>();
                    DestroyImmediate(rotIntepreter.CalibrationValues);
                    DestroyImmediate(rotIntepreter);
                    DestroyImmediate(goi.Target.GetComponent<AdditiveRotationConverter>());
                    DestroyImmediate(goi.Target.GetComponent<AbsoluteRotationConverter>());
                    break;
                case InformationType.position:
                    var posInterpreter = goi.Target.GetComponent<PositionInterpreter>();
                    DestroyImmediate(posInterpreter.CalibrationValues);
                    DestroyImmediate(posInterpreter);
                    DestroyImmediate(goi.Target.GetComponent<AdditivePositionConverter>());
                    DestroyImmediate(goi.Target.GetComponent<AbsolutePositionConverter>());
                    break;
                case InformationType.@bool:
                    var boolInterpreter = goi.Target.GetComponent<BooleanInterpreter>();
                    DestroyImmediate(boolInterpreter.CalibrationValues);
                    DestroyImmediate(boolInterpreter);
                    
                    break;
                case InformationType.value:
                    var valueInterpreter = goi.Target.GetComponent<ValueInterpreter>();
                    DestroyImmediate(valueInterpreter.CalibrationValues);
                    DestroyImmediate(valueInterpreter);
                    DestroyImmediate(goi.Target.GetComponent<AdditiveValueConverter>());
                    DestroyImmediate(goi.Target.GetComponent<AbsoluteValueConverter>());
                    break;
                case InformationType.sample:
                    DestroyImmediate(goi.Target.GetComponent<SampleInterpreter>());
                    break;
                default:
                    Debug.Log("Desconhecido: " + infoType);
                    break;
            }

            ListAllSelectedGameobjectProperties();
            ListAllSingleInputConections();
        }
    }
}

using System;
using System.Collections.Generic;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for managing the Property Editor Window in the GUI
    /// </summary>
    public class PropertyEditorGuiManager : MonoBehaviour
    {
        #region unityReferences

        [Header("Editor Window")]
        public GameObject EditorWindow;
        [Header("Position And Rotation Editor Window")]
        [Header(" ---  Panels")]
        public GameObject PositionOrRotationEditorWindow;
        public Text PositionOrRotationEditorTitle;
        public Toggle UseConverter;
        public Toggle UseonThisObject;
        [Header(" --- Gameobjects GameWorldX")]
        public Dropdown MapEnumX;
        public Button CalibrateBtnX;
        public Toggle InvertBoolX;
        public Toggle AdditiveModeX;
        public InputField IncrementX;
        public InputField ToleranceX;
        public Text ReadingFromX;
        public ValueReadingBar ReadingBarX;
        [Header(" --- Gameobjects GameWorldY")]
        public Dropdown MapEnumY;
        public Button CalibrateBtnY;
        public Toggle InvertBoolY;
        public Toggle AdditiveModeY;
        public InputField IncrementY;
        public InputField ToleranceY;
        public Text ReadingFromY;
        public ValueReadingBar ReadingBarY;
        [Header(" --- Gameobjects GameWorldZ")]
        public Dropdown MapEnumZ;
        public Button CalibrateBtnZ;
        public Toggle InvertBoolZ;
        public Toggle AdditiveModeZ;
        public InputField IncrementZ;
        public InputField ToleranceZ;
        public Text ReadingFromZ;
        public ValueReadingBar ReadingBarZ;

        [Header("Single Editor Window")]
        [Header(" ---  Panels")]
        public GameObject SingleEditorWindow;
        public Text SingleEditorTitle;
        [Header(" --- Gameobjects")]
        public Toggle SingleEditorAdditiveMode;
        public Text WorldAxisName;
        public Dropdown SingleEditorDropdownEnum;
        public Button SingleEditorCalibrateBtn;
        public Toggle SingleEditorInvertBool;
        public InputField SingleEditorIncrement;
        public InputField SingleEditorTolerance;
        public Text SingleReadingFrom;
        public ValueReadingBar SingleEditorReadingBarValue;

        [Header("Boolean Editor Window")]
        [Header(" ---  Panels")]
        public GameObject BooleanEditorWindow;
        [Header(" --- Gameobjects")]
        public Dropdown BooleanDropdownEnum;
        public Toggle BooleanInvertBool;
        public Toggle BooleanUseConverter;
        public Button BooleanCalibrateBtn;
        public Text BooleanReadingFrom;
        public BooleanReadingBar BooleanBar;


        /// ////////////////////////////////////////////////////
        [Header("Value Editor Window")]
        [Header(" ---  Panels")]
        public GameObject ValueEditorWindow;
        public Text ValueEditorTitle;
        public Toggle ValueEditorUseConverterToggle;
        public Toggle ValueEditorUseonThisObject;
        [Header(" --- Gameobjects")]
        public Dropdown ValueMapEnum;
        public Button ValueCalibrateBtn;
        public Toggle ValueInvertBool;
        public Toggle ValueAdditiveMode;
        public InputField ValueIncrement;
        public InputField ValueTolerance;
        public Text ValueReadingFrom;
        public ValueReadingBar ValueReadingBar;

        #endregion unityReferences
        /// <summary>
        /// Last shown property editor window
        /// </summary>
        private GameObject _lastShownWindow;

        /// <summary>
        /// Singleton of the <see cref="PropertyEditorGuiManager"/>
        /// </summary>
        public static PropertyEditorGuiManager Instance { get; set; }

        /// <summary>
        /// The current selected <see cref="SelectedGameObjectPropertyGui"/> 
        /// </summary>
        public GameObjectPropertyGui SelectedGameObjectPropertyGui { get; set; }

        /// <summary>   
        /// The current selected axis that is being calibrated (if it is a position or a rotation)
        /// </summary>
        public AxisLabels SelectedLabelToCalibrate { get; set; }

        void Awake()
        {
            Instance = this;
            EditorWindow.SetActive(false);
            PositionOrRotationEditorWindow.SetActive(false);
            SingleEditorWindow.SetActive(false);
            BooleanEditorWindow.SetActive(false);
            ValueEditorWindow.SetActive(false);

            MapEnumX.name = "MapEnumX";
            MapEnumY.name = "MapEnumY";
            MapEnumZ.name = "MapEnumZ";

            AdditiveModeX.name = "JoystickX"; 
            AdditiveModeY.name = "JoystickY"; 
            AdditiveModeZ.name = "JoystickZ"; 
        }

        /// <summary>
        /// Hides the <see cref="_lastShownWindow"/> and shows the <see cref="windowToShow"/>.
        /// <para> <see cref="_lastShownWindow"/> is now the <see cref="windowToShow"/> </para>
        /// </summary>
        /// <param name="windowToShow">the window to show</param>
        private void ShowEditorWindow(GameObject windowToShow)
        {
            if (_lastShownWindow != null)
                _lastShownWindow.SetActive(false);

            _lastShownWindow = windowToShow;
            windowToShow.SetActive(true);
        }

        /// <summary>
        /// Hides the <see cref="_lastShownWindow"/>
        /// </summary>
        public void HidePropertyEditorWindow()
        {
            EditorWindow.SetActive(false);
            _lastShownWindow.SetActive(false);
        }

        /// <summary>
        /// Shows in the GUI the Editor Widnow for the specified <see cref="GameObjectProperty"/> 
        /// </summary>
        /// <param name="gameObjectPropertyGui">Used to get the <see cref="InformationType"/></param>
        public void ShowPropertyEditorWindow(GameObjectPropertyGui gameObjectPropertyGui)
        {
            SelectedGameObjectPropertyGui = gameObjectPropertyGui;

            EditorWindow.SetActive(true);

            switch (gameObjectPropertyGui.GameObjectProperty.InfoType)
            {
                case InformationType.rotation:
                    ShowPositionOrRotationEditWindow();
                    break;
                case InformationType.position:
                    ShowPositionOrRotationEditWindow();
                    break;
                case InformationType.@bool:
                    ShowBooleanEditWindow();
                    break;
                case InformationType.value:
                    ShowValueEditWindow();
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("infoType", gameObjectPropertyGui.GameObjectProperty.InfoType,
                        null);
            }
        }

        /// <summary>
        /// Shows in the GUI the Value Editor Window and updates all its settings according to the interpreter
        /// </summary>
        private void ShowValueEditWindow()
        {
            SelectedLabelToCalibrate = AxisLabels.Value;

            ValueEditorUseConverterToggle.isOn = GetCalibrationMode() != CalibrationMode.Direct;
            ValueEditorUseonThisObject.isOn = GetUseOnThisObject();

            UpdateGuiAxisSettings(ValueMapEnum, 0, ValueCalibrateBtn, ValueInvertBool, ValueReadingFrom);
            UpdateGuiAxisBarSettings(ValueReadingBar);
            UpdateGuiJoystickModeSettings(ValueIncrement, ValueTolerance, ValueAdditiveMode);
            
            ShowEditorWindow(ValueEditorWindow);
        }

        /// <summary>
        /// Shows in the GUI the Single Editor Widnow for the specified <see cref="GameObjectProperty"/> <see cref="InformationType"/>
        /// </summary>
        /// <param name="gameObjectPropertyGui">Used to get the <see cref="InformationType"/></param>
        /// <param name="axisIndexClicked">Axis clicked</param>
        public void ShowSinglePropertyEditorWindow(GameObjectPropertyGui gameObjectPropertyGui, int axisIndexClicked)
        {
            SelectedGameObjectPropertyGui = gameObjectPropertyGui;

            SelectedLabelToCalibrate = (AxisLabels) axisIndexClicked + 2;
            EditorWindow.SetActive(true);

            ShowSinglePositionOrRotationEditWindow();
        }
        
        /// <summary>
        /// Shows in the GUI the BooleanEditor Widnow for the specified <see cref="GameObjectProperty"/> 
        /// </summary>
        private void ShowBooleanEditWindow()
        {
            SelectedLabelToCalibrate = AxisLabels.Bool;

            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;

            var boolInterpreter = curGop.Target.GetComponent<BooleanInterpreter>();
            var calibValues = boolInterpreter.CalibrationValues;

            BooleanInvertBool.isOn = curGop.GetBoolInterpreter().CalibrationValues.GetInvertLogic(AxisLabels.Bool);

            BooleanBar.InitBar(curGop, calibValues.GetMinReadValue(SelectedLabelToCalibrate),
                calibValues.GetMaxReadValue(SelectedLabelToCalibrate),
                boolInterpreter.CalibrationValues.InputData[AxisLabels.Bool].Center, boolInterpreter.CalibrationValues.GetInvertLogic(AxisLabels.Bool));

            UpdateGuiAxisSettings(BooleanDropdownEnum, 0, BooleanCalibrateBtn, BooleanInvertBool, BooleanReadingFrom);

            var sim = SelectedGameObjectPropertyGui.GameObjectProperty.SingleInputMappings[0];
            BooleanBar.GetComponent<CanvasGroup>().interactable =
                sim.InputMappingLabels != SingleInputMappingLabels.Bool;

            BooleanUseConverter.isOn = GetCalibrationMode() != CalibrationMode.Direct;
            
            ShowEditorWindow(BooleanEditorWindow);
        }

        /// <summary>
        /// Shows in the GUI the Single Value Editor Widnow for the specified <see cref="GameObjectProperty"/> according to the axis received
        /// </summary>
        private void ShowSinglePositionOrRotationEditWindow()
        {
            #if UNITY_EDITOR
            Debug.Log("world axis " + SelectedLabelToCalibrate);
            #endif

            SingleEditorTitle.text =
                string.Format("{0} editor", SelectedGameObjectPropertyGui.GameObjectProperty.InfoType).ToUpper();

            WorldAxisName.text =
                string.Format("Input - Game world {0}", SelectedLabelToCalibrate).ToUpper();

            UpdateGuiAxisSettings(SingleEditorDropdownEnum, 0, SingleEditorCalibrateBtn, SingleEditorInvertBool, SingleReadingFrom);
            UpdateGuiAxisBarSettings(SingleEditorReadingBarValue);
            UpdateGuiJoystickModeSettings(SingleEditorIncrement, SingleEditorTolerance, SingleEditorAdditiveMode);
            
            ShowEditorWindow(SingleEditorWindow);
        }

        /// <summary>
        /// Shows in the GUI and Populates the <see cref="PositionOrRotationEditorWindow"/> data according to the <see cref="SelectedGameObjectPropertyGui"/>
        /// </summary>
        private void ShowPositionOrRotationEditWindow()
        {
            PositionOrRotationEditorTitle.text =
                string.Format("{0} editor", SelectedGameObjectPropertyGui.GameObjectProperty.InfoType).ToUpper();

            UseConverter.isOn = GetCalibrationMode() != CalibrationMode.Direct ;
            UseonThisObject.isOn = GetUseOnThisObject();

            //x
            SelectedLabelToCalibrate = AxisLabels.X;
            UpdateGuiAxisSettings(MapEnumX, 0, CalibrateBtnX, InvertBoolX, ReadingFromX);
            UpdateGuiAxisBarSettings(ReadingBarX);
            UpdateGuiJoystickModeSettings(IncrementX, ToleranceX, AdditiveModeX);

            //y
            SelectedLabelToCalibrate = AxisLabels.Y;
            UpdateGuiAxisSettings(MapEnumY, 1, CalibrateBtnY, InvertBoolY, ReadingFromY);
            UpdateGuiAxisBarSettings(ReadingBarY);
            UpdateGuiJoystickModeSettings(IncrementY, ToleranceY, AdditiveModeY);

            //z
            SelectedLabelToCalibrate = AxisLabels.Z;
            UpdateGuiAxisSettings(MapEnumZ, 2, CalibrateBtnZ, InvertBoolZ, ReadingFromZ);
            UpdateGuiAxisBarSettings(ReadingBarZ);
            UpdateGuiJoystickModeSettings(IncrementZ, ToleranceZ, AdditiveModeZ);

            #region RAINBOW

            // You were never here! - UNCOMMENT TO SEE MAGIC!

            //var rainbow1 = new Color32(148, 0, 211, 255); // code wasn't rainbow enough
            //var rainbow2 = new Color32(75, 0, 130, 255); // code wasn't rainbow enough
            //var rainbow3 = new Color32(0, 0, 255, 255); // code wasn't rainbow enough
            //var rainbow4 = new Color32(0, 255, 0, 255); // code wasn't rainbow enough
            //var rainbow5 = new Color32(255, 255, 0, 255); // code wasn't rainbow enough
            //var rainbow6 = new Color32(255, 127, 0, 255); // code wasn't rainbow enough
            //var rainbow7 = new Color32(255, 0, 0, 255); // code wasn't rainbow enough
                                                        /*
                                                                             ___________________________
                                                                      _,----'  _______________________  `----._
                                                                   ,-'  __,---'  ___________________  `---.__  `-.
                                                                ,-'  ,-'  __,---'  _______________  `---.__  `-.  `-.
                                                              ,'  ,-'  ,-'  __,---'                `---.__  `-.  `-.  `.
                                                             /  ,'  ,-'  ,-'                               `-.  `-.  `.  \
                                                            / ,'  ,' ,--'                                     `--. `.  `. \
                                                           | /  ,' ,'                                             `. `.  \ |
                                                          ,--. ,--.                                                  _______
                                                         ( `  "   ')                                                (_______)
                                                          >-  .  -<                                                 /       \
                                                         ( ,      .)                                               ( G O L D )
                                                          `--'^`--'           SomeWhere Over The Rainbow            \_______/
                                                             /_\                    There Is Hope

                                                         * */

            #endregion

            ShowEditorWindow(PositionOrRotationEditorWindow);
        }

        /// <summary>
        /// Returns the <see cref="Interpreter.UseOnThisObject"/> according to the correct interperter type
        /// </summary>
        /// <returns></returns>
        private bool GetUseOnThisObject()
        {
            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;

            switch (curGop.InfoType)
            {
                case InformationType.rotation:
                    return curGop.GetRotInterpreter().UseOnThisObject;
                case InformationType.position:
                    return curGop.GetPosInterpreter().UseOnThisObject;
                case InformationType.@bool:
                    return curGop.GetBoolInterpreter().UseOnThisObject;
                case InformationType.value:
                    return curGop.GetValueInterpreter().UseOnThisObject;
                case InformationType.sample:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the <see cref="CalibrationMode"/> of the correct interpreter
        /// </summary>
        /// <returns></returns>
        private CalibrationMode GetCalibrationMode()
        {
            switch (SelectedGameObjectPropertyGui.GameObjectProperty.InfoType)
            {
                case InformationType.rotation:
                    return SelectedGameObjectPropertyGui.GameObjectProperty.GetRotInterpreter().Mode;
                case InformationType.position:
                    return SelectedGameObjectPropertyGui.GameObjectProperty.GetPosInterpreter().Mode;
                case InformationType.@bool:
                    return SelectedGameObjectPropertyGui.GameObjectProperty.GetBoolInterpreter().Mode;
                case InformationType.value:
                    return SelectedGameObjectPropertyGui.GameObjectProperty.GetValueInterpreter().Mode;
                case InformationType.sample:
                    return CalibrationMode.Direct;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the joystick mode section in the gui according tp the respective interpreter
        /// </summary>
        /// <param name="speedInput">The Inputfield that holds the speed</param>
        /// <param name="centerRegInput">The Inputfield that holds the center region percentage</param>
        /// <param name="additiveModeToggle">The toggle that holds the joystick mode</param>
        private void UpdateGuiJoystickModeSettings(InputField speedInput, InputField centerRegInput, Toggle additiveModeToggle)
        {
            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;
            float speed = 0f;
            float centerReg = 0f;
            var enable = false;
            switch (curGop.InfoType)
            {
                case InformationType.rotation:
                    var rotInterpreter = curGop.Target.GetComponent<RotationInterpreter>();
                    speed = rotInterpreter.GetSpeed(SelectedLabelToCalibrate);
                    centerReg = rotInterpreter.GetTolerance(SelectedLabelToCalibrate);
                    enable = CheckJoystickMode(rotInterpreter.CalibrationValues);
                    break;
                case InformationType.position:
                    var posInterpreter = curGop.Target.GetComponent<PositionInterpreter>();
                    speed = posInterpreter.GetSpeed(SelectedLabelToCalibrate);
                    centerReg = posInterpreter.GetTolerance(SelectedLabelToCalibrate);
                    enable = CheckJoystickMode(posInterpreter.CalibrationValues);
                    break;
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    var valueInterpreter = curGop.Target.GetComponent<ValueInterpreter>();
                    speed = valueInterpreter.GetSpeed(SelectedLabelToCalibrate);
                    centerReg = valueInterpreter.GetTolerance(SelectedLabelToCalibrate);
                    enable = CheckJoystickMode(valueInterpreter.CalibrationValues);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            speedInput.text = speed.ToString();
            centerRegInput.text = centerReg.ToString();
            additiveModeToggle.isOn = enable;
        }

        /// <summary>
        /// Returns true if the <see cref="SelectedLabelToCalibrate"/> is on Joystick Mode.
        /// </summary>
        private bool CheckJoystickMode(CalibrationValues calibrationValues)
        {
            return calibrationValues.OutputData[SelectedLabelToCalibrate].Mode == CalibrationMode.Additive;
        }
        
        /// <summary>
        /// Updates in the GUI the received <see cref="ValueReadingBar"/> according to the <see cref="SelectedGameObjectPropertyGui"/> type
        /// </summary>
        /// <param name="readingBar">The bar to initialize</param>
        private void UpdateGuiAxisBarSettings(ValueReadingBar readingBar)
        {
            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;

            CalibrationValues calibValues;
            switch (curGop.InfoType)
            {
                case InformationType.rotation:
                    var rotInterpreter = curGop.Target.GetComponent<RotationInterpreter>();
                    calibValues = rotInterpreter.CalibrationValues;
                    readingBar.InitBar(curGop, 
                        calibValues.GetMinReadValue(SelectedLabelToCalibrate), 
                        calibValues.GetMaxReadValue(SelectedLabelToCalibrate),
                        calibValues.GetMinWorldValue(SelectedLabelToCalibrate),
                        calibValues.GetMaxWorldValue(SelectedLabelToCalibrate),
                        rotInterpreter.GetTolerance(SelectedLabelToCalibrate), 
                        rotInterpreter.GetClampValue(SelectedLabelToCalibrate),
                        SelectedLabelToCalibrate, calibValues.GetCenterReadValue(SelectedLabelToCalibrate));
                    break;
                case InformationType.position:
                    var posINterpreter = curGop.Target.GetComponent<PositionInterpreter>();
                    calibValues = posINterpreter.CalibrationValues;
                    readingBar.InitBar(curGop, calibValues.GetMinReadValue(SelectedLabelToCalibrate),
                        calibValues.GetMaxReadValue(SelectedLabelToCalibrate),
                        calibValues.GetMinWorldValue(SelectedLabelToCalibrate),
                        calibValues.GetMaxWorldValue(SelectedLabelToCalibrate),
                        posINterpreter.GetTolerance(SelectedLabelToCalibrate),
                        posINterpreter.GetClampValue(SelectedLabelToCalibrate),
                        SelectedLabelToCalibrate, calibValues.GetCenterReadValue(SelectedLabelToCalibrate));
                    break;
                //case InformationType.@bool:
                //    break;
                case InformationType.value:
                    var valueinterpreter = curGop.Target.GetComponent<ValueInterpreter>();
                    calibValues = valueinterpreter.CalibrationValues;
                    readingBar.InitBar(curGop, calibValues.GetMinReadValue(SelectedLabelToCalibrate),
                        calibValues.GetMaxReadValue(SelectedLabelToCalibrate),
                        calibValues.GetMinWorldValue(SelectedLabelToCalibrate),
                        calibValues.GetMaxWorldValue(SelectedLabelToCalibrate),
                        valueinterpreter.GetTolerance(SelectedLabelToCalibrate),
                        valueinterpreter.GetClampValue(SelectedLabelToCalibrate),
                        AxisLabels.Value, calibValues.GetCenterReadValue(SelectedLabelToCalibrate));
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates in the GUI all the axis settings, such as dropdown, calibration button, additive mode and invert boolean toggle according to the index of the <see cref="SingleInputMapping"/> 
        /// </summary>
        /// <param name="mapEnum">the dropdown that belongs to the respective axis</param>
        /// <param name="index">the index of the <see cref="SingleInputMapping"/> </param>
        /// <param name="calibrationBtn">the calibration button that belongs to the respective axis</param>
        /// <param name="invertLogic">the invert boolean toggle that belongs to the respective axis</param>
        /// <param name="readingFromText"></param>
        /// <param name="additiveMode">the additive mode toggle that belongs to the respective axis</param>
        private void UpdateGuiAxisSettings(Dropdown mapEnum, int index, Button calibrationBtn, Toggle invertLogic, Text readingFromText)
        {
            mapEnum.ClearOptions();

            var sim = SelectedGameObjectPropertyGui.GameObjectProperty.SingleInputMappings[index];
            
            var mappingOptionsLabels = GetListOfOptions(sim);
            mapEnum.AddOptions(mappingOptionsLabels);
            mapEnum.value = SelectPreviousDropdownLabel(mappingOptionsLabels, index);

            readingFromText.text = "Reading From - " + sim.SingleInput.Label + " :";

            invertLogic.isOn = GetInvertLogic();

            calibrationBtn.interactable = mapEnum.value != 0;

            calibrationBtn.gameObject.SetActive(sim.InputMappingLabels != SingleInputMappingLabels.Bool);
            
            //TODO TIRAR A FITACOLA!!
            UpdateAxisMappedCalibrationBtn(sim.InputMappingLabels.ToString(), calibrationBtn);
        }

        /// <summary>
        /// Returns the Invert Logic value of the Calibration Values
        /// </summary>
        /// <returns></returns>
        private bool GetInvertLogic()
        {
            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;
            return curGop.GetInterpreter().CalibrationValues.GetInvertLogic(SelectedLabelToCalibrate);
        }

        /// <summary>
        /// Updates the 3 Invert Boolean Toggles (<see cref="InvertBoolX"/>, <see cref="InvertBoolY"/> and <see cref="InvertBoolZ"/>) according to the respective <see cref="SingleInputMappingLabels"/>.
        /// <para>If it is a boolean, the toggle is on</para>
        /// </summary>
        public void UpdateInvertBoolOption(Toggle invertedBoolToggle, bool isInteractable)
        {
            invertedBoolToggle.interactable = isInteractable;
        }

        /// <summary>
        /// Returns the index of the option that is selected
        /// </summary>
        /// <param name="mappingOptionsLabels">List of options to get the index from</param>
        /// <param name="index">The index of the <see cref="GameObjectProperty.SingleInputMappings"/> in the list</param>
        private int SelectPreviousDropdownLabel(List<string> mappingOptionsLabels, int index)
        {
            var singleINputMapping = SelectedGameObjectPropertyGui.GameObjectProperty.SingleInputMappings[index];

            if (singleINputMapping.InputMappingLabels == SingleInputMappingLabels.Bool ||
                singleINputMapping.InputMappingLabels == SingleInputMappingLabels.Value)
                return mappingOptionsLabels.IndexOf(singleINputMapping.InputMappingLabels + " - " +
                                                    singleINputMapping.SingleInput.Label);
           
            return mappingOptionsLabels.IndexOf(singleINputMapping.InputMappingLabels.ToString());
        }

        /// <summary>
        /// Returns a list of strings of all the mapping labels available to be shown in the Gui Property dropdown according to the <see cref="SingleInputMapping.SingleInput"/>
        /// </summary>
        /// <param name="singleInputMapping"></param>
        /// <returns></returns>
        private List<string> GetListOfOptions(SingleInputMapping singleInputMapping)
        {
            var labels = new List<string>();
            var singleInput = singleInputMapping.SingleInput;

            labels.Add(SingleInputMappingLabels.None.ToString());

            foreach (var infoTypes in singleInput.InfoTypes)
            {
                switch (infoTypes)
                {
                    case InformationType.rotation:
                        labels.Add(SingleInputMappingLabels.RotX.ToString());
                        labels.Add(SingleInputMappingLabels.RotY.ToString());
                        labels.Add(SingleInputMappingLabels.RotZ.ToString());
                        break;
                    case InformationType.position:
                        labels.Add(SingleInputMappingLabels.PosX.ToString());
                        labels.Add(SingleInputMappingLabels.PosY.ToString());
                        labels.Add(SingleInputMappingLabels.PosZ.ToString());
                        break;
                    case InformationType.@bool:
                        labels.Add(SingleInputMappingLabels.Bool + " - " + singleInput.Label);
                        break;
                    case InformationType.value:
                        //case InformationType.signal:
                        labels.Add(SingleInputMappingLabels.Value + " - " + singleInput.Label);
                        break;
                    case InformationType.sample:
                        break;
                    default:
                        Debug.LogError("Unkown InformationType: " + infoTypes);
                        break;
                }

            }

            return labels;
        }

        /// <summary>
        /// Updates the Calibration Button for the <see cref="UpdateAxisMappedCalibrationBtn"/> according to the dropdown modified 
        /// </summary>
        /// <param name="axisMappedName">selected label in the dropdown</param>
        /// <param name="currentAxisName">Name of the Dropdown modified</param>
        public void UpdateAxisMapped(string axisMappedName, string currentAxisName)
        {
            Button callibrateBtn;
            ValueReadingBar readingBar;
            int simIndex;
            bool invertBoolToggle;

            if (currentAxisName == MapEnumX.name)
            {
                callibrateBtn = CalibrateBtnX;
                SelectedLabelToCalibrate = AxisLabels.X;
                simIndex = 0;
                invertBoolToggle = InvertBoolX.isOn;
                readingBar = ReadingBarX;
            }
            else if (currentAxisName == MapEnumY.name)
            {
                callibrateBtn = CalibrateBtnY;
                SelectedLabelToCalibrate = AxisLabels.Y;
                simIndex = 1;
                invertBoolToggle = InvertBoolY.isOn;
                readingBar = ReadingBarY;
            }
            else
            {
                callibrateBtn = CalibrateBtnZ;
                SelectedLabelToCalibrate = AxisLabels.Z;
                simIndex = 2;
                invertBoolToggle = InvertBoolZ.isOn;
                readingBar = ReadingBarZ;
            }

            UpdateSingleInputMapping(simIndex, axisMappedName);
            UpdateAxisMappedCalibrationBtn(axisMappedName, callibrateBtn);
            readingBar.UpdateForBooleanBar(GetMappingLabel(axisMappedName) == SingleInputMappingLabels.Bool, invertBoolToggle);
        }

        /// <summary>
        /// Saves in the current <see cref="GameObjectProperty"/> the new mapping that was selected in the GUI
        /// </summary>
        /// <param name="index">The index of the <see cref="GameObjectProperty.SingleInputMappings"/> to save the mapping in</param>
        /// <param name="axisMappedName">the mapping name</param>
        public void UpdateSingleInputMapping(int index, string axisMappedName)
        {
            //TODO TIRAR SWITCH
            var inputMappingLabel = GetMappingLabel(axisMappedName);
            var sim = SelectedGameObjectPropertyGui.GameObjectProperty.SingleInputMappings[index];
            sim.InputMappingLabels = inputMappingLabel;

            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;

            switch (curGop.InfoType)
            {
                case InformationType.rotation:
                    curGop.GetRotInterpreter().SetReadingFrom(SelectedLabelToCalibrate, inputMappingLabel);
                    break;
                case InformationType.position:
                    curGop.GetPosInterpreter().SetReadingFrom(SelectedLabelToCalibrate, inputMappingLabel);
                    break;
                case InformationType.@bool:
                    curGop.GetBoolInterpreter().SetReadingFrom(SelectedLabelToCalibrate, inputMappingLabel);
                    break;
                case InformationType.value:
                    curGop.GetValueInterpreter().SetReadingFrom(SelectedLabelToCalibrate, inputMappingLabel);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the Calibration Button interaction according to the option selected in the <see cref="axisMappedName"/>
        /// </summary>
        /// <param name="axisMappedName">selected label in the dropdown</param>
        /// <param name="callibrateBtn">Button to add the click event</param>
        public void UpdateAxisMappedCalibrationBtn(string axisMappedName, Button callibrateBtn)
        {
            //Debug.Log("axisMappedName: " + axisMappedName);
            var calibrationAxis = GetMappingLabel(axisMappedName);

            callibrateBtn.onClick.RemoveAllListeners();
            if (calibrationAxis == SingleInputMappingLabels.Bool)
                callibrateBtn.gameObject.SetActive(false);
            else
            {
                callibrateBtn.gameObject.SetActive(true);

                var targetAxis = SelectedLabelToCalibrate == AxisLabels.X
                    ? AxisLabels.X
                    : SelectedLabelToCalibrate == AxisLabels.Y
                        ? AxisLabels.Y
                        : AxisLabels.Z;
                
                // adds a listener to the button according to the axis received and the dropdown changed
                if (calibrationAxis == SingleInputMappingLabels.None)
                    callibrateBtn.interactable = false;
                else
                {
                    callibrateBtn.interactable = true;

                    switch (SelectedGameObjectPropertyGui.GameObjectProperty.InfoType)
                    {
                        case InformationType.rotation:
                            callibrateBtn.onClick.AddListener(() => { CalibrateRotation(targetAxis, calibrationAxis); });
                            break;
                        case InformationType.position:
                            callibrateBtn.onClick.AddListener(() => { CalibratePosition(targetAxis, calibrationAxis); });
                            break;
                        case InformationType.@bool:
                            callibrateBtn.onClick.AddListener(() => { CalibrateBoolean(calibrationAxis); });
                            break;
                        case InformationType.value:
                            callibrateBtn.onClick.AddListener(() => { CalibrateValue(calibrationAxis); });
                            break;
                        case InformationType.sample:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            } 
        }

        /// <summary>
        /// Calls the Calibrate funcion of the selected <see cref="ValueInterpreter"/>
        /// </summary>
        /// <param name="calibrationAxis"></param>
        private void CalibrateValue(SingleInputMappingLabels calibrationAxis)
        {
            //Debug.Log("world axis being calibrated - value - " + calibrationAxis);

            var propGui = SelectedGameObjectPropertyGui.GameObjectProperty.Target.GetComponent<ValueInterpreter>();

            propGui.Calibrate(AxisLabels.Value, calibrationAxis);
        }

        /// <summary>
        /// Starts the rotation calibration process for the <see cref="AxisLabels"/> received
        /// <param name="targetAxis"> The <see cref="AxisLabels"/> being calibrated</param>
        /// </summary>
        public void CalibrateRotation(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            //Debug.Log("world axis being calibrated - rotation - " + targetAxis);
            //Debug.Log("calibrationAxis - rotation - " + calibrationAxis);

            var propGui = SelectedGameObjectPropertyGui.GameObjectProperty.Target.GetComponent<RotationInterpreter>();

            propGui.Calibrate(targetAxis, calibrationAxis);
        }

        /// <summary>
        /// Starts the boolean calibration process for the <see cref="BooleanInterpreter"/>
        /// </summary>
        public void CalibrateBoolean(SingleInputMappingLabels calibrationAxis)
        {
            var propGui = SelectedGameObjectPropertyGui.GameObjectProperty.Target.GetComponent<BooleanInterpreter>();

            propGui.Calibrate(AxisLabels.Bool, calibrationAxis);
        }

        /// <summary>
        /// Starts the position calibration process for the <see cref="AxisLabels"/> received
        /// <param name="targetAxis"> The <see cref="AxisLabels"/> being calibrated</param>
        /// </summary>
        public void CalibratePosition(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            var propGui = SelectedGameObjectPropertyGui.GameObjectProperty.Target.GetComponent<PositionInterpreter>();

            propGui.Calibrate(targetAxis, calibrationAxis);
        }

        /// <summary>
        /// Returns the right <see cref="SingleInputMappingLabels"/> according to the string received
        /// </summary>
        /// <param name="dropdownLabel">label shown in the dropdown options</param>
        /// <returns></returns>
        public SingleInputMappingLabels GetMappingLabel(string dropdownLabel)
        {
            if (dropdownLabel.Contains(" - ") == false)
                return Parser.StringToEnum<SingleInputMappingLabels>(dropdownLabel);

            var newLabel = dropdownLabel.Replace(" ", "");
            newLabel = newLabel.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries)[0];

            return Parser.StringToEnum<SingleInputMappingLabels>(newLabel);
        }
        
        /// <summary>
        /// Updates the calibration mode of the correct interpreter
        /// </summary>
        /// <param name="useConverter">The toggle that was changed</param>
        public void UpdateCalibrationDirectMode(Toggle useConverter)
        {
            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;

            switch (curGop.InfoType)
            {
                case InformationType.rotation:
                    var rotInterpreter = curGop.Target.GetComponent<RotationInterpreter>();
                    rotInterpreter.Mode = useConverter.isOn == false ? CalibrationMode.Direct : rotInterpreter.Mode == CalibrationMode.Additive ? rotInterpreter.Mode : CalibrationMode.Absolute;
                    break;
                case InformationType.position:
                    var posInterpreter = curGop.Target.GetComponent<PositionInterpreter>();
                    posInterpreter.Mode = useConverter.isOn == false ? CalibrationMode.Direct : posInterpreter.Mode == CalibrationMode.Additive ? posInterpreter.Mode : CalibrationMode.Absolute;
                    break;
                case InformationType.@bool:
                    var boolInterpreter = curGop.Target.GetComponent<BooleanInterpreter>();
                    boolInterpreter.Mode = useConverter.isOn == false ? CalibrationMode.Direct :  CalibrationMode.Absolute;
                    break;
                case InformationType.value:
                    var valueInterpreter = curGop.Target.GetComponent<ValueInterpreter>();
                    valueInterpreter.Mode = useConverter.isOn == false ? CalibrationMode.Direct : valueInterpreter.Mode == CalibrationMode.Additive ? valueInterpreter.Mode : CalibrationMode.Absolute;
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the calibration mode when the additive mode in the gui is toggled
        /// </summary>
        /// <param name="joystickToggle"></param>
        public void UpdateCalibrationToAdditiveMode(Toggle joystickToggle)
        {
            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;

            switch (curGop.InfoType)
            {
                case InformationType.rotation:
                    UpdateJoystickModeOnClick(curGop.Target.GetComponent<RotationInterpreter>().CalibrationValues, joystickToggle);
                    break;
                case InformationType.position:
                    UpdateJoystickModeOnClick(curGop.Target.GetComponent<PositionInterpreter>().CalibrationValues, joystickToggle);
                    break;
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    UpdateJoystickModeOnClick(curGop.Target.GetComponent<ValueInterpreter>().CalibrationValues, joystickToggle);
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the Joystick mode axis assossiated with the received <see cref="Toggle"/>.
        /// </summary>
        /// <param name="calibrationValues">The calibration values that hold the <see cref="Data"/> for this axis.</param>
        /// <param name="joystickToggle">The <see cref="Toggle"/> for this axis</param>
        private void UpdateJoystickModeOnClick(CalibrationValues calibrationValues, Toggle joystickToggle)
        {
            if (joystickToggle == AdditiveModeX)
                calibrationValues.OutputData[AxisLabels.X].Mode = joystickToggle.isOn
                    ? CalibrationMode.Additive
                    : CalibrationMode.Absolute;

            if (joystickToggle == AdditiveModeY)
                calibrationValues.OutputData[AxisLabels.Y].Mode = joystickToggle.isOn
                    ? CalibrationMode.Additive
                    : CalibrationMode.Absolute;

            if (joystickToggle == AdditiveModeZ)
                 calibrationValues.OutputData[AxisLabels.Z].Mode = joystickToggle.isOn
                    ? CalibrationMode.Additive
                    : CalibrationMode.Absolute;

            if (joystickToggle == SingleEditorAdditiveMode)
                calibrationValues.OutputData[SelectedLabelToCalibrate].Mode = joystickToggle.isOn
                    ? CalibrationMode.Additive
                    : CalibrationMode.Absolute;

            if (joystickToggle == ValueAdditiveMode)
                calibrationValues.OutputData[AxisLabels.Value].Mode = joystickToggle.isOn
                    ? CalibrationMode.Additive
                    : CalibrationMode.Absolute;
        }

        /// <summary>
        /// Updates if the data should be usedor not in the selected gameobject
        /// </summary>
        /// <param name="useOnThisObjectToggle">The toggle that was changed</param>
        public void UpdateUseOnThisObject(Toggle useOnThisObjectToggle)
        {
            var curGop = SelectedGameObjectPropertyGui.GameObjectProperty;

            switch (curGop.InfoType)
            {
                case InformationType.rotation:
                    var rotInterpreter = curGop.Target.GetComponent<RotationInterpreter>();
                    rotInterpreter.UseOnThisObject = useOnThisObjectToggle.isOn;
                    break;
                case InformationType.position:
                    var posInterpreter = curGop.Target.GetComponent<PositionInterpreter>();
                    posInterpreter.UseOnThisObject = useOnThisObjectToggle.isOn;
                    break;
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    var valueInterpreter = curGop.Target.GetComponent<ValueInterpreter>();
                    valueInterpreter.UseOnThisObject = useOnThisObjectToggle.isOn;
                    break;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Updates the invert logic option in the calibration values according to the toggle state and the <see cref="SelectedGameObjectPropertyGui"/>
        /// </summary>
        /// <param name="invertLogicToggle"></param>
        public void UpdateInvertLogic(Toggle invertLogicToggle)
        {
            ValueReadingBar bar = null;
            if (invertLogicToggle == InvertBoolX) //pos/rot X
            {
                SelectedLabelToCalibrate = AxisLabels.X;
                bar = ReadingBarX;
            }
            if (invertLogicToggle == InvertBoolY) //pos/rot Y
            {
                SelectedLabelToCalibrate = AxisLabels.Y;
                bar = ReadingBarY;
            }
            else if (invertLogicToggle == InvertBoolZ) //pos/rot Z
            {
                SelectedLabelToCalibrate = AxisLabels.Z;
                bar = ReadingBarZ;
            }
            else if (invertLogicToggle == SingleEditorInvertBool) //pos/rot single
            {
                bar = SingleEditorReadingBarValue;
            }
            else /*if (invertLogicToggle == ValueInvertBool)*/ //value
            {
                bar = ValueReadingBar;
            }

            if (SelectedLabelToCalibrate == AxisLabels.Bool)
                BooleanBar.UpdateInvertBoolBar(invertLogicToggle.isOn);
            else
                bar.UpdateInvertBoolBar(invertLogicToggle.isOn);
        }
    }
}
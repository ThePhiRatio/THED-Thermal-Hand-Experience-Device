using System;
using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;
using Math = Neurorehab.Scripts.Utilities.Math;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for managing a Value Reading Bar in the Gui
    /// </summary>
    public class ValueReadingBar : MonoBehaviour
    {
        [Header("Bar Gameobjects - Input")]
        public Slider MainBar;
        public RectTransform ToleranceBar;
        public GameObject CenterReadParent;
        public Text CurReadingValue;
        public InputField MinReadingValue;
        public InputField MaxReadingValue;
        public InputField CenterReadingValue;

        [Header("Bar Gameobjects - Input")]
        public Slider MainBarOutput;
        public Text CurMappedValue;
        public InputField MinWorldValue;
        public InputField MaxWorldValue;
        public Toggle Clamp;

        /// <summary>
        /// The <see cref="GameObjectProperty"/> to get the bar values from
        /// </summary>
        private GameObjectProperty _curGop;
        
        /// <summary>
        /// True if the bar is being initilized
        /// </summary>
        private bool _initializingBar = true;
        
        /// <summary>
        /// The last value that was received as the min reading value
        /// </summary>
        private float _lastReadMinValue;
        /// <summary>
        /// 
        /// The last value that was received as the max reading value
        /// </summary>
        private float _lastReadMaxValue;
        
        /// <summary>
        /// True if the bar is showing as a boolean bar
        /// </summary>
        public bool IsBooleanBar { get;  set; }

        /// <summary>
        /// The axis to where the bar belongs to
        /// </summary>
        public AxisLabels Axis { get; set; }

        private void Awake()
        {
            MainBar.minValue = 0;
            MainBar.maxValue = 100; 

            MainBarOutput.minValue = 0;
            MainBarOutput.maxValue = 100;
        }
        
        private void Update()
        {
            UpdateBarCurrentValue(GetPercentageValueBeforeCalibration());
            UpdateBarMappedValue(GetValueAfterCalibration());
        }
        
        /// <summary>
        /// Initiates all the Reading bar values. And hides/shows the <see cref="ToleranceBar"/> according to the tolerance value received.
        /// <para>A tolerance of 0 hides the bar</para>
        /// </summary>
        /// <param name="minValueRead">The minimum value that was read</param>
        /// <param name="maxValueRead"> maximum value that was read</param>
        /// <param name="maxLimit"></param>
        /// <param name="tolerance">The tolerance. Should be a number between 0 and 100</param>
        /// <param name="minLimit"></param>
        /// <param name="clampValue"></param>
        /// <param name="curAxis"></param>
        /// <param name="centerReading"></param>
        public void InitBar(GameObjectProperty gop, float minValueRead, float maxValueRead, float minLimit, float maxLimit, float tolerance, bool clampValue, AxisLabels curAxis, float centerReading = 0)
        {
            _initializingBar = true;
            _curGop = gop;

            UpdateBarTolerance(tolerance);
            MinReadingValue.text =  minValueRead.ToString();
            MaxReadingValue.text = maxValueRead.ToString();
            UpdateCenterRead(centerReading);
            MinWorldValue.text = minLimit.ToString();
            MaxWorldValue.text = maxLimit.ToString();
            Clamp.isOn = clampValue;
            Axis = curAxis;

            _lastReadMinValue = minValueRead;
            _lastReadMaxValue = maxValueRead;

            _initializingBar = false;
        }

        /// <summary>
        /// Update the center read value of the bar in the GUI according to the value received
        /// </summary>
        /// <param name="centerReading">The value to use as the center reading value</param>
        public void UpdateCenterRead(float centerReading)
        {
            CenterReadingValue.text = centerReading.ToString();
        }

        /// <summary>
        ///  Returns the correct percentage (before the calibration process) value to update the bar <see cref="Axis"/>
        /// </summary>
        /// <returns></returns>
        public float GetPercentageValueBeforeCalibration()
        {
            switch (_curGop.InfoType)
            {
                case InformationType.rotation:
                    return Axis == AxisLabels.X
                        ? _curGop.GetRotInterpreter().GetRelativeValue(_curGop.GetRotation().x, AxisLabels.X) * 100
                        : Axis == AxisLabels.Y
                            ? _curGop.GetRotInterpreter().GetRelativeValue(_curGop.GetRotation().y, AxisLabels.Y) * 100
                            : _curGop.GetRotInterpreter().GetRelativeValue(_curGop.GetRotation().z, AxisLabels.Z) * 100;
                case InformationType.position:
                    return Axis == AxisLabels.X
                        ? _curGop.GetPosInterpreter().GetRelativeValue(_curGop.GetPosition().x, AxisLabels.X) * 100
                        : Axis == AxisLabels.Y
                            ? _curGop.GetPosInterpreter().GetRelativeValue(_curGop.GetPosition().y, AxisLabels.Y) * 100
                            : _curGop.GetPosInterpreter().GetRelativeValue(_curGop.GetPosition().z, AxisLabels.Z) * 100;
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    return _curGop.GetValueInterpreter().GetRelativeValue(_curGop.GetFloat(), AxisLabels.Value) * 100;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0f;
        }

        /// <summary>
        /// Returns the value read before calibration
        /// </summary>
        /// <returns></returns>
        public float GetValueBeforeCalibration()
        {
            switch (_curGop.InfoType)
            {
                case InformationType.rotation:
                    return 
                        Axis == AxisLabels.X ?_curGop.GetRotation().x : 
                        Axis == AxisLabels.Y ?_curGop.GetRotation().y : 
                        _curGop.GetRotation().z;
                case InformationType.position:
                    return 
                        Axis == AxisLabels.X ? _curGop.GetPosition().x : 
                        Axis == AxisLabels.Y ? _curGop.GetPosition().y : 
                        _curGop.GetPosition().z;
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    return _curGop.GetFloat();
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0f;
        }

        /// <summary>
        /// Returns the correct mapped value according to the bar <see cref="Axis"/>
        /// </summary>
        /// <returns></returns>
        private float GetValueAfterCalibration()
        {
            switch (_curGop.InfoType)
            {
                case InformationType.rotation:
                    return 
                        Axis == AxisLabels.X ? _curGop.GetRotInterpreter().Value.Rotation.x : 
                        Axis == AxisLabels.Y ? _curGop.GetRotInterpreter().Value.Rotation.y : 
                        _curGop.GetRotInterpreter().Value.Rotation.z;
                case InformationType.position:
                    return 
                        Axis == AxisLabels.X ? _curGop.GetPosInterpreter().Value.Position.x : 
                        Axis == AxisLabels.Y ? _curGop.GetPosInterpreter().Value.Position.y : 
                        _curGop.GetPosInterpreter().Value.Position.z;
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    return _curGop.GetValueInterpreter().Value.Value;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0f;
        }

        /// <summary>
        /// Update the current value of the bar in the GUI and its slider according to the value received
        /// </summary>
        /// <param name="currentValue">The value to use as the current reading value</param>
        public void UpdateBarCurrentValue(float currentValue)
        {
            CurReadingValue.text = GetValueBeforeCalibration().ToString("##.##");
            MainBar.value = currentValue;
        }

        /// <summary>
        /// Update the tolerance bar in the GUI according to the value received
        /// </summary>
        /// <param name="newTolerance">The value to use as the current reading value</param>
        private void UpdateBarTolerance(float newTolerance)
        {
            if (newTolerance != 0)
            {
                ToleranceBar.gameObject.SetActive(true);
                //todo ir buscar o parentwidth dps do awake
                ToleranceBar.sizeDelta = new Vector2(495.92f * newTolerance / 100, ToleranceBar.sizeDelta.y);
            }
            else
                ToleranceBar.gameObject.SetActive(false);
        }

        /// <summary>
        /// Update the mapped value of the bar in the GUI according to the value received
        /// </summary>
        /// <param name="newMappedValue">The value to use as the current reading value</param>
        public void UpdateBarMappedValue(float newMappedValue)
        {
            MainBarOutput.value = GetPercentageValueAfterCalibration();
            CurMappedValue.text = newMappedValue == 0 ? newMappedValue.ToString() : newMappedValue.ToString("##.##");
        }

        /// <summary>
        /// Returns the relative value after calibration, according to the <see cref="Axis"/> variable. Returned value is a float between 0 and 100.
        /// </summary>
        private float GetPercentageValueAfterCalibration()
        {
            switch (_curGop.InfoType)
            {
                case InformationType.rotation:
                    return Axis == AxisLabels.X
                        ? _curGop.GetRotInterpreter().GetRelativeWorldValue(_curGop.GetRotInterpreter().Value.Rotation.x, AxisLabels.X) * 100
                        : Axis == AxisLabels.Y
                            ? _curGop.GetRotInterpreter().GetRelativeWorldValue(_curGop.GetRotInterpreter().Value.Rotation.y, AxisLabels.Y) * 100
                            : _curGop.GetRotInterpreter().GetRelativeWorldValue(_curGop.GetRotInterpreter().Value.Rotation.z, AxisLabels.Z) * 100;
                case InformationType.position:
                    return Axis == AxisLabels.X
                        ? _curGop.GetPosInterpreter().GetRelativeWorldValue(_curGop.GetPosInterpreter().Value.Position.x, AxisLabels.X) * 100
                        : Axis == AxisLabels.Y
                            ? _curGop.GetPosInterpreter().GetRelativeWorldValue(_curGop.GetPosInterpreter().Value.Position.y, AxisLabels.Y) * 100
                            : _curGop.GetPosInterpreter().GetRelativeWorldValue(_curGop.GetPosInterpreter().Value.Position.z, AxisLabels.Z) * 100;
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    return _curGop.GetValueInterpreter().GetRelativeWorldValue(_curGop.GetValueInterpreter().Value.Value, AxisLabels.Value) * 100;
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0f;
        }

        /// <summary>
        /// Saves the tolerance value of the inputfield in the calibration settings
        /// </summary>
        /// <param name="toleranceInput"></param>
        public void SaveTolerance(InputField toleranceInput)
        {
            var value = Math.GetValue(toleranceInput.text);
            UpdateBarTolerance(value);

            if (_initializingBar) return;

            PropertyEditorSaver.SaveTolerance(value, Axis);
        }

        /// <summary>
        /// Saves the speed value of the inputfield in the calibration settings
        /// </summary>
        /// <param name="speedInput"></param>
        public void SaveSpeed(InputField speedInput)
        {
            var value = Math.GetValue(speedInput.text);
            //UpdateBarTolerance(value);

            if (_initializingBar) return;

            PropertyEditorSaver.SaveSpeed(value, Axis);
        }

        /// <summary>
        /// Saves the minimum world value of the inputfield in the calibration settings
        /// </summary>
        public void SaveMinimumWorldValue()
        {
            if (_initializingBar) return;
            
            PropertyEditorSaver.SaveMinOutputValue(Math.GetValue(MinWorldValue.text), Axis);
        }

        /// <summary>
        /// Saves the maximum world value of the inputfield in the calibration settings
        /// </summary>
        public void SaveMaximumWorldValue()
        {
            if (_initializingBar) return;

            PropertyEditorSaver.SaveMaxOutputValue(Math.GetValue(MaxWorldValue.text), Axis);
        }

        /// <summary>
        /// Saves the center read value of the inputfield in the calibration settings
        /// </summary>
        public void SaveCenterRead()
        {
            if (_initializingBar) return;

            PropertyEditorSaver.SaveCenterInputValue(Math.GetValue(CenterReadingValue.text), Axis);
        }

        /// <summary>
        /// Saves the maximum read value of the inputfield in the calibration settings
        /// </summary>
        public void SaveMaximumRead()
        {
            if (IsBooleanBar) return;
            //_savingValues = true;
            var value = Math.GetValue(MaxReadingValue.text);

            //UpdateBarMaximumRead(value);
            _lastReadMaxValue = value;

            if (_initializingBar) return;

            PropertyEditorSaver.SaveMaxInputValue(value, Axis);
            UpdateCenterRead(GetCenter());
        }
        
        /// <summary>
        /// Saves the minimum read value of the inputfield in the calibration settings
        /// </summary>
        public void SaveMinimumRead()
        {
            if (IsBooleanBar) return;
            //_savingValues = true;
            var value = Math.GetValue(MinReadingValue.text);

            //UpdateBarMinimumRead(value);
            _lastReadMinValue = value;

            if (_initializingBar) return;

            PropertyEditorSaver.SaveMinInputValue(value, Axis);

            UpdateCenterRead(GetCenter());
        }

        /// <summary>
        /// Saves the clamp value of the inputfield in the calibration settings
        /// </summary>
        public void SaveClamp()
        {
            if (_initializingBar) return;
            PropertyEditorSaver.SaveClampValue(Clamp.isOn, Axis);
        }

        /// <summary>
        /// Updates the reading bar to show as a boolean or values
        /// </summary>
        /// <param name="isBooleanBar">true if it is a boolean bar</param>
        /// <param name="invertBoolean">true if it is to invert the boolean value</param>
        public void UpdateForBooleanBar(bool isBooleanBar, bool invertBoolean)
        {
            IsBooleanBar = isBooleanBar;
            if (isBooleanBar)
            {
                MinReadingValue.text = invertBoolean ? "TRUE" : "FALSE";
                MaxReadingValue.text = invertBoolean ?  "FALSE" : "TRUE";
                MinReadingValue.interactable = MaxReadingValue.interactable = false;
                CenterReadParent.SetActive(false);
            }
            else
            {
                MinReadingValue.text = _lastReadMinValue.ToString();
                MaxReadingValue.text = _lastReadMaxValue.ToString();
                MinReadingValue.interactable = MaxReadingValue.interactable = true;
                CenterReadParent.SetActive(true);
            }
        }

        /// <summary>
        /// Changes the MinReading value with the MaxReading value
        /// </summary>
        /// <param name="invertBool"></param>
        public void UpdateInvertBoolBar(bool invertBool)
        {
            if (_initializingBar) return; 
            _curGop.GetInterpreter().CalibrationValues.SetInvertLogic(Axis, invertBool);
            
                var minAux = MinReadingValue.text;
                MinReadingValue.text = MaxReadingValue.text;
                MaxReadingValue.text = minAux;
        }

        /// <summary>
        /// Updates the reading values of the bar according to what it receives
        /// </summary>
        /// <param name="minXRead">the new minimum value</param>
        /// <param name="maxXRead">the new maximum value</param>
        /// <param name="savingValues">true if you want to save it as well in the settings</param>
        public void UpdateReadValues(float minXRead, float maxXRead, bool savingValues)
        {
            MinReadingValue.text = minXRead.ToString();
            MaxReadingValue.text = maxXRead.ToString();
        }

        /// <summary>
        /// Gets the input center according to the <see cref="_curGop"/> <see cref="InformationType"/>
        /// </summary>
        /// <returns></returns>
        private float GetCenter()
        {
            switch (_curGop.InfoType)
            {
                case InformationType.unknown:
                    break;
                case InformationType.rotation:
                    return _curGop.GetRotInterpreter().CalibrationValues.GetCenterReadValue(Axis);
                case InformationType.position:
                    return _curGop.GetPosInterpreter().CalibrationValues.GetCenterReadValue(Axis);
                case InformationType.@bool:
                    break;
                case InformationType.value:
                    return _curGop.GetValueInterpreter().CalibrationValues.GetCenterReadValue(Axis);
                case InformationType.sample:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return 0f;
        }
    }
}
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for managing a Boolean Reading Bar in the Gui
    /// </summary>
    public class BooleanReadingBar : MonoBehaviour
    {
        /// <summary>
        /// The main input bar
        /// </summary>
        [Header("Bar Gameobjects - Input")]
        [Tooltip("Input bar")]
        public Slider MainBar;
        /// <summary>
        /// The Treshold Marker that will be placed in the InputBar
        /// </summary>
        [Tooltip("The Treshold Marker that will be placed in the InputBar")]
        public RectTransform TresholdMarker;
        /// <summary>
        /// The current value being read
        /// </summary>
        [Tooltip("The current value being read")]
        public Text CurValue;
        /// <summary>
        /// The input field holding the minimum reading value
        /// </summary>
        [Tooltip("The input field holding the minimum reading value")]
        public InputField MinReadingValue;
        /// <summary>
        /// The input field holding the maximum reading value
        /// </summary>
        [Tooltip("The input field holding the maximum reading value")]
        public InputField MaxReadingValue;
        /// <summary>
        /// The input field holding the treashold value
        /// </summary>
        [Tooltip("The input field holding the treashold value")]
        public InputField ThresholdValue;

        /// <summary>
        /// The main output bar
        /// </summary>
        [Header("Bar Gameobjects - Output")]
        [Tooltip("The main output bar")]
        public Slider MainBarOutput;
        /// <summary>
        /// Text label holding the current mapped value
        /// </summary>
        [Tooltip("Text label holding the current mapped value")]
        public Text MappedValue;
        /// <summary>
        /// Text label holding the minimum output value
        /// </summary>
        [Tooltip("Text label holding the minimum output value")]
        public Text MinBool;
        /// <summary>
        /// Text label holding the maximum output value
        /// </summary>
        [Tooltip("Text label holding the maximum output value")]
        public Text MaxBool;

        /// <summary>
        /// This gameobject's parent width
        /// </summary>
        private float _parentWidth;
        /// <summary>
        /// True if the bar is being initilized
        /// </summary>
        private bool _initializingBar = true;

        /// <summary>
        /// The <see cref="GameObjectProperty"/> to get the bar values from
        /// </summary>
        private GameObjectProperty _curGop;
        
        private Dropdown _boolDropdown;
        
        private void Awake()
        {
            MainBar.minValue = 0;
            MainBar.maxValue = 100;
            MainBarOutput.minValue = 0;
            MainBarOutput.maxValue = 100;
        }

        private void Start()
        {
            _boolDropdown = PropertyEditorGuiManager.Instance.BooleanDropdownEnum;
        }

        private void Update()
        {
            UpdateBarCurrentValue(_curGop.GetBoolInterpreter().GetRelativeValue(_curGop.GetBoolean(), AxisLabels.Bool) * 100);
            UpdateBarMappedValue();

        }

        /// <summary>
        /// Update the current value of the bar in the GUI and its slider according to the value received
        /// </summary>
        /// <param name="currentValue">The value to use as the current reading value</param>
        public void UpdateBarCurrentValue(float currentValue)
        {
            CurValue.text = _curGop.GetBoolean().ToString("##.##");
            MainBar.value = currentValue;
        }

        /// <summary>
        /// Initiates all the boolean bar values. 
        /// </summary>
        /// <param name="gop">The <see cref="GameObjectProperty"/> with the values for this bar</param>
        /// <param name="minValueRead">The initial minimum value read</param>
        /// <param name="maxValueRead">The initial maximum value read</param>
        /// <param name="treshold">The initial treashold value</param>
        /// <param name="invertBool">The initial invertbool value</param>
        public void InitBar(GameObjectProperty gop, float minValueRead, float maxValueRead, float treshold, bool invertBool)
        {
            //print("Init boolean bar. treshold value: " + treshold);
            _parentWidth = MainBar.GetComponent<RectTransform>().rect.width;
            _initializingBar = true;

            _curGop = gop;

            MinReadingValue.text = minValueRead.ToString();
            MaxReadingValue.text = maxValueRead.ToString();
            
            UpdateBarThreshold(treshold, true);
            UpdateInvertBoolBar(invertBool);
            
            _initializingBar = false;
        }
        
        /// <summary>
        /// Updates the <see cref="Data.InvertLogic"/> value. This function does nothing if the bar is first initializing
        /// </summary>
        /// <param name="invertBool">The value for the <see cref="Data.InvertLogic"/> variable.</param>
        public void UpdateInvertBoolBar(bool invertBool)
        {
            if (_initializingBar) return;
            _curGop.GetInterpreter().CalibrationValues.SetInvertLogic(AxisLabels.Bool, invertBool);
            MinBool.text = invertBool ? "TRUE" : "FALSE";
            MaxBool.text = invertBool ? "FALSE" : "TRUE";
        }

        /// <summary>
        /// Updates the bar treshold position in the gui accrding to the received threshold value
        /// </summary>
        /// <param name="threshold">A value between 0 and 360</param>
        /// <param name="updateValue">If true, it also updates the <see cref="ThresholdValue"/> text field.</param>
        public void UpdateBarThreshold(float threshold, bool updateValue = false)
        {
            var boolInterpreter = _curGop.GetBoolInterpreter();
            var amplitude = boolInterpreter.CalibrationValues.InputData[AxisLabels.Bool].Amplitude;

            var tresholdPerc = amplitude == 0f
                ? 0f
                : boolInterpreter.GetRelativeValue(threshold, AxisLabels.Bool);

            //print("UpdateBarThreshold" + threshold + " ||| Amplitude: " + amplitude + " ||| tresholdPerc " + tresholdPerc);
            TresholdMarker.anchoredPosition = new Vector2(Mathf.Clamp(_parentWidth * tresholdPerc, 0 , _parentWidth), TresholdMarker.localPosition.y);

            if(updateValue)
                ThresholdValue.text = threshold.ToString();
        }


        /// <summary>
        /// Update the current value of the bar in the GUI and its slider according to the value received
        /// </summary>
        public void UpdateBarMappedValue()
        {
            var boolVal = _curGop.GetBoolInterpreter().Value.Bool;
            MainBarOutput.value = _curGop.GetBoolInterpreter().GetRelativeValue(_curGop.GetBoolean(), AxisLabels.Bool) * 100;
            MappedValue.text = _curGop.GetInterpreter().CalibrationValues.OutputData[AxisLabels.Bool].InvertLogic ? (!boolVal).ToString() : boolVal.ToString();
        }

        /// <summary>
        /// Saves the maximum read value of the inputfield in the calibration settings. This function does nothing if the bar is initializing.
        /// </summary>
        public void SaveMaximumRead()
        {
            var value = Math.GetValue(MaxReadingValue.text);
            
            if (_initializingBar) return;

            PropertyEditorSaver.SaveMaxInputValue(value, AxisLabels.Bool);
        }

        /// <summary>
        /// Saves the threshold value of the inputfield in the calibration settings
        /// </summary>
        public void SaveThreshold()
        {
            if (_boolDropdown == null) return; //fita colaaaaaaaaaaaaaaaaaaaaaaaaaa

            var label = PropertyEditorGuiManager.Instance.GetMappingLabel(_boolDropdown.options[_boolDropdown.value].text);

            float value;

            if (_curGop.GetBoolInterpreter().CalibrationValues.IsSourceARotation(label))
                value = (Math.GetValue(ThresholdValue.text) + 3600) % 360;
            else
                value = Math.GetValue(ThresholdValue.text);

            UpdateBarThreshold(value);

            if (_initializingBar) return;

            PropertyEditorSaver.SaveThresholdValue(value);
        }

        /// <summary>
        /// Saves the minimum read value of the inputfield in the calibration settings This function does nothing if the bar is initializing.
        /// </summary>
        public void SaveMinimumRead()
        {
            var value = Math.GetValue(MinReadingValue.text);

            if (_initializingBar) return;

            PropertyEditorSaver.SaveMinInputValue(value, AxisLabels.Bool);
        }

        /// <summary>
        /// Updates the reading values of the bar according to what it receives
        /// </summary>
        /// <param name="minRead">the new minimum value</param>
        /// <param name="maxRead">the new maximum value</param>
        /// <param name="savingValues">true if you want to save it as well in the settings</param>
        public void UpdateReadValues(float minRead, float maxRead, bool savingValues)
        {
            //_savingValues = savingValues;
            MinReadingValue.text = minRead.ToString();
            MaxReadingValue.text = maxRead.ToString();
        }
    }
}
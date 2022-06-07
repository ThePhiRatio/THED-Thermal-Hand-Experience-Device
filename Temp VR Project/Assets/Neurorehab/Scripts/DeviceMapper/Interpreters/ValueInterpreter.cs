using System.Collections;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Interpreters
{
    /// <summary>
    /// Responsible for mapping the <see cref="_gameObjectProperty"/> Float
    /// </summary>
    public class ValueInterpreter : Interpreter
    {
        /// <summary>
        /// Sets the value according the configuration. Performs absolute or additive translation of values.
        /// </summary>
        protected override void SetMappedValue()
        {
            var readingValue = new MapperValue
            {
                Value = GameObjectProperty.GetFloat()
            };

            if (GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.Bool) || GameObjectProperty.GetFloat() == 0)
                return;

            if (Mode == CalibrationMode.Direct)
            {
                Value.Value = GameObjectProperty.GetFloat(true);
                return;
            }

            Value.Value = CalibrationValues.OutputData[AxisLabels.Value].Mode == CalibrationMode.Additive ?
                AdditiveConverter.GetValue(readingValue.Value, Value.Value, AxisLabels.Value) :
                AbsoluteConverter.GetValue(readingValue.Value, Value.Value, AxisLabels.Value);

            ClampValue(Value);
        }
        
        /// <summary>
        /// Clamp the values inside the world
        /// </summary>
        protected override void ClampValue(MapperValue value)
        {
            var newValue = CalibrationValues.OutputData[AxisLabels.Value].Clamp
                ? Mathf.Clamp(value.Value, CalibrationValues.OutputData[AxisLabels.Value].Min,
                    CalibrationValues.OutputData[AxisLabels.Value].Max)
                : value.Value;

            value.Value = newValue;
        }

        /// <summary>
        /// Waits for the calibration to finish and sets the local values according to the calibration results
        /// </summary>
        /// <param name="calibrationAxis"></param>
        protected override IEnumerator WaitingForCalibration(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            while (InputCalibrator.Instance.Calibrating)
                yield return null;
            
            CalibrationValues.InputData[targetAxis].ReadingFrom = calibrationAxis;
            CalibrationValues.InputData[targetAxis].Min = InputCalibrator.Instance.Min;
            CalibrationValues.InputData[targetAxis].Max = InputCalibrator.Instance.Max;
            CalibrationValues.InputData[targetAxis].Center = InputCalibrator.Instance.Center;

            PropertyEditorGuiManager.Instance.ValueReadingBar.UpdateReadValues(CalibrationValues.InputData[targetAxis].Min,
                CalibrationValues.InputData[targetAxis].Max, false);

            if (CalibrationValues.IsSourceARotation(calibrationAxis))
                PropertyEditorGuiManager.Instance.ValueReadingBar.UpdateCenterRead(CalibrationValues.InputData[targetAxis].Center);

            InputCalibrator.Instance.ResetValues();
        }
    }
}
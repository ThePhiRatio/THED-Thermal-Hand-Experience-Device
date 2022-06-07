using System.Collections;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Enums;

namespace Neurorehab.Scripts.DeviceMapper.Interpreters
{
    /// <summary>
    /// Responsible for converting the input values into booleans
    /// </summary>
    public class BooleanInterpreter : Interpreter
    {
        /// <summary>
        /// Waits for the calibration to finish and sets the local values according to the calibration results
        /// </summary>
        /// <param name="targetAxis">The axis being calibrated</param>
        /// <param name="calibrationAxis">The axis being mapped to the axes being calibrated</param>
        protected override IEnumerator WaitingForCalibration(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            while (InputCalibrator.Instance.Calibrating)
                yield return null;

            CalibrationValues.InputData[targetAxis].ReadingFrom = calibrationAxis;
            CalibrationValues.InputData[targetAxis].Center = InputCalibrator.Instance.Center;
            CalibrationValues.InputData[targetAxis].Min = InputCalibrator.Instance.Min;
            CalibrationValues.InputData[targetAxis].Max = InputCalibrator.Instance.Max;

            if (CalibrationValues.IsSourceARotation(calibrationAxis))
            {
                CalibrationValues.OutputData[targetAxis].Center = GetRelativeValue(CalibrationValues.InputData[targetAxis].Center, AxisLabels.Bool);
            }

            PropertyEditorGuiManager.Instance.BooleanBar.UpdateBarThreshold(CalibrationValues.InputData[targetAxis].Center, true);
            PropertyEditorGuiManager.Instance.BooleanBar.UpdateReadValues(CalibrationValues.InputData[targetAxis].Min,
                CalibrationValues.InputData[targetAxis].Max, false);

            InputCalibrator.Instance.ResetValues();
        }

        /// <summary>
        /// Does nothing for booleans
        /// </summary>
        /// <param name="value"></param>
        protected override void ClampValue(MapperValue value)
        {
            //Intentionally empty, bool has no clamp
        }

        /// <summary>
        /// Sets the <see cref="Value"/> boolean to true or false, according to the calibrations
        /// </summary>
        protected override void SetMappedValue()
        {
            if (GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.Bool) || GameObjectProperty.GetBoolean() == 0)
                return;

            var @bool = GetRelativeValue(GameObjectProperty.GetBoolean(), AxisLabels.Bool) >= GetRelativeValue(CalibrationValues.OutputData[AxisLabels.Bool].Center, AxisLabels.Bool);

            if (Mode == CalibrationMode.Direct)
            {
                Value.Bool = CalibrationValues.GetInvertLogic(AxisLabels.Bool) ? GameObjectProperty.GetBoolean() <= 0 : GameObjectProperty.GetBoolean() > 0;
                return;
            }

            Value.Bool = CalibrationValues.GetInvertLogic(AxisLabels.Bool) ? @bool == false : @bool;
        }
    }
}
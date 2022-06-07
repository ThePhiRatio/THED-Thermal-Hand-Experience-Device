using System.Collections;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Interpreters
{
    /// <summary>
    /// Responsible for converting the input values into positions
    /// </summary>
    public class PositionInterpreter : Interpreter
    {
        /// <summary>
        /// Sets the position according the configuration. Sets the coordinates as mapped in the device. Performs absolute or additive translation of position.
        /// </summary>
        protected override void SetMappedValue()
        {
            var readingValue = new MapperValue
            {
                Position = GameObjectProperty.GetPosition()
            };
            if (GameObjectProperty.AllInputsAreNone || readingValue.Position == Vector3.zero)
            {
                if(MyTargetTransform != null)
                    Value.Position = MyTargetTransform.position;

                return;
            }

            var previousValue = new MapperValue(Value);

            if(Mode == CalibrationMode.Direct) {
                readingValue.Position = GameObjectProperty.GetPosition(true);
                if (float.IsNaN(readingValue.Position.x) == false && float.IsNaN(readingValue.Position.y) == false && float.IsNaN(readingValue.Position.z) == false)
                    if (UseOnThisObject && MyTargetTransform != null)
                        MyTargetTransform.position = readingValue.Position;
                    else
                        Value = readingValue;
                return;
            }
            
            var x = CalibrationValues.OutputData[AxisLabels.X].Mode == CalibrationMode.Additive ? 
                    AdditiveConverter.GetValue(readingValue.Position.x, Value.Position.x, AxisLabels.X) :
                    AbsoluteConverter.GetValue(readingValue.Position.x, Value.Position.x, AxisLabels.X);

            var y = CalibrationValues.OutputData[AxisLabels.Y].Mode == CalibrationMode.Additive ? 
                    AdditiveConverter.GetValue(readingValue.Position.y, Value.Position.y, AxisLabels.Y) :
                    AbsoluteConverter.GetValue(readingValue.Position.y, Value.Position.y, AxisLabels.Y);

            var z = CalibrationValues.OutputData[AxisLabels.Z].Mode == CalibrationMode.Additive ? 
                    AdditiveConverter.GetValue(readingValue.Position.z, Value.Position.z, AxisLabels.Z) :
                    AbsoluteConverter.GetValue(readingValue.Position.z, Value.Position.z, AxisLabels.Z);

            Value.Position = new Vector3(x, y, z);
            
            ClampValue(Value);
            RemoveNoneValues(Value, previousValue);

            if (float.IsNaN(Value.Position.x) == false && float.IsNaN(Value.Position.y) == false && float.IsNaN(Value.Position.z) == false && UseOnThisObject && MyTargetTransform != null)
                MyTargetTransform.position = Value.Position;
        }

        /// <summary>
        /// For each axis, if the clamping is turned on, clamp the values inside the world limits.
        /// </summary>
        protected override void ClampValue(MapperValue value)
        {
            var newPos = value.Position;
            var x = newPos.x;
            var y = newPos.y;
            var z = newPos.z;

            if (CalibrationValues.OutputData[AxisLabels.X].Clamp)
            {
                if (x > CalibrationValues.OutputData[AxisLabels.X].Max)
                    x = CalibrationValues.OutputData[AxisLabels.X].Max;
                else if (x < CalibrationValues.OutputData[AxisLabels.X].Min)
                    x = CalibrationValues.OutputData[AxisLabels.X].Min;
            }

            if (CalibrationValues.OutputData[AxisLabels.Y].Clamp)
            {
                if (y > CalibrationValues.OutputData[AxisLabels.Y].Max)
                    y = CalibrationValues.OutputData[AxisLabels.Y].Max;
                else if (y < CalibrationValues.OutputData[AxisLabels.Y].Min)
                    y = CalibrationValues.OutputData[AxisLabels.Y].Min;
            }

            if (CalibrationValues.OutputData[AxisLabels.Z].Clamp)
            {
                if (z > CalibrationValues.OutputData[AxisLabels.Z].Max)
                    z = CalibrationValues.OutputData[AxisLabels.Z].Max;
                else if (z < CalibrationValues.OutputData[AxisLabels.Z].Min)
                    z = CalibrationValues.OutputData[AxisLabels.Z].Min;
            }
            value.Position = new Vector3(x, y, z);
        }

        /// <summary>
        /// Waits for the calibration to finish and sets the local values according to the calibration results
        /// </summary>
        /// <param name="targetAxis">The axis to be calibrated</param>
        /// <param name="calibrationAxis"></param>
        protected override IEnumerator WaitingForCalibration(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            while (InputCalibrator.Instance.Calibrating)
                yield return null;
            
            CalibrationValues.InputData[targetAxis].ReadingFrom = calibrationAxis;
            CalibrationValues.InputData[targetAxis].Center = InputCalibrator.Instance.Center;
            CalibrationValues.InputData[targetAxis].Min = InputCalibrator.Instance.Min;
            CalibrationValues.InputData[targetAxis].Max = InputCalibrator.Instance.Max;

            ValueReadingBar bar = null;

            switch (targetAxis)
            {
                case AxisLabels.X:
                    bar = PropertyEditorGuiManager.Instance.ReadingBarX;
                    break;
                case AxisLabels.Y:
                    bar = PropertyEditorGuiManager.Instance.ReadingBarY;
                    break;
                case AxisLabels.Z:
                    bar = PropertyEditorGuiManager.Instance.ReadingBarZ;
                    break;
                default:
                    Debug.LogError("Unkown AxisLabels for position: " + targetAxis);
                    yield break;
            }

            bar.UpdateReadValues(CalibrationValues.InputData[targetAxis].Min,
                CalibrationValues.InputData[targetAxis].Max, false);
            if (CalibrationValues.IsSourceARotation(calibrationAxis))
                bar.UpdateCenterRead(CalibrationValues.InputData[targetAxis].Center);

            InputCalibrator.Instance.ResetValues();
        }

        /// <summary>
        /// For each coordinate, sets the values as the previous, if that axis is being mapped to <see cref="SingleInputMappingLabels.None"/>.
        /// </summary>
        private void RemoveNoneValues(MapperValue value, MapperValue previousValue)
        {
            var x = GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.X) ? previousValue.Position.x : value.Position.x;
            var y = GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.Y) ? previousValue.Position.y : value.Position.y;
            var z = GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.Z) ? previousValue.Position.z : value.Position.z;

            value.Position = new Vector3(x, y, z);
        }
    }
}
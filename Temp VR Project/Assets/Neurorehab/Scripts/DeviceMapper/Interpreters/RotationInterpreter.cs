using System.Collections;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Interpreters
{
    /// <summary>
    /// Responsible for converting the input values into rotations
    /// </summary>
    public class RotationInterpreter : Interpreter
    {
        /// <summary>
        /// Sets the rotation according the configuration. Sets the coordinates as mapped in the device. Performs absolute or additive translation of rotation.
        /// </summary>
        protected override void SetMappedValue()
        {
            var readingValue = new MapperValue
            {
                Rotation = GameObjectProperty.GetRotation()
            };

            if (GameObjectProperty.AllInputsAreNone || readingValue.Rotation == Vector3.zero)
            {
                if (MyTargetTransform != null)
                    Value.Rotation = MyTargetTransform.rotation.eulerAngles;

                return;
            }

            var previousValue = new MapperValue(Value);

            if (Mode == CalibrationMode.Direct)
            {
                readingValue.Rotation = GameObjectProperty.GetRotation(true);
                if (float.IsNaN(readingValue.Rotation.x) == false && float.IsNaN(readingValue.Rotation.y) == false && float.IsNaN(readingValue.Rotation.z) == false)
                {
                    if (UseOnThisObject)
                        GameObjectProperty.Target.transform.rotation = Quaternion.Euler(readingValue.Rotation);

                    Value = readingValue;
                }
                return;
            }

            var x = CalibrationValues.OutputData[AxisLabels.X].Mode == CalibrationMode.Additive ?
                AdditiveConverter.GetValue(readingValue.Rotation.x, Value.Rotation.x, AxisLabels.X) :
                AbsoluteConverter.GetValue(readingValue.Rotation.x, Value.Rotation.x, AxisLabels.X);

            var y = CalibrationValues.OutputData[AxisLabels.Y].Mode == CalibrationMode.Additive ?
                AdditiveConverter.GetValue(readingValue.Rotation.y, Value.Rotation.y, AxisLabels.Y) :
                AbsoluteConverter.GetValue(readingValue.Rotation.y, Value.Rotation.y, AxisLabels.Y);

            var z = CalibrationValues.OutputData[AxisLabels.Z].Mode == CalibrationMode.Additive ?
                AdditiveConverter.GetValue(readingValue.Rotation.z, Value.Rotation.z, AxisLabels.Z) :
                AbsoluteConverter.GetValue(readingValue.Rotation.z, Value.Rotation.z, AxisLabels.Z);

            Value.Rotation = new Vector3(x, y, z);

            ClampValue(Value);

            RemoveNoneValues(Value, previousValue);

            if (float.IsNaN(Value.Rotation.x) == false && float.IsNaN(Value.Rotation.y) == false && float.IsNaN(Value.Rotation.z) == false)
            {
                if (UseOnThisObject)
                    MyTargetTransform.Rotate(previousValue.Rotation - Value.Rotation, Space.World);
            }
        }

        /// <summary>
        /// For each coordinate, sets the values as the previous, if that axis is being mapped to <see cref="SingleInputMappingLabels.None"/>.
        /// </summary>
        private void RemoveNoneValues(MapperValue value, MapperValue previousValue)
        {
            var x = GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.X) ? previousValue.Rotation.x : value.Rotation.x;
            var y = GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.Y) ? previousValue.Rotation.y : value.Rotation.y;
            var z = GameObjectProperty.IsSingleInputMappingLabelNone(AxisLabels.Z) ? previousValue.Rotation.z : value.Rotation.z;

            value.Rotation = new Vector3(x, y, z);
        }

        /// <summary>
        /// Waits for the calibration to finish and sets the local values according to the calibration results
        /// </summary>
        /// <param name="targetAxis">The axis to be calibrated</param>
        /// <param name="calibrationAxis"></param>
        protected override IEnumerator WaitingForCalibration(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            //print("Axis being calibrated: " + targetAxis);
            while (InputCalibrator.Instance.Calibrating)
                yield return null;

            //print(InputCalibrator.Instance.Min + " - " + InputCalibrator.Instance.Max);

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
                    break;
            }

            bar.UpdateReadValues(CalibrationValues.InputData[targetAxis].Min,
                CalibrationValues.InputData[targetAxis].Max, false);
            
            if (CalibrationValues.IsSourceARotation(calibrationAxis))
                //also updates the calibrationValues center
                bar.UpdateCenterRead(CalibrationValues.InputData[targetAxis].Center);

            InputCalibrator.Instance.ResetValues();
        }

        /// <summary>
        /// For each axis, if the clamping is turned on, clamp the values inside the world limits.
        /// </summary>
        protected override void ClampValue(MapperValue value)
        {
            var x = value.Rotation.x;
            var y = value.Rotation.y;
            var z = value.Rotation.z;

            if (GetClampValue(AxisLabels.X))
                x = Mathf.Clamp(x, CalibrationValues.GetMinWorldValue(AxisLabels.X), CalibrationValues.GetMaxWorldValue(AxisLabels.X));
            if (GetClampValue(AxisLabels.Y))
                y = Mathf.Clamp(y, CalibrationValues.GetMinWorldValue(AxisLabels.Y), CalibrationValues.GetMaxWorldValue(AxisLabels.Y));
            if (GetClampValue(AxisLabels.Z))
                z = Mathf.Clamp(z, CalibrationValues.GetMinWorldValue(AxisLabels.Z), CalibrationValues.GetMaxWorldValue(AxisLabels.Z));


            Value.Rotation = new Vector3(x, y, z);
        }
    }
}
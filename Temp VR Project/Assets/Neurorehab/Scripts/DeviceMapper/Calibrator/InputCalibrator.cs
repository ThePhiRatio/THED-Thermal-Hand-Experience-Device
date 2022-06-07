using System.Collections;
using System.Diagnostics;
using Neurorehab.Scripts.Enums;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator
{
    /// <summary>
    /// Used for calibrate RehaPanel devices. Exists one for each mapping made. This is a Singleton Class.
    /// </summary>
    public class InputCalibrator : MonoBehaviour
    {
        /// <summary>
        /// Smallest value read from the input
        /// </summary>
        public float Min;
        /// <summary>
        /// Largest value read from the input
        /// </summary>
        public float Max;
        /// <summary>
        /// The center value read from the input
        /// </summary>
        public float Center;
        /// <summary>
        /// Indicated wheater the calibrator is calibrating or not.
        /// </summary>
        public bool Calibrating;

        /// <summary>
        /// Singleton reference.
        /// </summary>
        public static InputCalibrator Instance;

        public bool ValueAccepted { get; set; }

        private void Awake()
        {
            Instance = this;
            ResetValues();
        }

        /// <summary>
        /// Uses the <see cref="GameObjectProperty"/> received in the parameters to calibrate the min and max value for that component.
        /// </summary>
        /// <param name="gameObjectProperty"></param>
        /// <param name="targetAxis"></param>
        /// <param name="calibrationAxis"></param>
        public void Calibrate(GameObjectProperty gameObjectProperty, AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            StartCoroutine(CalibratePosition(gameObjectProperty, targetAxis, calibrationAxis));
        }
        

        /// <summary>
        /// Coroutine that will read the values and return the max and minimum values read.
        /// </summary>
        /// <param name="calibrationAxis"></param>
        private IEnumerator CalibratePosition(GameObjectProperty gameObjectProperty, AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            //print("calibrationAxis being calibrated: " + calibrationAxis);
            Calibrating = true;
            CalibratorGui.Instance.ShowCalibrationWindow(calibrationAxis, true);
            
            var maxArrow = CalibratorGui.Instance.GetArrow(calibrationAxis, ValueBeingCalibrated.Max);
            var minArrow = CalibratorGui.Instance.GetArrow(calibrationAxis, ValueBeingCalibrated.Min);
            var centerArrow = CalibratorGui.Instance.GetArrow(calibrationAxis, ValueBeingCalibrated.Center);

            maxArrow.SetActive(true);
            if (calibrationAxis != SingleInputMappingLabels.Value)
                yield return StartCoroutine(WaitForTimeCalibration(gameObjectProperty, targetAxis, calibrationAxis,  ValueBeingCalibrated.Max, 2f));
            else
                yield return StartCoroutine(WaitForAcceptCalibration(gameObjectProperty, targetAxis, calibrationAxis, ValueBeingCalibrated.Max));
            
            maxArrow.SetActive(false);

            if (calibrationAxis == SingleInputMappingLabels.RotX || calibrationAxis == SingleInputMappingLabels.RotY ||
                calibrationAxis == SingleInputMappingLabels.RotZ)
            {
                centerArrow.SetActive(true);
                yield return StartCoroutine(WaitForTimeCalibration(gameObjectProperty, targetAxis, calibrationAxis,
                    ValueBeingCalibrated.Center, 2f));
                centerArrow.SetActive(false);
            }

            minArrow.SetActive(true);

            if (calibrationAxis != SingleInputMappingLabels.Value)
                yield return StartCoroutine(WaitForTimeCalibration(gameObjectProperty, targetAxis, calibrationAxis, ValueBeingCalibrated.Min, 2f));
            else
                yield return StartCoroutine(WaitForAcceptCalibration(gameObjectProperty, targetAxis, calibrationAxis, ValueBeingCalibrated.Min));

            minArrow.SetActive(false);

            
            //if (/*IsCalibrationSourceRotation(calibrationAxis) == false && */Min > Max)
            //{
            //    var aux = Max;
            //    Max = Min;
            //    Min = aux;
            //}

            CalibratorGui.Instance.ShowCalibrationWindow(calibrationAxis, false);
            Calibrating = false;
        }

        /// <summary>
        /// Waits for the user to accept the value being calibrated
        /// </summary>
        /// <param name="gameObjectProperty"> The <see cref="GameObjectProperty"/> containing the data</param>
        /// <param name="axis"></param>
        /// <param name="calibrationAxis"></param>
        /// <param name="valueBeingCalibrated"></param>
        /// <returns></returns>
        private IEnumerator WaitForAcceptCalibration(GameObjectProperty gameObjectProperty, AxisLabels axis, SingleInputMappingLabels calibrationAxis, ValueBeingCalibrated valueBeingCalibrated)
        {
            ValueAccepted = false;
            while (ValueAccepted == false)
            {
                SetValue(gameObjectProperty, axis, calibrationAxis, valueBeingCalibrated);
                // value calibration dont require a center value
                CalibratorGui.Instance.ChangeValue(valueBeingCalibrated == ValueBeingCalibrated.Max ? Max : Min, valueBeingCalibrated);
                yield return null;
            }
        }

        /// <summary>
        /// Waits a certain time to calibrate the value in the <see cref="GameObjectProperty"/> received.
        /// </summary>
        /// <param name="gameObjectProperty"></param>
        /// <param name="targetAxis"></param>
        /// <param name="calibrationAxis"></param>
        /// <param name="valueBeingCalibrated"></param>
        /// <param name="timeToWait"></param>
        /// <returns></returns>
        private IEnumerator WaitForTimeCalibration(GameObjectProperty gameObjectProperty, AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis, ValueBeingCalibrated valueBeingCalibrated, float timeToWait)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            while (stopwatch.ElapsedMilliseconds < timeToWait * 1000)
            {
                SetValue(gameObjectProperty, targetAxis, calibrationAxis, valueBeingCalibrated);
                yield return null;
            }
        }

        /// <summary>
        /// Sets a new value for the <see cref="Max"/> if the value in <see cref="GameObjectProperty"/> is bigger
        /// </summary>
        /// <param name="gameObjectProperty">The <see cref="GameObjectProperty"/> with the raw data</param>
        /// <param name="calibrationAxis">The <see cref="SingleInputMappingLabels"/> where the information must be retrieved.</param>
        /// <param name="maxValue"> Indicates if the value being set is the Max value or the Min value. </param>
        private void SetValue(GameObjectProperty gameObjectProperty, AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis, ValueBeingCalibrated valueBeingCalibrated)
        {
            var singleInputIndex = targetAxis == AxisLabels.Bool || targetAxis == AxisLabels.X || targetAxis == AxisLabels.Value ? 0 : targetAxis == AxisLabels.Y ? 1 : 2;

            switch (calibrationAxis)
            {
                case SingleInputMappingLabels.PosX:
                    if (valueBeingCalibrated == ValueBeingCalibrated.Max)
                        Max = gameObjectProperty.GetSingleInput(singleInputIndex).GetPosition().x;
                    else if (valueBeingCalibrated == ValueBeingCalibrated.Min)
                        Min = gameObjectProperty.GetSingleInput(singleInputIndex).GetPosition().x;
                    break;
                case SingleInputMappingLabels.PosY:
                    if (valueBeingCalibrated == ValueBeingCalibrated.Max)
                        Max = gameObjectProperty.GetSingleInput(singleInputIndex).GetPosition().y;
                    else if (valueBeingCalibrated == ValueBeingCalibrated.Min)
                        Min = gameObjectProperty.GetSingleInput(singleInputIndex).GetPosition().y;
                    break;
                case SingleInputMappingLabels.PosZ:
                    if (valueBeingCalibrated == ValueBeingCalibrated.Max)
                        Max = gameObjectProperty.GetSingleInput(singleInputIndex).GetPosition().z;
                    else if (valueBeingCalibrated == ValueBeingCalibrated.Min)
                        Min = gameObjectProperty.GetSingleInput(singleInputIndex).GetPosition().z;
                    break;
                case SingleInputMappingLabels.RotX:
                    var read = (gameObjectProperty.GetSingleInput(singleInputIndex).GetRotation().eulerAngles.x + 3600) % 360;
                    switch (valueBeingCalibrated)
                    {
                        case ValueBeingCalibrated.Max:
                            Max = read;
                            break;
                        case ValueBeingCalibrated.Min:
                            Min = read;
                            break;
                        case ValueBeingCalibrated.Center:
                            Center = read;
                            break;
                    }
                    break;
                case SingleInputMappingLabels.RotY:
                    read = (gameObjectProperty.GetSingleInput(singleInputIndex).GetRotation().eulerAngles.y + 3600) % 360;
                    switch (valueBeingCalibrated)
                    {
                        case ValueBeingCalibrated.Max:
                            Max = read;
                            break;
                        case ValueBeingCalibrated.Min:
                            Min = read;
                            break;
                        case ValueBeingCalibrated.Center:
                            Center = read;
                            break;
                    }
                    break;
                case SingleInputMappingLabels.RotZ:
                    read = (gameObjectProperty.GetSingleInput(singleInputIndex).GetRotation().eulerAngles.z + 3600) % 360;
                    switch (valueBeingCalibrated)
                    {
                        case ValueBeingCalibrated.Max:
                            Max = read;
                            break;
                        case ValueBeingCalibrated.Min:
                            Min = read;
                            break;
                        case ValueBeingCalibrated.Center:
                            Center = read;
                            break;
                    }
                    break;
                case SingleInputMappingLabels.Value:
                    if (valueBeingCalibrated == ValueBeingCalibrated.Max)
                        Max = gameObjectProperty.GetSingleInput(singleInputIndex).GetFloat();
                    else if (valueBeingCalibrated == ValueBeingCalibrated.Min)
                        Min = gameObjectProperty.GetSingleInput(singleInputIndex).GetFloat();
                    break;
                default:
                    Debug.LogError("Unkown SingleInputMappingLabels for calibration: " + calibrationAxis);
                    break;
            }
        }

        /// <summary>
        /// Returns true if the source of the calibration is a rotation.
        /// </summary>
        /// <param name="calibAxis"></param>
        /// <returns></returns>
        private bool IsCalibrationSourceRotation(SingleInputMappingLabels calibAxis)
        {
            return calibAxis == SingleInputMappingLabels.RotX ||
                   calibAxis == SingleInputMappingLabels.RotY ||
                   calibAxis == SingleInputMappingLabels.RotZ;
        }

        /// <summary>
        /// Resets <see cref="Max"/> and <see cref="Min"/> variables.
        /// </summary>
        public void ResetValues()
        {
            Max = float.MinValue;
            Min = float.MaxValue;
            Center = 0f;
        }
    }
}
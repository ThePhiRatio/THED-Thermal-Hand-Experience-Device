using System;
using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator
{
    /// <summary>
    /// Contains all the GUI elements for the Calibrator
    /// </summary>
    public class CalibratorGui : MonoBehaviour
    {
        /// <summary>
        /// Singleton reference
        /// </summary>
        public static CalibratorGui Instance;

        /// <summary>
        /// The panel containing the position indicators
        /// </summary>
        [Header("Position")]
        [Tooltip("The panel containing the position indicators")]
        public GameObject PositionCalibratorWindow;
        /// <summary>
        /// Left border GUI indicator
        /// </summary>
        [Tooltip("Left border GUI indicator")]
        public GameObject Left;
        /// <summary>
        /// Right border GUI indicator
        /// </summary>
        [Tooltip("Right border GUI indicator")]
        public GameObject Right;
        /// <summary>
        /// Forward border GUI indicator
        /// </summary>
        [Tooltip("Forward border GUI indicator")]
        public GameObject Forward;
        /// <summary>
        /// Backward border GUI indicator
        /// </summary>
        [Tooltip("Backward border GUI indicator")]
        public GameObject Backward;
        /// <summary>
        /// Bottom border GUI indicator
        /// </summary>
        [Tooltip("Bottom border GUI indicator")]
        public GameObject Bottom;
        /// <summary>
        /// Top border GUI indicator
        /// </summary>
        [Tooltip("Top border GUI indicator")]
        public GameObject Top;

        /// <summary>
        /// The panel containing the rotation indicators.
        /// </summary>
        [Header("Rotation")]
        [Tooltip("The panel containing the rotation indicators.")]
        public GameObject RotationCalibratorWindow;
        /// <summary>
        /// Indicator for the maximum value of the Z axis rotation
        /// </summary>
        [Tooltip("Indicator for the maximum value of the Z axis rotation")]
        public GameObject RollRight;
        /// <summary>
        /// Indicator for the minimum value of the Z axis rotation
        /// </summary>
        [Tooltip("Indicator for the minimum value of the Z axis rotation")]
        public GameObject RollLeft;
        /// <summary>
        /// Indicator for the maximum value of the X axis rotation
        /// </summary>
        [Tooltip("Indicator for the maximum value of the X axis rotation")]
        public GameObject PitchForward;
        /// <summary>
        /// Indicator for the minimum value of the X axis rotation
        /// </summary>
        [Tooltip("Indicator for the minimum value of the X axis rotation")]
        public GameObject PitchBackward;
        /// <summary>
        /// Indicator for the maximum value of the Y axis rotation
        /// </summary>
        [Tooltip("Indicator for the maximum value of the Y axis rotation")]
        public GameObject YawRight;
        /// <summary>
        /// Indicator for the minimum value of the Y axis rotation
        /// </summary>
        [Tooltip("Indicator for the minimum value of the Y axis rotation")]
        public GameObject YawLeft;
        /// <summary>
        /// Indicator for the minimum value of the Y axis rotation
        /// </summary>
        [Tooltip("Indicator for the center rotation")]
        public GameObject Center;

        /// <summary>
        /// The panel containing the value indicators.
        /// </summary>
        [Header("Values")]
        [Tooltip("The panel containing the value indicators.")]
        public GameObject ValuesCalibratorWindow;
        /// <summary>
        /// The minimum value window
        /// </summary>
        [Tooltip("The minimum value window")]
        public GameObject MinValueWindow;
        /// <summary>
        /// The minimum value indicator
        /// </summary>
        [Tooltip("The minimum value indicator")]
        public Text MinValue;
        /// <summary>
        /// The maximum value window
        /// </summary>
        [Tooltip("The maximum value window")]
        public GameObject MaxValueWindow;
        /// <summary>
        /// The maximum value indicator
        /// </summary>
        [Tooltip("The maximum value indicator")]
        public Text MaxValue;


        private void Awake()
        {
            Instance = this;

            Left.gameObject.SetActive(false);
            Forward.gameObject.SetActive(false);
            Right.gameObject.SetActive(false);
            Backward.gameObject.SetActive(false);
            Top.gameObject.SetActive(false);
            Bottom.gameObject.SetActive(false);
            YawRight.SetActive(false);
            YawLeft.SetActive(false);
            PitchForward.SetActive(false);
            PitchBackward.SetActive(false);
            RollRight.SetActive(false);
            RollLeft.SetActive(false);
            Center.SetActive(false);

            PositionCalibratorWindow.SetActive(false);
            RotationCalibratorWindow.SetActive(false);
            ValuesCalibratorWindow.SetActive(false);
        }

        /// <summary>
        /// Returns the GameObject that represents the value for the received <see cref="CalibrationType"/> and <see cref="AxisLabels"/>. maxValue indicates if the arrow represents the max value or not
        /// </summary>
        /// <param name="calibrationAxis"> The calibration being performed</param>
        /// <param name="maxValue"> Indicates if the arrow represents the max value or not.</param>
        /// <returns> The GameObject that represents the Max value for the received <see cref="CalibrationType"/> and <see cref="AxisLabels"/></returns>
        public GameObject GetArrow(SingleInputMappingLabels calibrationAxis, ValueBeingCalibrated valueBeingCalibrated)
        {
            var maxValue = valueBeingCalibrated == ValueBeingCalibrated.Max;
            switch (calibrationAxis)
            {
                case SingleInputMappingLabels.PosX:
                    return maxValue ? Right : Left;
                case SingleInputMappingLabels.PosY:
                    return maxValue ? Top : Bottom;
                case SingleInputMappingLabels.PosZ:
                    return maxValue ? Forward : Backward;
                case SingleInputMappingLabels.RotX:
                    switch (valueBeingCalibrated)
                    {
                        case ValueBeingCalibrated.Max:
                            return PitchBackward;
                        case ValueBeingCalibrated.Min:
                            return PitchForward;
                        case ValueBeingCalibrated.Center:
                            return Center;
                        default:
                            Debug.LogError("Unkown ValueBeingCalibrated: " + valueBeingCalibrated);
                            return null;
                    }
                case SingleInputMappingLabels.RotY:
                    switch (valueBeingCalibrated)
                    {
                        case ValueBeingCalibrated.Max:
                            return YawRight;
                        case ValueBeingCalibrated.Min:
                            return YawLeft;
                        case ValueBeingCalibrated.Center:
                            return Center;
                        default:
                            Debug.LogError("Unkown ValueBeingCalibrated: " + valueBeingCalibrated);
                            return null;
                    }
                case SingleInputMappingLabels.RotZ:
                    switch (valueBeingCalibrated)
                    {
                        case ValueBeingCalibrated.Max:
                            return RollRight;
                        case ValueBeingCalibrated.Min:
                            return RollLeft;
                        case ValueBeingCalibrated.Center:
                            return Center;
                        default:
                            Debug.LogError("Unkown ValueBeingCalibrated: " + valueBeingCalibrated);
                            return null;
                    }
                case SingleInputMappingLabels.Value:
                    return maxValue ? MaxValueWindow : MinValueWindow;
                case SingleInputMappingLabels.Bool:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("calibrationAxis", calibrationAxis, null);
            }

            //switch (axis)
            //{
            //    case AxisLabels.None:
            //        break;
            //    case AxisLabels.X:
            //        if (calibType == CalibrationType.Value)
            //            return maxValue ? Top.gameObject : Bottom.gameObject;
            //        return maxValue ? Right.gameObject : Left.gameObject;
            //    case AxisLabels.Y:
            //        return maxValue ? Top.gameObject : Bottom.gameObject;
            //    case AxisLabels.Z:
            //        return maxValue ? Forward.gameObject : Backward.gameObject;
            //    default:
            //        throw new ArgumentOutOfRangeException("axis", axis, null);
            //}

            return null;
        }

        /// <summary>
        /// Shows the calibration panel for the <see cref="SingleInputMappingLabels"/> received in the parameters. Hides if <see cref="show"/> is false.
        /// </summary>
        /// <param name="calibrationAxis"></param>
        /// <param name="show"></param>
        public void ShowCalibrationWindow(SingleInputMappingLabels calibrationAxis, bool show)
        {
            if(calibrationAxis == SingleInputMappingLabels.PosX || calibrationAxis == SingleInputMappingLabels.PosY || calibrationAxis == SingleInputMappingLabels.PosZ)
                PositionCalibratorWindow.SetActive(show);
            else if(calibrationAxis == SingleInputMappingLabels.RotX || calibrationAxis == SingleInputMappingLabels.RotY || calibrationAxis == SingleInputMappingLabels.RotZ)
                RotationCalibratorWindow.SetActive(show);
            else if(calibrationAxis == SingleInputMappingLabels.Value)
                ValuesCalibratorWindow.SetActive(show);
        }
        
        /// <summary>
        /// Accepts the value being calibrated
        /// </summary>
        public void AcceptValue()
        {
            InputCalibrator.Instance.ValueAccepted = true;
        }

        /// <summary>
        /// Updates the value text in the calibration window
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueBeingCalibrated"></param>
        public void ChangeValue(float value, ValueBeingCalibrated valueBeingCalibrated)
        {
            switch (valueBeingCalibrated)
            {
                case ValueBeingCalibrated.Max:
                    MaxValue.text = value.ToString();
                    break;
                case ValueBeingCalibrated.Min:
                    MinValue.text = value.ToString();
                    break;
            }
        }
    }
}
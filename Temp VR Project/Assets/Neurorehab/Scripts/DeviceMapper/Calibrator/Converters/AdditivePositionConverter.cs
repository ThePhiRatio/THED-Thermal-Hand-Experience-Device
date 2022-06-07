using System;
using System.Collections.Generic;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator.Converters
{
    /// <summary>
    /// Class responsible for converting the Values read and stored on <see cref="CalibrationValues"/> into an additive position.  The GameObject containing this script must have a <see cref="CalibrationValues"/> and a <see cref="PositionInterpreter"/>
    /// </summary>
    [Serializable]
    public class AdditivePositionConverter : MonoBehaviour, IAdditiveConverter, ISerializationCallbackReceiver
    {

        [SerializeField]
        private List<AxisLabels> _speedKeys = new List<AxisLabels>();
        [SerializeField]
        private List<float> _speedValues = new List<float>();
        [SerializeField]
        private List<AxisLabels> _toleranceKeys = new List<AxisLabels>();
        [SerializeField]
        private List<float> _toleranceValues = new List<float>();

        //////////////////////////////////////////////////////////////
    
        
        /// <summary>
        /// Private reference for the <see cref="Scripts.DeviceMapper.Calibrator.CalibrationValues"/> contaning all the values relevant for this converter.
        /// </summary>
        public CalibrationValues CalibrationValues { get; set; }

        /// <summary>
        /// A dictionary of all The maximum speed values according to the axis.
        /// <para>These values represent how much it will be incremented in each interaction.</para>
        /// </summary>
        public Dictionary<AxisLabels, float> MaxSpeed { get; set; }

        /// <summary>
        /// A dictionary of all the tolerance values according to the axis
        /// </summary>
        public Dictionary<AxisLabels, float> Tolerance { get; set; }

        private void Awake()
        {
            Reset();
        }

        /// <summary>
        /// Gets the final calibrated value for the received <see cref="AxisLabels"/>
        /// </summary>
        /// <param name="valueRead"></param>
        /// <param name="currentValue"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetValue(float valueRead, float currentValue, AxisLabels axis)
        {
            // get the % of this read
            var calibratedValue = CalibrationValues.GetRelativeReadValue(valueRead, axis);
            
            var newAxisValue =
                Mathf.MoveTowards(
                    currentValue, // from here
                    calibratedValue > .5f ? float.MaxValue : float.MinValue, // to here
                    GetTForLerp(calibratedValue, Tolerance[axis] / 100f / 2f) * MaxSpeed[axis] * Time.deltaTime);

            return newAxisValue;
        }

        /// <summary>
        /// Get the normalized T value for this calibration. It returns 1 if the <see cref="value"/> is bigger than 1 or smaller than 0. Returns 0 if the value is inside the tolerance region. Returns Mathf.Abs(value - (.5f - tolerance / 2)) otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private float GetTForLerp(float value, float tolerance)
        {
            if (value <= 0 || value >= 1) // beyond the limits, travel at max speed
                return 1;
            if (value <= .5 + tolerance && value >= .5 - tolerance) // close to center, don't move. % tolerance
                return 0;

            return Mathf.Abs(2 * (value - .5f)); // between the limits, must be = 1 if  value == 0 and value == .5f. Variates linearly till value = 0
        }

        /// <summary>
        /// Resets both dictionaries
        /// </summary>
        public void Reset()
        {
            Tolerance = new Dictionary<AxisLabels, float>
            {
                {AxisLabels.X, 10},
                {AxisLabels.Y, 10},
                {AxisLabels.Z, 10},
                {AxisLabels.Value, 10},
                {AxisLabels.Bool, 10}

            };

            MaxSpeed = new Dictionary<AxisLabels, float>
            {
                {AxisLabels.X, 40},
                {AxisLabels.Y, 40},
                {AxisLabels.Z, 40},
                {AxisLabels.Value, 40},
                {AxisLabels.Bool, 40}

            };
        }

        /// <summary>
        /// Adds or Updates the speed for the received <see cref="AxisLabels"/>
        /// </summary>
        /// <param name="key">The <see cref="AxisLabels"/> to be added/updated.</param>
        /// <param name="value">The value of the speed</param>
        public void AddSpeed(AxisLabels key, float value)
        {
            if (MaxSpeed.ContainsKey(key))
                MaxSpeed[key] = value;
            else
                MaxSpeed.Add(key, value);
        }

        /// <summary>
        /// Adds or Updates the tolerance for the received <see cref="AxisLabels"/>
        /// </summary>
        /// <param name="key">The <see cref="AxisLabels"/> to be added/updated.</param>
        /// <param name="value">The value of the tolerance</param>
        public void AddTolerance(AxisLabels key, float value)
        {
            if (Tolerance.ContainsKey(key))
                Tolerance[key] = value;
            else
                Tolerance.Add(key, value);
        }

        /// <summary>
        /// Callback for <see cref="ISerializationCallbackReceiver"/>
        /// </summary>
        public void OnBeforeSerialize()
        {
            _speedKeys.Clear();
            _speedValues.Clear();
            _toleranceKeys.Clear();
            _toleranceValues.Clear();

            foreach (var data in MaxSpeed)
            {
                _speedKeys.Add(data.Key);
                _speedValues.Add(data.Value);
            }
            foreach (var data in Tolerance)
            {
                _toleranceKeys.Add(data.Key);
                _toleranceValues.Add(data.Value);
            }
        }
        
        /// <summary>
        /// Callback for <see cref="ISerializationCallbackReceiver"/>
        /// </summary>
        public void OnAfterDeserialize()
        {
            //instentionally empty
        }

    }
}
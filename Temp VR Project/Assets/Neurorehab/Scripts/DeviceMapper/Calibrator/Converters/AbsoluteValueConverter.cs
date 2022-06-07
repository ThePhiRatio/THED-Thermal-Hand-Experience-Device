using System;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator.Converters
{
    /// <summary>
    /// Class responsible for converting the Values read and stored on <see cref="CalibrationValues"/> into an absolute value.  The GameObject containing this script must have a <see cref="CalibrationValues"/> and a <see cref="ValueInterpreter"/>
    /// </summary>
    [Serializable]
    public class AbsoluteValueConverter : MonoBehaviour, IConverter
    {
        /// <summary>
        /// Private reference for the <see cref="CalibrationValues"/> contaning all the values relevant for this converter.
        /// </summary>
        public CalibrationValues CalibrationValues { private get; set; }
        
        /// <summary>
        /// returns teh calibrated value for the <see cref="AxisLabels"/> received in the parameters
        /// </summary>
        /// <param name="valueRead"></param>
        /// <param name="currentValue"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public  float GetValue(float valueRead, float currentValue, AxisLabels axis)
        {
            var calibratedValue = CalibrationValues.GetRelativeReadValue(valueRead, axis);

            var minWorldValue = CalibrationValues.GetMinWorldValue(axis);
            var worldAmplitude = CalibrationValues.GetWorldAmplitude(axis);

            return calibratedValue * worldAmplitude + minWorldValue;
        }
    }
}
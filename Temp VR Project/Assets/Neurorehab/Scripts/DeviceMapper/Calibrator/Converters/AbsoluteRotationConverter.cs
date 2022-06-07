using System;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator.Converters
{
    /// <summary>
    /// Class responsible for converting the Values read and stored on <see cref="Scripts.DeviceMapper.Calibrator.CalibrationValues"/> into an absolute rotation.  The GameObject containing this script must have a <see cref="Scripts.DeviceMapper.Calibrator.CalibrationValues"/> and a <see cref="RotationInterpreter"/>
    /// </summary>
    [Serializable]
    public class AbsoluteRotationConverter : MonoBehaviour, IConverter
    {
        /// <summary>
        /// Private reference for the <see cref="CalibrationValues"/> contaning all the values relevant for this converter.
        /// </summary>
        public CalibrationValues CalibrationValues { private get; set; }

        /// <summary>
        /// Returns teh calibrated value for the <see cref="AxisLabels"/> received in the parameters
        /// </summary>
        /// <param name="valueRead"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public float GetValue(float valueRead, float currentValue, AxisLabels axis)
        {
            var calibratedValue = CalibrationValues.GetRelativeReadValue(valueRead, axis);

            var minWorldValue = CalibrationValues.GetMinWorldValue(axis);
            var axisAmpliitude = CalibrationValues.GetWorldAmplitude(axis);

            return calibratedValue * axisAmpliitude + minWorldValue;
        }
    }
}
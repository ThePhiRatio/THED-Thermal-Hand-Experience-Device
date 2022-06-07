using Neurorehab.Scripts.Enums;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator.Converters
{
    /// <summary>
    /// Interface to be used by all data converters
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Private reference for the <see cref="Scripts.DeviceMapper.Calibrator.CalibrationValues"/> contaning all the values relevant for this converter.
        /// </summary>
        CalibrationValues CalibrationValues { set; }
        
        /// <summary>
        /// Calculates the converted value for the <see cref="AxisLabels"/> received in the parameters
        /// </summary>
        /// <param name="valueRead">The latest value read</param>
        /// <param name="currentValue">The previous value converted</param>
        /// <param name="axis">The <see cref="AxisLabels"/> to be calculated</param>
        /// <returns></returns>
        float GetValue(float valueRead, float currentValue, AxisLabels axis);

    }
}
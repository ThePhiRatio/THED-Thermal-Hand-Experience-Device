using System.Collections.Generic;
using Neurorehab.Scripts.Enums;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator.Converters
{
    /// <summary>
    /// Interface that implements <see cref="IConverter"/> in order to add the funcionality of an additive converter
    /// </summary>
    public interface IAdditiveConverter : IConverter
    {
        /// <summary>
        /// Dictionary with the Max allowed speed for each <see cref="AxisLabels"/>
        /// </summary>
        Dictionary<AxisLabels, float> MaxSpeed { get; set; }
        /// <summary>
        /// Dictionary with the tolerance for each <see cref="AxisLabels"/>
        /// </summary>
        Dictionary<AxisLabels, float> Tolerance { get; set; }
        /// <summary>
        /// Function to reset the additive converter
        /// </summary>
        void Reset();
        /// <summary>
        /// Adds or Updates the speed for the received <see cref="AxisLabels"/>
        /// </summary>
        /// <param name="key">The <see cref="AxisLabels"/> to be added/updated.</param>
        /// <param name="value">The value of the speed</param>
        void AddSpeed(AxisLabels key, float value);
        /// <summary>
        /// Adds or Updates the tolerance for the received <see cref="AxisLabels"/>
        /// </summary>
        /// <param name="key">The <see cref="AxisLabels"/> to be added/updated.</param>
        /// <param name="value">The value of the tolerance</param>
        void AddTolerance(AxisLabels key, float value);
    }
}
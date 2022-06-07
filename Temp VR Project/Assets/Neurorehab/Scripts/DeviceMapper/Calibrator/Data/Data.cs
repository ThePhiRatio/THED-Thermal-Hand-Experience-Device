using System;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator.Data
{
    /// <summary>
    /// Responsible for storing all the data relating to one <see cref="Label"/> in <see cref="CalibrationValues"/>
    /// </summary>
    [Serializable]
    public class Data
    {
        /// <summary>
        /// The minimum value for this data
        /// </summary>
        [Tooltip("The minimum value for this data")]
        [SerializeField]
        private float _min;
        /// <summary>
        /// The maximum value for this data
        /// </summary>
        [Tooltip("The maximum value for this data")]
        [SerializeField]
        private float _max;
        /// <summary>
        /// The center value for this data
        /// </summary>
        [Tooltip("The center value for this data")]
        [SerializeField]
        private float _center;
        /// <summary>
        /// The amplitude value for this data
        /// </summary>
        [Tooltip("The amplitude value for this data")]
        [SerializeField]
        private float _amplitude;

        /// <summary>
        /// The <see cref="AxisLabels"/> where is data is being used
        /// </summary>
        [Tooltip("The axis where is data is being used")]
        [SerializeField]
        private AxisLabels _label;

        /// <summary>
        /// Indicates if the final value should be clamped between the <see cref="Max"/> and <see cref="Min"/> values.
        /// </summary>
        [Tooltip("Indicates if the final value should be clamped between the Max and Min values.")]
        [SerializeField]
        private bool _clamp;
        /// <summary>
        /// The <see cref="SingleInputMappingLabels"/> where this information is coming from.
        /// </summary>
        [Tooltip("The SingleInputMappingLabels where this information is coming from.")]
        [SerializeField]
        private SingleInputMappingLabels _readingFrom;
        /// <summary>
        /// The <see cref="CalibrationMode"/> being used for this data
        /// </summary>
        [Tooltip("The CalibrationMode being used for this data")]
        [SerializeField]
        private CalibrationMode _mode;
        /// <summary>
        /// Indicates if the calibration is inverted or not
        /// </summary>
        [Tooltip("Indicates if the calibration is inverted or not")]
        [SerializeField]
        private bool _invertLogic;

        /// <summary>
        /// Public property for the <see cref="_label"/> variable
        /// </summary>
        public AxisLabels Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// Public property for the <see cref="_min"/> variable. When this property is set, the <see cref="Center"/> and <see cref="Amplitude"/> are recalculated.
        /// </summary>
        public float Min
        {
            get { return _min; }
            set
            {
                _min = value;
                CalculateCenter();
                CalculateAmplitude();
            }
        }

        /// <summary>
        /// Public property for the <see cref="_max"/> variable. When this property is set, the <see cref="Center"/> and <see cref="Amplitude"/> are recalculated.
        /// </summary>
        public float Max
        {
            get { return _max; }
            set
            {
                _max = value;
                CalculateCenter();
                CalculateAmplitude();
            }
        }

        /// <summary>
        /// Public property for the <see cref="_center"/> variable. When this property is set, the <see cref="Amplitude"/> is recalculated.
        /// </summary>
        public float Center
        {
            get { return _center; }
            set
            {
                _center = value;
                CalculateAmplitude();
            }
        }

        /// <summary>
        /// Public property for the <see cref="_amplitude"/> variable. When this property is set, the <see cref="Amplitude"/> is recalculated.
        /// </summary>
        public float Amplitude  
        {
            get { return _amplitude; }
            set { _amplitude = value; }
        }

        /// <summary>
        /// Public property for the <see cref="_clamp"/> variable.
        /// </summary>
        public bool Clamp
        {
            get { return _clamp; }
            set { _clamp = value; }
        }
        
        /// <summary>
        /// Public property for <see cref="_readingFrom"/> 
        /// Input (read) - shows which reading we are doing (rotx, posY, value, etc)
        /// Output (world) - shows to what we are outputting to: Axislabel = x and it is a rotation then the label is RotX. AxisLabel = Value, then the label is Value
        /// </summary>
        public SingleInputMappingLabels ReadingFrom
        {
            get { return _readingFrom; }
            set { _readingFrom = value; }
        }

        /// <summary>
        /// Public property for <see cref="_mode"/>.
        /// </summary>
        public CalibrationMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// Indicates if the calibration is inverted or not
        /// </summary>
        public bool InvertLogic
        {
            get { return _invertLogic; }
            set { _invertLogic = value; }
        }

        /// <summary>
        /// Public constructor. Starts with default values for <see cref="ReadingFrom"/> (None) and <see cref="Clamp"/> (false).
        /// </summary>
        /// <param name="label"></param>
        public Data(AxisLabels label)
        {
            Label = label;
            ReadingFrom = SingleInputMappingLabels.None;
            Clamp = true;
        }

        /// <summary>
        /// Returns true if the received <see cref="SingleInputMappingLabels"/> has its sourcce in a Rotation
        /// </summary>
        /// <returns></returns>
        public bool IsSourceARotation()
        {
            return ReadingFrom == SingleInputMappingLabels.RotX ||
                   ReadingFrom == SingleInputMappingLabels.RotY ||
                   ReadingFrom == SingleInputMappingLabels.RotZ;
        }

        /// <summary>
        /// Returns the center value when the source is not a Rotation.
        /// </summary>
        public float GetCenterFromValue()
        {
            return GetAmplitudeFromValue() / 2 + Min;
        }
        
        /// <summary>
        /// Calculates the rotation amplitude in the received axis. Used the middle read to determine which direction the rotation was performed.
        /// </summary>
        /// <returns></returns>
        public float GetAmplitudeFromRotation()
        {
            if (Center == Min && Center == Max) return 0f;

            if (RotateThroughZero())
                return 360 - Mathf.Abs(Max - Min);

            return Mathf.Abs(Max - Min);
        }

        /// <summary>
        /// Returns true of the rotation arc stored in this data passed through zero
        /// </summary>
        /// <returns></returns>
        private bool RotateThroughZero()
        {
            return RotateThroughZeroBiggerMax() ||
                   RotateThroughZeroBiggerMin();
        }

        /// <summary>
        /// Returns true of the rotation arc stored in this data passed through zero and has Max bigger than Min
        /// </summary>
        /// <returns></returns>
        private bool RotateThroughZeroBiggerMax()
        {
            return (0 <= Center && Center <= Min || Max <= Center && Center <= 360) && Max >= Min;
        }

        /// <summary>
        /// Returns true of the rotation arc stored in this data passed through zero and has Min bigger than Max
        /// </summary>
        /// <returns></returns>
        private bool RotateThroughZeroBiggerMin()
        {
            return (Min <= Center && Center <= 360 || 0 <= Center && Center <= Max) && Min >= Max;
        }

        /// <summary>
        /// Returns the amplitude if th source is not a Rotation
        /// </summary>
        /// <returns></returns>
        public float GetAmplitudeFromValue()
        {
            return Max - Min;
        }
        
        /// <summary>
        /// Gets the relative value when the source is not a rotation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float GetRelativeValue(float value)
        {
            return IsSourceARotation() ? GetRelativeValueFromRotation(value) : GetRelativeValueFromValue(value);
        }

        /// <summary>
        /// Calculates the Center
        /// </summary>
        public void CalculateCenter()
        {
            Center =
                IsSourceARotation() == false ?
                    GetCenterFromValue() :
                    Center;
        }

        /// <summary>
        /// Calculates the Amplitude
        /// </summary>
        public void CalculateAmplitude()
        {
            Amplitude =
                IsSourceARotation() == false ?
                    GetAmplitudeFromValue() :
                    GetAmplitudeFromRotation();
        }
        
        /// <summary>
        /// Get relative value when source is not a rotation
        /// </summary>
        private float GetRelativeValueFromValue(float value)
        {
            return Amplitude != 0 ? (value - Min) / Amplitude : 0;
        }

        /// <summary>
        /// Get relative value when source is a rotation
        /// </summary>
        private float GetRelativeValueFromRotation(float value)
        {
            Center = (Center + 3600) % 360;

            if (RotateThroughZeroBiggerMax())
                return GetRelativeValueWhenRotateThroughZeroBiggerMax(value);

            if (RotateThroughZeroBiggerMin())
                return GetRelativeValueWhenRotateThroughZeroBiggerMin(value);

            if(RotateRegularBiggerMax())
                return GetRelativeValueWhenRotateRegularBiggerMax(value);

            if (RotateRegularBiggerMin())
                return GetRelativeValueWhenRotateRegularBiggerMin(value);

            // theorically unreacheble code
            Debug.LogError("Code should have no reached here. Check logic above this line.");
            return 0f;
        }

        /// <summary>
        /// Gets the relative reading value when the rotation arc stored in this data does not pass through zero and has Min bigger than Max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float GetRelativeValueWhenRotateRegularBiggerMin(float value)
        {
            if(Max <= value && value <= Min)
                return (Min - value) / Amplitude;

            if (Max <= value && value <= 360)
                return 0f;

            return 1f;
        }

        /// <summary>
        /// Gets the relative reading value when the rotation arc stored in this data does not pass through zero and has Min bigger than Max.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float GetRelativeValueWhenRotateRegularBiggerMax(float value)
        {
            if(Min <= value && value <= Max)
                return (value - Min) / Amplitude;

            if (0 <= value && value <= Min)
                return 0f;

            return 1f;
        }

        /// <summary>
        /// Returns true if the rotation arc stored in this data does not pass through zero and has Min bigger than Max
        /// </summary>
        /// <returns></returns>
        private bool RotateRegularBiggerMin()
        {
            return Max <= Center && Center <= Min;
        }

        /// <summary>
        /// Returns true if the rotation arc stored in this data does not pass through zero and has MAx bigger than Min
        /// </summary>
        /// <returns></returns>
        private bool RotateRegularBiggerMax()
        {
            return Min <= Center && Center <= Max;
        }

        /// <summary>
        /// Gets the relative value for the received parameter when the rotation on this data rotates through Zero, with Max bigger than Min. Example: Min - 300. Center - . Max - 50;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float GetRelativeValueWhenRotateThroughZeroBiggerMin(float value)
        {
            if (180 <= value && value <= Min)
                return 0f;
            if (Min <= value && value <= 360)
                return (value - Min) / Amplitude;
            if (0 <= value && value <= Max)
                return (360 - Min + value) / Amplitude;

            return 1f;
        }

        /// <summary>
        /// Gets the relative value for the received parameter when the rotation on this data rotates through Zero, with Max bigger than Min. Example: Min - 50. Center - . Max - 300;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float GetRelativeValueWhenRotateThroughZeroBiggerMax(float value)
        {
            if (value >= Min && value <= 180)
                return 0f;
            if (0 <= value && value <= Min)
                return (Min - value) / Amplitude;
            if (Max <= value && value <= 360)
                return (360 - value + Min) / Amplitude;
            return 1f;
        }

        /// <summary>
        /// Resets the data
        /// </summary>
        public void Reset()
        {
            Min = 0;
            Max = 0;
            Center = 0;
            Amplitude = 0;
            Clamp = true;
        }
    }
}
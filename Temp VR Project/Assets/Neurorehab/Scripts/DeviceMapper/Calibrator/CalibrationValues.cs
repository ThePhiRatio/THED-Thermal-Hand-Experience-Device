using System;
using System.Collections.Generic;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator
{
    /// <summary>
    /// Script that contains the data relevant to the position of GameObject. It has values for its Boundries and all the values read from the input
    /// </summary>
    [Serializable]
    public class CalibrationValues : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region serialization
        /// <summary>
        /// List of InputKeys used for serialization
        /// </summary>
        [SerializeField]
        private List<AxisLabels> _inputKeys = new List<AxisLabels>();
        /// <summary>
        /// List of InputValues used for serialization
        /// </summary>
        [SerializeField]
        private List<Data.Data> _inputValues = new List<Data.Data>();

        /// <summary>
        /// List of OutputKeys used for serialization
        /// </summary>
        [SerializeField]
        private List<AxisLabels> _outputKeys = new List<AxisLabels>();
        /// <summary>
        /// List of OutputValues used for serialization
        /// </summary>
        [SerializeField]
        private List<Data.Data> _outputValues = new List<Data.Data>();

        /// <summary>
        /// Callback for <see cref="ISerializationCallbackReceiver"/>
        /// </summary>
        public void OnBeforeSerialize()
        {
            _inputKeys.Clear();
            _inputValues.Clear();
            _outputKeys.Clear();
            _outputValues.Clear();

            foreach (var data in InputData)
            {
                _inputKeys.Add(data.Key);
                _inputValues.Add(data.Value);
            }
            foreach (var data in OutputData)
            {
                _outputKeys.Add(data.Key);
                _outputValues.Add(data.Value);
            }
        }

        /// <summary>
        /// Callback for <see cref="ISerializationCallbackReceiver"/>
        /// </summary>
        public void OnAfterDeserialize() { /* Intensionally empty */ }

        #endregion
        
        /// <summary>
        /// Dictionary thats stores a <see cref="Data.Data"/> for each <see cref="AxisLabels"/> input.
        /// </summary>
        public Dictionary<AxisLabels, Data.Data> InputData { get; set; }

        /// <summary>
        /// Dictionary thats stores a <see cref="Data.Data"/> for each <see cref="AxisLabels"/> output.
        /// </summary>
        public Dictionary<AxisLabels, Data.Data> OutputData { get; set; }

        /// <summary>
        /// Initializes the calibration values dictionaries. And updates the <see cref="OutputData"/> according to the <see cref="InformationType"/> received
        /// </summary>
        /// <param name="infoType"></param>
        public void Init(InformationType infoType)
        {
            InputData = new Dictionary<AxisLabels, Data.Data>
            {
                {AxisLabels.X, new Data.Data(AxisLabels.X)},
                {AxisLabels.Y, new Data.Data(AxisLabels.Y)},
                {AxisLabels.Z, new Data.Data(AxisLabels.Z)},
                {AxisLabels.Value, new Data.Data(AxisLabels.Value)},
                {AxisLabels.Bool, new Data.Data(AxisLabels.Bool)},

            };
            OutputData = new Dictionary<AxisLabels, Data.Data>
            {

                {AxisLabels.X, new Data.Data(AxisLabels.X)},
                {AxisLabels.Y, new Data.Data(AxisLabels.Y)},
                {AxisLabels.Z, new Data.Data(AxisLabels.Z)},
                {AxisLabels.Value, new Data.Data(AxisLabels.Value)},
                {AxisLabels.Bool, new Data.Data(AxisLabels.Bool)},
            };
            
            switch (infoType)
            {
                case InformationType.rotation:
                    OutputData[AxisLabels.X].ReadingFrom = SingleInputMappingLabels.None;
                    OutputData[AxisLabels.Y].ReadingFrom = SingleInputMappingLabels.None;
                    OutputData[AxisLabels.Z].ReadingFrom = SingleInputMappingLabels.None;
                    break;
                case InformationType.position:
                    OutputData[AxisLabels.X].ReadingFrom = SingleInputMappingLabels.None;
                    OutputData[AxisLabels.Y].ReadingFrom = SingleInputMappingLabels.None;
                    OutputData[AxisLabels.Z].ReadingFrom = SingleInputMappingLabels.None;
                    break;
                case InformationType.@bool:
                    OutputData[AxisLabels.Bool].ReadingFrom = SingleInputMappingLabels.None;
                    break;
                case InformationType.value:
                    OutputData[AxisLabels.Value].ReadingFrom = SingleInputMappingLabels.None;
                    break;
                case InformationType.sample:
                    OutputData[AxisLabels.Value].ReadingFrom = SingleInputMappingLabels.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("infoType", infoType, null);
            }
        }

        /// <summary>
        ///  Returns the max world value for the <see cref="AxisLabels"/> received.
        /// </summary>
        /// <param name="axis"> The <see cref="AxisLabels"/> to get the value from. </param>
        /// <returns></returns>
        public float GetMaxWorldValue(AxisLabels axis)
        {
            return OutputData[axis].Max;
        }

        /// <summary>
        ///  Returns the min world value for the <see cref="AxisLabels"/> received.
        /// </summary>
        /// <param name="axis"> The <see cref="AxisLabels"/> to get the value from. </param>
        /// <returns></returns>
        public float GetMinWorldValue(AxisLabels axis)
        {
            return OutputData[axis].Min;
        }

        /// <summary>
        /// Returns a float betweem 0 and 1, representing the percentage that the new read represents of the amplitude of that axis.
        /// </summary>
        /// <param name="readValue"> The latest value read. </param>
        /// <param name="axis"> The axis where the latestes value read is from. </param>
        /// <returns></returns>
        public float GetRelativeReadValue(float readValue, AxisLabels axis)
        {
            return InputData[axis].GetRelativeValue(readValue);
        }

        /// <summary>
        /// Returns a float betweem 0 and 1, representing the percentage that the new read represents of the amplitude of that axis.
        /// </summary>
        /// <param name="value"> The latest value read. </param>
        /// <param name="axis"> The axis where the latestes value read is from. </param>
        /// <returns></returns>
        public float GetRelativeWorldValue(float value, AxisLabels axis)
        {
            return OutputData[axis].GetRelativeValue(value);
        }

        /// <summary>
        /// TODO comentario
        /// </summary>
        /// <param name="readingFrom"> The source of the readings. </param>
        /// <returns></returns>
        public bool IsSourceARotation(SingleInputMappingLabels readingFrom)
        {
            return readingFrom == SingleInputMappingLabels.RotX ||
                   readingFrom == SingleInputMappingLabels.RotY ||
                   readingFrom == SingleInputMappingLabels.RotZ;
        }

        /// <summary>
        /// Sets the max read value according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the max  read value will be saved on</param>
        public void SetMaxReadValue(float value, AxisLabels axis)
        {
            InputData[axis].Max = value;
        }

        /// <summary>
        /// Sets the min read value according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min read value will be saved on</param>
        public void SetMinReadValue(float value, AxisLabels axis)
        {
            InputData[axis].Min = value;
        }

        /// <summary>
        /// Sets the min world read value according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min world read value will be saved on</param>
        public void SetMinWorldReadValue(float value, AxisLabels axis)
        {
            OutputData[axis].Min = value;
        }

        /// <summary>
        /// Sets the max world read value according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the max world read value will be saved on</param>
        public void SetMaxWorldReadValue(float value, AxisLabels axis)
        {
            OutputData[axis].Max = value;
        }

        /// <summary>
        /// Sets the center read value according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the center read value will be saved on</param>
        public void SetCenterReadValue(float value, AxisLabels axis)
        {
            InputData[axis].Center = value;
        }

        /// <summary>
        /// Returns the min read value according to the given axis
        /// </summary>
        /// <param name="axis">axis where the min read value will be retrived from</param>
        /// <returns></returns>
        public float GetMinReadValue(AxisLabels axis)
        {
            return InputData[axis].Min;
        }

        /// <summary>
        /// Returns the max read value according to the given axis
        /// </summary>
        /// <param name="axis">axis where the min read value will be retrived from</param>
        /// <returns></returns>
        public float GetMaxReadValue(AxisLabels axis)
        {
            return InputData[axis].Max;
        }

        /// <summary>
        /// Returns the center read value according to the given axis
        /// </summary>
        /// <param name="axis">axis where the center read value will be retrived from</param>
        /// <returns></returns>
        public float GetCenterReadValue(AxisLabels axis)
        {
            return InputData[axis].Center;
        }

        /// <summary>
        /// Returns the <see cref="Data.Data.Amplitude"/> for the received <see cref="AxisLabels"/>
        /// </summary>
        public float GetWorldAmplitude(AxisLabels axis)
        {
            return OutputData[axis].Amplitude;
        }

        /// <summary>
        /// Resets both dictionaries, <see cref="InputData"/> and <see cref="OutputData"/>
        /// </summary>
        public void Reset()
        {
            foreach (var data in InputData.Values)
            {
                data.Reset();
            }
            foreach (var data in OutputData.Values)
            {
                data.Reset();
            }
        }
        
        /// <summary>
        /// Adds the received <see cref="AxisLabels"/> - <see cref="Data.Data"/> pair to the <see cref="InputData"/> dictionary.
        /// </summary>
        public void AddInputValue(AxisLabels key, Data.Data value)
        {
            if (InputData.ContainsKey(key))
                InputData[key] = value;
            else
                InputData.Add(key, value);
        }

        /// <summary>
        /// Adds the received <see cref="AxisLabels"/> - <see cref="Data.Data"/> pair to the <see cref="OutputData"/> dictionary.
        /// </summary>
        public void AddOutputValue(AxisLabels key, Data.Data value)
        {
            if (OutputData.ContainsKey(key))
                OutputData[key] = value;
            else
                OutputData.Add(key, value);
        }

        /// <summary>
        /// Returns the invert logic for the received axis
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public bool GetInvertLogic(AxisLabels axis)
        {
            return OutputData[axis].InvertLogic;
        }

        /// <summary>
        /// Sets the invert logic for the received axis
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public void SetInvertLogic(AxisLabels axis, bool value)
        {
            OutputData[axis].InvertLogic = value;
        }
    }
}
using System;
using System.Collections;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Converters;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Interpreters
{
    [Serializable]
    public abstract class Interpreter : MonoBehaviour
    {
        [SerializeField]
        private bool _useOnThisObject = true;
        [SerializeField]
        private MapperValue _value;
        [SerializeField]
        private CalibrationMode _mode;

        protected Transform MyTargetTransform
        {
            get
            {
                if (GameObjectProperty == null)
                    return null;
                if (GameObjectProperty.Target == null)
                    return null;

                return GameObjectProperty.Target.transform;
            }
        }

        /// <summary>
        /// Private reference for the <see cref="CalibrationValues"/> containing all the values relevant for this interpreter.
        /// </summary>
        [SerializeField]
        private CalibrationValues _calibrationValues;

        /// <summary>
        /// Private referecne for the <see cref="IConverter"/> this script will use to calculate an absolute rotation, if the <see cref="Mode"/> is set to <see cref="CalibrationMode.Absolute"/>
        /// </summary>
        private IConverter _absoluteConverter;
        /// <summary>
        /// Private referecne for the <see cref="IAdditiveConverter"/> this script will use to calculate an additive rotation, if the <see cref="Mode"/> is set to <see cref="CalibrationMode.Additive"/>
        /// </summary>
        [SerializeField]
        private IAdditiveConverter _additiveConverter;

        /// <summary>
        /// Private referecne for the <see cref="IConverter"/> this script will use to calculate an absolute rotation, if the <see cref="Mode"/> is set to <see cref="CalibrationMode.Absolute"/>
        /// </summary>
        public IConverter AbsoluteConverter
        {
            get { return _absoluteConverter; }
            set { _absoluteConverter = value; }
        }

        /// <summary>
        /// Private referecne for the <see cref="IAdditiveConverter"/> this script will use to calculate an additive rotation, if the <see cref="Mode"/> is set to <see cref="CalibrationMode.Additive"/>
        /// </summary>
        public IAdditiveConverter AdditiveConverter
        {
            get { return _additiveConverter; }
            set { _additiveConverter = value; }
        }

        /// <summary>
        /// The <see cref="Scripts.DeviceMapper.GameObjectProperty"/> from where the <see cref="RotationInterpreter"/> gets its Data
        /// </summary>
        public GameObjectProperty GameObjectProperty;



        /// <summary>
        /// Defines the Calibration Mode for this component.
        /// </summary>
        public CalibrationMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// Sets the boolean logic to inverted logic.
        /// </summary>
        //public bool InvertLogic { get; set; }

        /// <summary>
        /// Indicates if the values should be appled to the transform of this game object
        /// </summary>
        public bool UseOnThisObject
        {
            get { return _useOnThisObject; }
            set { _useOnThisObject = value; }
        }

        /// <summary>
        /// The current calibrated value
        /// </summary>
        public MapperValue Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Private reference for the <see cref="CalibrationValues"/> containing all the values relevant for this interpreter.
        /// </summary>
        public CalibrationValues CalibrationValues
        {
            get { return _calibrationValues; }
            set
            {
                _calibrationValues = value;
                _calibrationValues.Init(GameObjectProperty.InfoType);
            }
        }

        private void Start()
        {
            Value = new MapperValue
            {
                Rotation = MyTargetTransform.rotation.eulerAngles,
                Position = MyTargetTransform.position,
                Bool = false,
                Value = 0
            };
        }

        //private void Start()
        //{
        //    InitializeConverters();
        //}

        private void FixedUpdate()
        {
            if (GameObjectProperty == null) return;

            SetMappedValue();
        }
        

        /// <summary>
        /// Sets the rotation according the configuration. Sets the coordinates as mapped in the device. Performs absolute or additive translation of rotation.
        /// </summary>
        protected abstract void SetMappedValue();

        ///// <summary>
        ///// Set the <see cref="IAdditiveConverter"/> and <see cref="IConverter"/> references and set their <see cref="CalibrationValues"/> reference to the same one as in this <see cref="Interpreter"/>.
        ///// </summary>>
        public void InitializeConverters(IConverter absoluteConverter, IAdditiveConverter additiveConverter)
        {
            AdditiveConverter = additiveConverter;
            AbsoluteConverter = absoluteConverter;

            AdditiveConverter.CalibrationValues = CalibrationValues;
            AbsoluteConverter.CalibrationValues = CalibrationValues;
        }

        /// <summary>
        /// Saves the received <see cref="SingleInputGui"/> reference in the <see cref="GameObjectProperty"/>
        /// </summary>
        /// <param name="gameObjectProperty"><see cref="SingleInputGui"/> to reference in the <see cref="GameObjectProperty"/> </param>
        public void BindToInput(GameObjectProperty gameObjectProperty)
        {
            GameObjectProperty = gameObjectProperty;
        }

        /// <summary>
        /// Puts the <see cref="GameObjectProperty"/> as null. Resets to the initial rotation
        /// </summary>
        public void StopBind()
        {
            ResetInterpreter();
            GameObjectProperty = null;
        }
        /// <summary>
        /// Called from unity OnClick. Starts the calibration process for the <see cref="AxisLabels"/> received
        /// <param name="targetAxis"> The <see cref="AxisLabels"/> being calibrated</param>
        /// </summary>
        public void Calibrate(AxisLabels targetAxis, SingleInputMappingLabels calibrationAxis)
        {
            InputCalibrator.Instance.Calibrate(GameObjectProperty, targetAxis, calibrationAxis);
            StartCoroutine(WaitingForCalibration(targetAxis, calibrationAxis));
        }

        /// <summary>
        /// Waits for the calibration to finish and sets the local values according to the calibration results
        /// </summary>
        /// <param name="targetAxis">The axis to be calibrated</param>
        /// <param name="calibrationAxis"></param>
        protected abstract IEnumerator WaitingForCalibration(AxisLabels targetAxis,
            SingleInputMappingLabels calibrationAxis);
        
        protected abstract void ClampValue(MapperValue value);

        /// <summary>
        /// Sets the max read value of its <see cref="CalibrationValues"/> according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the max read value will be saved on</param>
        public void SetMaxInputValue(float value, AxisLabels axis)
        {
            CalibrationValues.SetMaxReadValue(value, axis);
        }

        /// <summary>
        /// Sets the min read value of its <see cref="CalibrationValues"/> according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min read value will be saved on</param>
        public void SetMinInputValue(float value, AxisLabels axis)
        {
            CalibrationValues.SetMinReadValue(value, axis);
        }

        /// <summary>
        /// Sets the max world read value of its <see cref="CalibrationValues"/> according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the max world read value will be saved on</param>
        public void SetMaxOutputValue(float value, AxisLabels axis)
        {
            CalibrationValues.SetMaxWorldReadValue(value, axis);
        }

        /// <summary>
        /// Sets the min world read value of its <see cref="CalibrationValues"/> according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the min world read value will be saved on</param>
        public void SetMinOutputValue(float value, AxisLabels axis)
        {
            CalibrationValues.SetMinWorldReadValue(value, axis);
        }

        /// <summary>
        /// Sets the center read value of its <see cref="CalibrationValues"/> according to the received axis
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the center read value will be saved on</param>
        public void SetCenterInputValue(float value, AxisLabels axis)
        {
            CalibrationValues.SetCenterReadValue(value, axis);
        }

        /// <summary>
        /// Sets the tolerance value in its <see cref="AdditivePositionConverter"/> according to the received axis 
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the tolerance value will be saved on</param>
        public void SetTolerance(float value, AxisLabels axis)
        {
            AdditiveConverter.Tolerance[axis] = value;
        }

        /// <summary>
        /// Sets the speed value in its <see cref="AdditivePositionConverter"/> according to the received axis 
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="axis">axis where the tolerance value will be saved on</param>
        public void SetSpeed(float value, AxisLabels axis)
        {
            AdditiveConverter.MaxSpeed[axis] = value;
        }

        /// <summary>
        /// Sets the clamp value according to the received axis
        /// </summary>
        /// <param name="value">true = actiavte clamp</param>
        /// <param name="axis">axis where the clamp value will be saved on</param>
        public void SetClampValue(bool value, AxisLabels axis)
        {
            CalibrationValues.OutputData[axis].Clamp = value;
        }

        /// <summary>
        /// Returns the clamp value according to the axis received
        /// </summary>
        /// <param name="axis">axis from where the clamp will be retrieved</param>
        /// <returns></returns>
        public bool GetClampValue(AxisLabels axis)
        {
            return CalibrationValues.OutputData[axis].Clamp;
        }

        /// <summary>
        /// Returns the speed value of the <see cref="AdditiveRotationConverter"/> according to the axis received
        /// </summary>
        /// <param name="axis">axis from where the speed will be retrieved</param>
        /// <returns></returns>
        public float GetSpeed(AxisLabels axis)
        {
            return AdditiveConverter.MaxSpeed[axis];
        }

        /// <summary>
        /// Returns the tolerance value of the <see cref="AdditiveRotationConverter"/> according to the axis received
        /// </summary>
        /// <param name="axis">axis from where the tolerance will be retrieved</param>
        /// <returns></returns>
        public float GetTolerance(AxisLabels axis)
        {
            return AdditiveConverter.Tolerance[axis];
        }

        /// <summary>
        /// Sets the readingFrom variable according to the axis to the inputMappingLabel received
        /// </summary>
        /// <param name="axis">the axis label that will change the corresponding readingfrom variable</param>
        /// <param name="inputMappingLabel">The label that we are reading from</param>
        public void SetReadingFrom(AxisLabels axis, SingleInputMappingLabels inputMappingLabel)
        {
            CalibrationValues.InputData[axis].ReadingFrom = inputMappingLabel;
        }

        /// <summary>
        /// Get the relative value of the latest read used by the bool interpreter.
        /// </summary>
        /// <param name="axis">The axis to get the value from.</param>
        /// <returns></returns>
        public float GetRelativeValue(float value, AxisLabels axis)
        {
            return _calibrationValues.GetRelativeReadValue(value, axis);
        }

        public AxisLabels GetAxisLabel(int index)
        {
            switch (index)
            {
                case 0:
                    switch (GameObjectProperty.InfoType)
                    {
                        case InformationType.rotation:
                        case InformationType.position:
                            return AxisLabels.X;
                        case InformationType.@bool:
                            return AxisLabels.Bool;
                        case InformationType.value:
                            return AxisLabels.Value;
                    }
                    break;
                case 1:
                    return AxisLabels.Y;
                case 2:
                    return AxisLabels.Z;
            }

            throw new ArgumentOutOfRangeException("Unkown Index: " + index);
        }

        /// <summary>
        /// Get the relative value of the latest read used by the bool interpreter.
        /// </summary>
        /// <param name="axis">The axis to get the value from.</param>
        /// <returns></returns>
        public float GetRelativeWorldValue(float value, AxisLabels axis)
        {
            return _calibrationValues.GetRelativeWorldValue(value, axis);
        }

        /// <summary>
        /// Resets everything related to the itnerpreter values, including the data inside <see cref="CalibrationValues"/> and <see cref="AdditiveConverter"/>
        /// </summary>
        public void ResetInterpreter()
        {
            if (Value == null) return;
            Value.Bool = false;
            Value.Value = 0f;
            Value.Position = Vector3.zero;
            Value.Rotation = Vector3.zero;

            CalibrationValues.Reset();
            if(AdditiveConverter != null)
            AdditiveConverter.Reset();
        }
    }
}
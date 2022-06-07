using System;
using System.Collections.Generic;
using System.Linq;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper
{
    /// <summary>
    /// Responsible for binding its <see cref="SingleInputGui"/> to the correct Interpreter component (<see cref="RotationInterpreter"/>, <see cref="PositionInterpreter"/>, <see cref="BooleanInterpreter"/>, <see cref="ValueInterpreter"/> or <see cref="SampleInterpreter"/> in the scene Gameobject
    /// </summary>
    [Serializable]
    public class GameObjectProperty
    {
        [SerializeField]
        private List<SingleInputMapping> _singleInputMappings;

        [SerializeField]
        private InformationType _infoType;

        /// <summary>
        /// List of <see cref="SingleInputMapping"/> used by this <see cref="GameObjectProperty"/>
        /// </summary>
        public List<SingleInputMapping> SingleInputMappings
        {
            get { return _singleInputMappings; }
            set { _singleInputMappings = value; }
        }

        /// <summary>
        /// The <see cref="InformationType"/>. Used to map its <see cref="Target"/> gameobject
        /// </summary>
        internal InformationType InfoType
        {
            get { return _infoType; }
            set { _infoType = value; }
        }

        /// <summary>
        /// The Gameobject in the scene to which this Property belongs to
        /// </summary>
        public GameObject Target { get; set; }
        /// <summary>
        /// The name of the<see cref="Target"/> object
        /// </summary>
        public string GameObjectName { get { return Target.name; } }
        
        /// <summary>
        /// If this <see cref="GameObjectProperty"/> is active in the <see cref="Target"/> gameobject
        /// </summary>
        internal bool Active { get; set; }

        /// <summary>
        /// True if all the <see cref="SingleInputMappingLabels"/> in this gameobject is none
        /// </summary>
        public bool AllInputsAreNone
        {
            get { return SingleInputMappings.All(sim => sim.InputMappingLabels == SingleInputMappingLabels.None); }
        }

        public GameObjectPropertyGui GameObjectPropertyGui { get; set; }

        public GameObjectProperty()
        {
            ResetSingleInputMappingsList();
        }

        /// <summary>
        /// Resets the <see cref="SingleInputMappings"/> list
        /// </summary>
        public void ResetSingleInputMappingsList()
        {
            SingleInputMappings =
                new List<SingleInputMapping>
                {
                    new SingleInputMapping(new SingleInput(null, ""), SingleInputMappingLabels.None)
                };

            if (InfoType == InformationType.value || InfoType == InformationType.@bool || InfoType == InformationType.sample) return;

            SingleInputMappings.Add(new SingleInputMapping(new SingleInput(null, ""), SingleInputMappingLabels.None));
            SingleInputMappings.Add(new SingleInputMapping(new SingleInput(null, ""), SingleInputMappingLabels.None));
        }

        /// <summary>
        ///  Gets the position of <see cref="Target"/> gameobject, according to the mapping performed.
        /// <para>This is the raw value before doing any calibration</para>
        /// <param name="direct">true if it is to bypass the calibration phase</param>
        /// todo apagar esta parte
        /// conforme o tipo de mapeamento que está a ser feito, retorna uma posição. Exemplo
        /// bitalino -> (curr, D0, curr)
        /// waist -> (Y, X, Z)
        /// waist -> (curr, curr, Z)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition(bool direct = false)
        {
            var siX = SingleInputMappings[0];

            if (direct)
                return siX.SingleInput.GetPosition();

            
            var siY = SingleInputMappings[1];
            var siZ = SingleInputMappings[2];

            return new Vector3(siX.GetValue(), siY.GetValue(), siZ.GetValue());
        }

        /// <summary>
        /// Gets the rotation of <see cref="Target"/> gameobject, according to the mapping performed.
        /// <para>This is the raw value before doing any calibration</para>
        /// </summary>
        /// <param name="direct">true if it is to bypass the calibration phase</param>
        /// <returns></returns>
        public Vector3 GetRotation(bool direct = false)
        {
            var siX = SingleInputMappings[0];

            if (direct)
                return siX.SingleInput.GetRotation().eulerAngles;

            var siY = SingleInputMappings[1];
            var siZ = SingleInputMappings[2];
            
            var rot = new Vector3(siX.GetValue(), siY.GetValue(), siZ.GetValue());

            return rot;
        }

        /// <summary>
        /// Gets the value, according to the mapping performed.
        /// <para>This is the raw value before doing any calibration</para>
        /// </summary>
        /// <param name="direct">true if it is to bypass the calibration phase</param>
        /// <returns></returns>
        public float GetFloat(bool direct = false)
        {
            if (direct)
                return SingleInputMappings[0].SingleInput.GetFloat();

            return SingleInputMappings[0].GetValue();
        }

        /// <summary>
        /// Gets the float value that will be used to calculate the boolean value
        /// <para>This is the raw value before doing any calibration</para>
        /// </summary>
        /// <returns></returns>
        public float GetBoolean()
        {
            return SingleInputMappings[0].GetValue();
        }
        /// <summary>
        /// Sets the index of <see cref="SingleInputMappings"/> to the received in the parameters. If the received index is zero, then adds it to the end of the list
        /// </summary>
        public void ChangeSingleInputMapping(SingleInputMapping newMapping, int index)
        {
            SingleInputMappings[index] = newMapping;
        }

        /// <summary>
        /// Binds the mapped data to the <see cref="Target"/> Interpreter component (<see cref="RotationInterpreter"/>, <see cref="PositionInterpreter"/>, <see cref="BooleanInterpreter"/>, <see cref="ValueInterpreter"/> or <see cref="SampleInterpreter"/> according to its <see cref="InfoType"/>)
        /// </summary>
        public void StartMapping()
        {
            switch (InfoType)
            {
                case InformationType.rotation:
                    Target.GetComponent<RotationInterpreter>().BindToInput(this);
                    break;
                case InformationType.position:
                    Target.GetComponent<PositionInterpreter>().BindToInput(this);
                    break;
                case InformationType.@bool:
                    Target.GetComponent<BooleanInterpreter>().BindToInput(this);
                    break;
                case InformationType.value:
                    Target.GetComponent<ValueInterpreter>().BindToInput(this);
                    break;
                case InformationType.sample:
                    Target.GetComponent<SampleInterpreter>().BindToInput(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Stops binding the mapped data to the <see cref="Target"/> Interpreter component (<see cref="RotationInterpreter"/>, <see cref="PositionInterpreter"/>, <see cref="BooleanInterpreter"/>, <see cref="ValueInterpreter"/> or <see cref="SampleInterpreter"/> according to its <see cref="InfoType"/>)
        /// <para>Puts the <see cref="SingleInputMappings"/> as null.</para>
        /// </summary>
        public void StopMapping()
        {
            ResetSingleInputMappingsList();

            if (Target != null)
            {
                var interpreter = GetInterpreter();
                if(interpreter != null)
                    interpreter.StopBind();
            }
        }

        /// <summary>
        /// Returns the <see cref="SingleInput"/> at the index received in the parameters
        /// </summary>
        /// <param name="index"> The index of the <see cref="SingleInput"/></param>
        public SingleInput GetSingleInput(int index)
        {
            return SingleInputMappings[index].SingleInput;
        }

        /// <summary>
        /// Gets the sample, according to the mapping performed.
        /// <para>This is the raw value before doing any calibration</para>
        /// </summary>
        /// <param name="direct">true if it is to bypass the calibration phase</param>
        /// <returns></returns>
        public List<float> GetSample()
        {
            return SingleInputMappings[0].SingleInput.GetSample();
        }
        
        /// <summary>
        /// Updates the <see cref="SingleInputMappings"/> according to its <see cref="InfoType"/> 
        /// </summary>
        /// <param name="singleinput">The <see cref="SingleInputGui"/> to map</param>
        public void UpdateSingleInputMappingsList(SingleInput singleinput)
        {
            switch (InfoType)
            {
                case InformationType.rotation:
                    UpdateSpecificSingleInputMappingRotation(singleinput, AxisLabels.X);
                    UpdateSpecificSingleInputMappingRotation(singleinput, AxisLabels.Y);
                    UpdateSpecificSingleInputMappingRotation(singleinput, AxisLabels.Z);
                    break;
                case InformationType.position:
                    UpdateSpecificSingleInputMappingPosition(singleinput, AxisLabels.X);
                    UpdateSpecificSingleInputMappingPosition(singleinput, AxisLabels.Y);
                    UpdateSpecificSingleInputMappingPosition(singleinput, AxisLabels.Z);
                    break;
                case InformationType.@bool:
                    ChangeSingleInputMapping(new SingleInputMapping(singleinput, SingleInputMappingLabels.Bool), 0);
                    break;
                case InformationType.value:
                    //case InformationType.signal:
                    ChangeSingleInputMapping(new SingleInputMapping(singleinput, SingleInputMappingLabels.Value), 0);
                    break;
                case InformationType.sample:
                    ChangeSingleInputMapping(new SingleInputMapping(singleinput, SingleInputMappingLabels.Sample), 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Adds a <see cref="SingleInputMapping"/> to the <see cref="SingleInputMappings"/> list using the <see cref="axis"/> to know which Position <see cref="SingleInputMappingLabels"/> to add
        /// </summary>
        /// <param name="singleInput">The <see cref="SingleInputGui"/> to map</param>
        /// <param name="axis">The <see cref="AxisLabels"/> to add to</param>
        public void UpdateSpecificSingleInputMappingPosition(SingleInput singleInput, AxisLabels axis)
        {
            switch (axis)
            {
                case AxisLabels.X:
                    ChangeSingleInputMapping(new SingleInputMapping(singleInput, SingleInputMappingLabels.PosX), 0);
                    break;
                case AxisLabels.Y:
                    ChangeSingleInputMapping(new SingleInputMapping(singleInput, SingleInputMappingLabels.PosY), 1);
                    break;
                case AxisLabels.Z:
                    ChangeSingleInputMapping(new SingleInputMapping(singleInput, SingleInputMappingLabels.PosZ), 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("axis", axis, null);
            }
        }

        /// <summary>
        /// Adds a <see cref="SingleInputMapping"/> to the <see cref="SingleInputMappings"/> list using the <see cref="axis"/> to know which Rotation <see cref="SingleInputMappingLabels"/> to add
        /// </summary>
        /// <param name="singleInput">The <see cref="SingleInputGui"/> to map</param>
        /// <param name="axis">The <see cref="AxisLabels"/> to add to</param>
        public void UpdateSpecificSingleInputMappingRotation(SingleInput singleInput, AxisLabels axis)
        {
            switch (axis)
            {
                case AxisLabels.X:
                    ChangeSingleInputMapping(new SingleInputMapping(singleInput, SingleInputMappingLabels.RotX), 0);
                    break;
                case AxisLabels.Y:
                    ChangeSingleInputMapping(new SingleInputMapping(singleInput, SingleInputMappingLabels.RotY), 1);
                    break;
                case AxisLabels.Z:
                    ChangeSingleInputMapping(new SingleInputMapping(singleInput, SingleInputMappingLabels.RotZ), 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("axis", axis, null);
            }
        }

        /// <summary>
        /// Returns the target <see cref="PositionInterpreter"/>
        /// </summary>
        public PositionInterpreter GetPosInterpreter()
        {
            return Target.GetComponent<PositionInterpreter>();
        }

        /// <summary>
        /// Returns the target <see cref="RotationInterpreter"/>
        /// </summary>
        public RotationInterpreter GetRotInterpreter()
        {
            return Target.GetComponent<RotationInterpreter>();
        }

        /// <summary>
        /// Returns the target <see cref="BooleanInterpreter"/>
        /// </summary>
        /// <returns></returns>
        public BooleanInterpreter GetBoolInterpreter()
        {
            return Target.GetComponent<BooleanInterpreter>();
        }

        /// <summary>
        /// Returns the target <see cref="ValueInterpreter"/>
        /// </summary>
        /// <returns></returns>
        public ValueInterpreter GetValueInterpreter()
        {
            return Target.GetComponent<ValueInterpreter>();
        }

        public SampleInterpreter GetSampleInterpreter()
        {
            return Target.GetComponent<SampleInterpreter>();
        }

        /// <summary>
        /// Returns the correct <see cref="Interpreter"/>, according to the <see cref="InfoType"/> of this <see cref="GameObjectProperty"/>
        /// </summary>
        /// <returns></returns>
        public Interpreter GetInterpreter()
        {
            switch (InfoType)
            {
                case InformationType.rotation:
                    return GetRotInterpreter();
                case InformationType.position:
                    return GetPosInterpreter();
                case InformationType.@bool:
                    return GetBoolInterpreter();
                case InformationType.value:
                    return GetValueInterpreter();
                case InformationType.sample:
                    return GetSampleInterpreter();
            }

            Debug.LogError("Theorically unreachable code. Check the logic above.");
            return null;
        }

        /// <summary>
        /// Returns true if the received <see cref="AxisLabels"/> is mapped to <see cref="SingleInputMappingLabels.None"/>
        /// </summary>
        public bool IsSingleInputMappingLabelNone(AxisLabels axisLabels)
        {
            switch (axisLabels)
            {
                case AxisLabels.Bool:
                case AxisLabels.Value:
                case AxisLabels.X:
                    return SingleInputMappings[0].InputMappingLabels == SingleInputMappingLabels.None;
                case AxisLabels.Y:
                    return SingleInputMappings[1].InputMappingLabels == SingleInputMappingLabels.None;
                case AxisLabels.Z:
                    return SingleInputMappings[2].InputMappingLabels == SingleInputMappingLabels.None;
                default:
                    throw new ArgumentOutOfRangeException("axisLabels", axisLabels, null);
            }
        }

        /// <summary>
        /// Overrides the default ToString() method. returns the <see cref="InfoType"/> string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InfoType.ToString();
        }
    }
}
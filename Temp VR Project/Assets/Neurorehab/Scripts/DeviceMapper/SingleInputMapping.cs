using System;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper
{
    /// <summary>
    /// Class containing information about where is the information relevant for hte mapping of this single input
    /// </summary>
    [Serializable]
    public class SingleInputMapping
    {
        /// <summary>
        /// The <see cref="SingleInput"/> being used by this mappin g
        /// </summary>
        [SerializeField]
        private SingleInput _singleInput;
        /// <summary>
        /// The <see cref="SingleInputMappingLabels"/> where this <see cref="SingleInput"/> is being used
        /// </summary>
        [SerializeField]
        private SingleInputMappingLabels _inputMappingLabels;

        /// <summary>
        /// The <see cref="SingleInput"/> being used by this mappin g
        /// </summary>
        public SingleInput SingleInput
        {
            get { return _singleInput; }
            set { _singleInput = value; }
        }

        /// <summary>
        /// The <see cref="SingleInputMappingLabels"/> where this <see cref="SingleInput"/> is being used
        /// </summary>
        public SingleInputMappingLabels InputMappingLabels
        {
            get { return _inputMappingLabels; }
            set { _inputMappingLabels = value; }
        }

        public SingleInputMapping(SingleInput singleInput, SingleInputMappingLabels inputMappingLabels)
        {
            SingleInput = singleInput;
            InputMappingLabels = inputMappingLabels;
        }

        /// <summary>
        /// Returns the remapped coordinate, acoording to the <see cref="Gui.SingleInputGui"/> bound to this property.
        /// </summary>
        /// <param name="coordinate"> The coordinate we want to remap.</param>
        /// <returns> The value of the remapped coordinate.</returns>
        public float GetValue()
        {
            switch (InputMappingLabels)
            {
                case SingleInputMappingLabels.None:
                    return 0f;
                case SingleInputMappingLabels.PosX:
                    return SingleInput.GetPosition().x;
                case SingleInputMappingLabels.PosY:
                    return SingleInput.GetPosition().y;
                case SingleInputMappingLabels.PosZ:
                    return SingleInput.GetPosition().z;
                case SingleInputMappingLabels.RotX:
                    return (SingleInput.GetRotation().eulerAngles.x + 3600) % 360;
                case SingleInputMappingLabels.RotY:
                    return (SingleInput.GetRotation().eulerAngles.y + 3600) % 360;
                case SingleInputMappingLabels.RotZ:
                    return (SingleInput.GetRotation().eulerAngles.z + 3600) % 360;
                case SingleInputMappingLabels.Value:
                    return SingleInput.GetFloat();
                case SingleInputMappingLabels.Bool:
                    return SingleInput.GetBoolean() ? float.MaxValue : float.MinValue;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns the value that will be used to define the boolean.
        /// </summary>
        //public float GetBoolean()
        //{
        //    switch (InputMappingLabels)
        //    {
        //        case SingleInputMappingLabels.None:
        //            return float.MinValue;
        //        case SingleInputMappingLabels.PosX:
        //            return SingleInput.GetPosition().x;
        //        case SingleInputMappingLabels.PosY:
        //            return SingleInput.GetPosition().y;
        //        case SingleInputMappingLabels.PosZ:
        //            return SingleInput.GetPosition().z;
        //        case SingleInputMappingLabels.RotX:
        //            return (SingleInput.GetRotation().eulerAngles.x + 3600) % 360;
        //        case SingleInputMappingLabels.RotY:
        //            return (SingleInput.GetRotation().eulerAngles.y + 3600) % 360;
        //        case SingleInputMappingLabels.RotZ:
        //            return (SingleInput.GetRotation().eulerAngles.z + 3600) % 360;
        //        case SingleInputMappingLabels.Value:
        //            return SingleInput.GetFloat();
        //        case SingleInputMappingLabels.Bool:
        //            return SingleInput.GetBoolean() ? float.MaxValue : float.MinValue;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}
    }
}
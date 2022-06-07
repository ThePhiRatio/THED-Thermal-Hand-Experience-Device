using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator.Data
{
    /// <summary>
    /// Class used by the converters to pass information. Contains all possible conversions.
    /// </summary>
    [Serializable]
    public class MapperValue
    {
        /// <summary>
        /// The last converted rotation
        /// </summary>
        [SerializeField]
        private Vector3 _rotation;
        /// <summary>
        /// The last converted position
        /// </summary>
        [SerializeField]
        private Vector3 _position;
        /// <summary>
        /// The last converted value
        /// </summary>
        [SerializeField]
        private float _value;
        /// <summary>
        /// The last converted bool
        /// </summary>
        [SerializeField]
        private bool _bool;

        /// <summary>
        /// The last converted samples
        /// </summary>
        [SerializeField]
        private List<float> _sample;


        /// <summary>
        /// The last converted samples
        /// </summary>
        public List<float> Sample
        {
            get { return _sample; }
            set { _sample = value; }
        }

        /// <summary>
        /// The last converted rotation
        /// </summary>
        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// The last converted position
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// The last converted value
        /// </summary>
        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The last converted bool
        /// </summary>
        public bool Bool
        {
            get { return _bool; }
            set { _bool = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MapperValue() { /* intensionally empty */}

        /// <summary>
        /// 'Copy' constructor. Instantiates a new <see cref="MapperValue"/> with the values of the variable received as parameter.
        /// </summary>
        /// <param name="value"></param>
        public MapperValue(MapperValue value)
        {
            Rotation = new Vector3(value.Rotation.x, value.Rotation.y, value.Rotation.z);
            Position = new Vector3(value.Position.x, value.Position.y, value.Position.z);
            Value = value.Value;
            Bool = value.Bool;
        }
    }
}
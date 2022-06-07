using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Calibrator
{
    /// <summary>
    /// Used for calibrate RehaPanel devices. Exists one for each mapping made.
    /// </summary>
    public class Calibrator : MonoBehaviour
    {
        /// <summary>
        /// Used for debug. Visual representation of the max vector3
        /// </summary>
        public Transform MaxCube;
        /// <summary>
        /// Used for debig. Visual representation of the min vector3
        /// </summary>
        public Transform MinCube;

        /// <summary>
        /// The cube that limits the play area
        /// </summary>
        [Header("Border")]
        [Tooltip("The cube that limits the play area")]
        public Collider Borders;
        /// <summary>
        /// Max value for X
        /// </summary>
        [Space]
        [Header("Position")]
        [Tooltip("Max value for X")]
        public float XMax;
        /// <summary>
        /// Max value for Y
        /// </summary>
        [Tooltip("Max value for Y")]
        public float YMax;
        /// <summary>
        /// Max value for Z
        /// </summary>
        [Tooltip("Max value for Z")]
        public float ZMax;
        /// <summary>
        /// Min value for X
        /// </summary>
        [Tooltip("Min value for X")]
        public float XMin;
        /// <summary>
        /// Min value for Y
        /// </summary>
        [Tooltip("Min value for Y")]
        public float YMin;
        /// <summary>
        /// Min value for Z
        /// </summary>
        [Tooltip("Min value for Z")]
        public float ZMin;

        /// <summary>
        /// Max value for X rotation
        /// </summary>
        [Space]
        [Header("Rotation")]
        [Tooltip("Max value for X rotation")]
        public float XMaxRotation;
        /// <summary>
        /// Max value for Y rotation
        /// </summary>
        [Tooltip("Max value for Y rotation")]
        public float YMaxRotation;
        /// <summary>
        /// Max value for Z rotation
        /// </summary>
        [Tooltip("Max value for Z rotation")]
        public float ZMaxRotation;
        /// <summary>
        /// Min value for X rotation
        /// </summary>
        [Tooltip("Min value for X rotation")]
        public float XMinRotation;
        /// <summary>
        /// Min value for Y rotation
        /// </summary>
        [Tooltip("Min value for Y rotation")]
        public float YMinRotation;
        /// <summary>
        /// Min value for Z rotation
        /// </summary>
        [Tooltip("Min value for Z rotation")]
        public float ZMinRotation;

        private void Awake()
        {
            XMax = Borders.bounds.max.x;
            YMax = Borders.bounds.max.y;
            ZMax = Borders.bounds.max.z;
            XMin = Borders.bounds.min.x;
            YMin = Borders.bounds.min.y;
            ZMin = Borders.bounds.min.z;
        }


        public void Update()
        {
            MaxCube.position = Borders.bounds.max;
            MinCube.position = Borders.bounds.min;
        }
    }
}
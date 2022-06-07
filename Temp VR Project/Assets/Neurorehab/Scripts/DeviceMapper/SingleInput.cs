using System.Collections.Generic;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Enums;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper
{
    public class SingleInput
    {
        /// <summary>
        /// Holds the name of the <see cref="GenericDeviceData"/>, to allow access to it even if the <see cref="GenericDeviceData"/> is null.
        /// </summary>
        public string GenericDeviceDataName { get; set; }

        /// <summary>
        /// The espective <see cref="GenericDeviceData"/> where it gets its data from
        /// </summary>
        public GenericDeviceData GenericDeviceData
        {
            get { return _genericDeviceData; }
            set
            {
                if(value != null)
                    GenericDeviceDataName = value.DeviceName;

                _genericDeviceData = value;
            }
        }

        /// <summary>
        /// The list of <see cref="InformationType"/> that can be mapped to
        /// </summary>
        public List<InformationType> InfoTypes { get; set; }

        /// <summary>
        /// Reference to the <see cref="SingleInputGui"/> that represents this <see cref="SingleInput"/>
        /// </summary>
        public SingleInputGui SingleInputGui { get; set; }

        /// <summary>
        /// The key of the specific entry in the <see cref="GenericDeviceData"/> Dictionaries.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;

                if(SingleInputGui != null)
                    SingleInputGui.Label = value;
            }
        }

        /// <summary>
        /// Backfield of the <see cref="Activated"/> property
        /// </summary>
        private bool _activated;

        private string _label = "";
        private GenericDeviceData _genericDeviceData;

        /// <summary>
        /// If the <see cref="SingleInput"/> has any connections or not. Used in the GUI
        /// </summary>
        public bool Activated
        {
            get { return _activated; }
            set
            {
                _activated = value;
                SingleInputGui.UpdateActivation(_activated);
            }
        }

        public SingleInput(SingleInputGui singleInputGui, string label)
        {
            InfoTypes = new List<InformationType>();
            SingleInputGui = singleInputGui;
            Label = label;
        }

        /// <summary>
        /// Fills InfoTypes list with all the information types available for the specific singleInput
        /// </summary>
        /// <returns></returns>
        public void UpdateInformationTypes()
        {
            InfoTypes.Clear();

            if (GenericDeviceData.ContainsPosition(Label))
                InfoTypes.Add(InformationType.position);
            if (GenericDeviceData.ContainsRotation(Label))
                InfoTypes.Add(InformationType.rotation);
            if (GenericDeviceData.ContainsFloat(Label))
                InfoTypes.Add(InformationType.value);
            if (GenericDeviceData.ContainsBoolean(Label))
                InfoTypes.Add(InformationType.@bool);
            if (GenericDeviceData.ContainsSample(Label))
                InfoTypes.Add(InformationType.sample);

            //if(InfoTypes.Count == 0)
            //    InfoTypes.Add(InformationType.unknown);


        }

        /// <summary>
        /// Gets the rotation data from its current <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> using the <see cref="Label"/> as key.
        /// </summary>
        /// <returns>If <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> is null, returns a Quaternion.identity, else returns the data from the <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> Rotation dictionary</returns>
        public Quaternion GetRotation()
        {
            if (GenericDeviceData == null) return Quaternion.identity;
            return GenericDeviceData.GetRotation(Label);
        }

        /// <summary>
        /// Gets the position data from its current <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> using the <see cref="Label"/> as key.
        /// </summary>
        /// <returns>If <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> is null, returns a Vector3.zero, else returns the data from the <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> Position dictionary</returns>
        public Vector3 GetPosition()
        {
            if (GenericDeviceData == null) return Vector3.zero;
            return GenericDeviceData.GetPosition(Label);
        }

        /// <summary>
        /// Gets the value data from its current <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> using the <see cref="Label"/> as key.
        /// </summary>
        /// <returns>If <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> is null, returns 0, else returns the data from the <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> Float dictionary</returns>
        public float GetFloat()
        {
            if (GenericDeviceData == null) return 0f;
            return GenericDeviceData.GetFloat(Label);
        }

        /// <summary>
        /// Gets the sample data from its current <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> using the <see cref="Label"/> as key.
        /// </summary>
        /// <returns>If <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> is null, returns a empty list of floats, else returns the data from the <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> Sample dictionary</returns>
        public List<float> GetSample()
        {
            if (GenericDeviceData == null) return new List<float>();
            return GenericDeviceData.GetSample(Label);
        }

        /// <summary>
        /// Gets the boolean data from its current <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> using the <see cref="Label"/> as key.
        /// </summary>
        /// <returns>If <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> is null, returns false, else return the data from the <see cref="Scripts.Devices.Abstracts.GenericDeviceData"/> Boolean dictionary</returns>
        public bool GetBoolean()
        {
            if (GenericDeviceData == null) return false;
            return GenericDeviceData.GetBoolean(Label);
        }
        /// <summary>
        /// Checks if it is receiving this <see cref="Label"/> by UDP
        /// </summary>
        /// <returns></returns>
        public bool Connected()
        {
            if (GenericDeviceData == null) return false;

            var isConnected = false;
            foreach (var infoType in InfoTypes)
            {
                switch (infoType)
                {
                    case InformationType.rotation:
                        isConnected |= GenericDeviceData.ContainsRotation(Label);
                        break;
                    case InformationType.position:
                        isConnected |= GenericDeviceData.ContainsPosition(Label);
                        break;
                    case InformationType.@bool:
                        isConnected |= GenericDeviceData.ContainsBoolean(Label);
                        break;
                    case InformationType.value:
                        isConnected |= GenericDeviceData.ContainsFloat(Label);
                        break;
                    //case InformationType.signal:
                    //    isConnected |= GenericDeviceData.ContainsFloat(Label);
                    //    break;
                    case InformationType.sample:
                        isConnected |= GenericDeviceData.ContainsSample(Label);
                        break;
                }
                if (isConnected) break;
            }
            return isConnected;
        }

        /// <summary>
        /// Overrides the default ToString() Method. Returns the <see cref="Label"/> string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Label;
        }
    }
}
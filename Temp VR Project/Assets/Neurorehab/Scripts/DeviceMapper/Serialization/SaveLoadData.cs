using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neurorehab.Scripts.DeviceMapper.Calibrator;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Converters;
using Neurorehab.Scripts.DeviceMapper.Calibrator.Data;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.Scripts.DeviceMapper.Interpreters;
using Neurorehab.Scripts.Devices.Abstracts;
using Neurorehab.Scripts.Enums;
using Neurorehab.Scripts.Udp;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Serialization
{
    /// <summary>
    /// Manages the save and load process
    /// </summary>
    public static class SaveLoadData
    {
        public static List<SingleInput> AllSingleInputs = new List<SingleInput>();

        private static CalibrationSaveData _lastSerializedData;

        /// <summary>
        /// Converts the data to json and stores it into a file on the specified path. 
        /// </summary>
        /// <param name="path">The file complete path</param>
        public static void SaveData(string path)
        {
            if (_lastSerializedData.MappedObjects.Count == 0) return;

            PlayerPrefs.SetString("LastSave_" + SceneManager.GetActiveScene().name, path);
            
            var json = JsonUtility.ToJson(_lastSerializedData);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Serializes the calibration data for the save process
        /// </summary>
        public static void SerializeData()
        {
            _lastSerializedData = new CalibrationSaveData();

            var objectsAvailableToMap = GameObject.FindGameObjectsWithTag(Tags.AvailableToMap.ToString());
            _lastSerializedData.MappedObjects = new List<MappedObject>();

            foreach (var availableToMap in objectsAvailableToMap)
            {
                var mappedObject = new MappedObject
                {
                    Name = availableToMap.name
                };
                var interpreters = availableToMap.GetComponents<Interpreter>();

                mappedObject.Gops = new List<SerializableGop>();

                foreach (var interpreter in interpreters)
                {
                    var serializableGop = new SerializableGop
                    {
                        InformationType = interpreter.GameObjectProperty.InfoType,
                        GenericDeviceDataLabel = new List<string>(),
                        SingleInputLabels = new List<string>(),
                        SingleInputMappingLabels = new List<SingleInputMappingLabels>(),
                        StringValuesSamples = new List<StringValues>()
                    };


                    foreach (var gopSingleInputMapping in interpreter.GameObjectProperty.SingleInputMappings)
                    {
                        if (gopSingleInputMapping.SingleInput.GenericDeviceData == null) continue;

                        serializableGop.GenericDeviceDataLabel.Add(gopSingleInputMapping.SingleInput.GenericDeviceData
                            .DeviceName);
                        serializableGop.StringValuesSamples.Add(gopSingleInputMapping.SingleInput.GenericDeviceData
                            .StringValuesSample);
                        serializableGop.SingleInputLabels.Add(gopSingleInputMapping.SingleInput.Label);
                        serializableGop.SingleInputMappingLabels.Add(gopSingleInputMapping.InputMappingLabels);
                    }

                    serializableGop.SerializableInterpreter = new SerializableInterpreter
                    {
                        Mode = interpreter.Mode,
                        UseOnThisObject = interpreter.UseOnThisObject,
                        InputKeys = interpreter.CalibrationValues.InputData.Keys.ToList(),
                        InputValues = interpreter.CalibrationValues.InputData.Values.ToList(),
                        OutputKeys = interpreter.CalibrationValues.OutputData.Keys.ToList(),
                        OutputValues = interpreter.CalibrationValues.OutputData.Values.ToList()
                    };

                    if (serializableGop.InformationType != InformationType.@bool &&
                        serializableGop.InformationType != InformationType.sample)
                    {
                        var additiveConverter = interpreter.GetComponent<IAdditiveConverter>();

                        serializableGop.SerializableInterpreter.SpeedKeys = additiveConverter.MaxSpeed.Keys.ToList();
                        serializableGop.SerializableInterpreter.SpeedValues =
                            additiveConverter.MaxSpeed.Values.ToList();
                        serializableGop.SerializableInterpreter.ToleranceKeys =
                            additiveConverter.Tolerance.Keys.ToList();
                        serializableGop.SerializableInterpreter.ToleranceValues =
                            additiveConverter.Tolerance.Values.ToList();
                    }

                    if (serializableGop.GenericDeviceDataLabel.Count > 0)
                        mappedObject.Gops.Add(serializableGop);
                }

                if (mappedObject.Gops.Count > 0)
                    _lastSerializedData.MappedObjects.Add(mappedObject);
            }
        }

        /// <summary>
        /// Loads the data from a json file, in the specified path, and converts it to the respectives data structures
        /// </summary>
        /// <param name="path">The file complete path</param>
        public static void LoadData(string path)
        {
            PlayerPrefs.SetString("LastSave_" + SceneManager.GetActiveScene().name, path);
            var loadedData = JsonUtility.FromJson<CalibrationSaveData>(File.ReadAllText(path));

            var sceneMappedObjects = DeviceMapperGuiManager.Instance.AvailableGameobjectsPanel.transform;
            
            foreach (var mappedObject in loadedData.MappedObjects)
            {
                var goiObject = sceneMappedObjects.Find(mappedObject.Name + "GOI");
                if (goiObject == null) continue;

                var gameObjectInformation = goiObject.GetComponent<GameObjectInformation>();

                gameObjectInformation.GetComponent<Toggle>().isOn = true;

                foreach (var gop in gameObjectInformation.ObjectProperties.Where(gop => gop.Active))
                    DeviceMapperGuiManager.Instance.RemoveGameobjectProperty(gameObjectInformation, gop.InfoType);
                
                foreach (var serializableGop in mappedObject.Gops)
                {
                    var gop = gameObjectInformation.ObjectProperties.First(
                        GoP => GoP.InfoType == serializableGop.InformationType);
                    DeviceMapperGuiManager.Instance.AddGameobjectProperty(gameObjectInformation, serializableGop.InformationType);

                    gop.StartMapping();
                    var interpreter = gop.GetInterpreter();

                    for (var i = 0; i < serializableGop.GenericDeviceDataLabel.Count; i++)
                    {
                        var genericDeviceDataLabel = serializableGop.GenericDeviceDataLabel[i];
                        var singleInputLabel = serializableGop.SingleInputLabels[i];
                        var singleInputMappingLabel = serializableGop.SingleInputMappingLabels[i];
                        var stringValues = serializableGop.StringValuesSamples[i];
                        
                        if (GenericDeviceGuiManager.Instance.GetAlreadyShowingDevices().Contains(genericDeviceDataLabel) == false)
                        {
                            var gdd = DevicesController.Instance.CreateGenericDeviceData(genericDeviceDataLabel, stringValues); ;
                            GenericDeviceGuiManager.Instance.PopulateDeviceGui(gdd);
                        }

                        var pm = GenericDeviceGuiManager.Instance.AllPrefabManagers.First(gpm => gpm.GenericDeviceName == genericDeviceDataLabel);
                        
                        if (AllSingleInputs.Any(si => si.Label == singleInputLabel) == false)
                        {
                            pm.AddNewSingleInput(singleInputLabel);
                            pm.UpdateConnections();
                        }

                        var singleInput = AllSingleInputs.First(si => si.GenericDeviceDataName == genericDeviceDataLabel && si.Label == singleInputLabel);

                        MapperManager.ItemBeingDragged = singleInput.SingleInputGui;
                        MapperManager.ItemBeingDragged = singleInput.SingleInputGui;
                        MapperManager.TriggerMiniGameObjectPropertyActivation(gop.GameObjectPropertyGui, interpreter.GetAxisLabel(i));
                        gop.SingleInputMappings[i].InputMappingLabels = singleInputMappingLabel;

                        //MessageManager.Instance.ShowErrorMessage("You are not receiving information from " + genericDeviceDataLabel + " " + singleInputLabel + ". This information was being used on: " + gop + " " + interpreter.GetAxisLabel(i));
                    }
                    
                    for (var i = 0; i < serializableGop.SerializableInterpreter.InputKeys.Count; i++)
                    {
                        var inputKey = serializableGop.SerializableInterpreter.InputKeys[i];
                        var inputValue = serializableGop.SerializableInterpreter.InputValues[i];
                        var outputKey = serializableGop.SerializableInterpreter.OutputKeys[i];
                        var outputValue = serializableGop.SerializableInterpreter.OutputValues[i];

                        interpreter.CalibrationValues.AddInputValue(inputKey, inputValue);
                        interpreter.CalibrationValues.AddOutputValue(outputKey, outputValue);
                        interpreter.Mode = serializableGop.SerializableInterpreter.Mode;
                        interpreter.UseOnThisObject = serializableGop.SerializableInterpreter.UseOnThisObject;
                        
                        if (interpreter.AdditiveConverter == null) continue;

                        var speedKeys = serializableGop.SerializableInterpreter.SpeedKeys[i];
                        var speedValues = serializableGop.SerializableInterpreter.SpeedValues[i];
                        var toleranceKeys = serializableGop.SerializableInterpreter.ToleranceKeys[i];
                        var toleranceValues = serializableGop.SerializableInterpreter.ToleranceValues[i];
                        
                        interpreter.AdditiveConverter.AddSpeed(speedKeys, speedValues);
                        interpreter.AdditiveConverter.AddTolerance(toleranceKeys, toleranceValues);
                    }
                }
            }
        }

        /// <summary>
        /// Serializable data structure used for the save and load algorithm. This is the root structure.
        /// </summary>
        [Serializable]
        private struct CalibrationSaveData
        {
            /// <summary>
            /// List containing all the mapped objects on the scene. Only objects with active calibrations will be added to this list.
            /// </summary>
            public List<MappedObject> MappedObjects;
        }

        /// <summary>
        /// Serializable data structure used for the save and load algorithm.
        /// </summary>
        [Serializable]
        private struct MappedObject
        {
            /// <summary>
            /// The name of the GameObject
            /// </summary>
            public string Name;
            /// <summary>
            /// All the <see cref="SerializableGop"/> existing in this object
            /// </summary>
            public List<SerializableGop> Gops;
        }

        /// <summary>
        /// Serializable data structure used for the save and load algorithm. 
        /// </summary>
        [Serializable]
        private struct SerializableGop
        {
            /// <summary>
            /// A list of all the labels identifying the <see cref="GenericDeviceData"/> where the information comes from. This informations must be paired wihth the <see cref="SingleInputLabels"/> and <see cref="SingleInputMappingLabels"/> lists
            /// </summary>
            public List<string> GenericDeviceDataLabel;
            /// <summary>
            /// A List of <see cref="StringValues"/> samples received by this <see cref="GameObjectProperty"/>. Used for proper initialization. One sample for each device in <see cref="GenericDeviceDataLabel"/>
            /// </summary>
            public List<StringValues> StringValuesSamples;
            /// <summary>
            /// A list of all the labels identifying the <see cref="SingleInput"/> where the information comes from. This informations must be paired wihth the <see cref="GenericDeviceDataLabel"/> and <see cref="SingleInputMappingLabels"/> lists
            /// </summary>
            public List<string> SingleInputLabels;
            /// <summary>
            /// A list of all the labels identifying the <see cref="Enums.SingleInputMappingLabels"/> where the information comes from. This informations must be paired wihth the <see cref="GenericDeviceDataLabel"/> and <see cref="SingleInputLabels"/> lists
            /// </summary>
            public List<SingleInputMappingLabels> SingleInputMappingLabels;
            /// <summary>
            /// The type of information being saved in this <see cref="GameObjectProperty"/>
            /// </summary>
            public InformationType InformationType;
            
            /// <summary>
            /// The <see cref="Interpreter"/> used by this <see cref="GameObjectProperty"/>
            /// </summary>
            public SerializableInterpreter SerializableInterpreter;
        }

        /// <summary>
        /// Serializable data structure used for the save and load algorithm. 
        /// </summary>
        [Serializable]
        private struct SerializableInterpreter
        {
            /// <summary>
            /// Indicates if the values of this <see cref="Interpreter"/> should be applyed to the GameObject where it is assossiated
            /// </summary>
            public bool UseOnThisObject;
            /// <summary>
            /// the calibration mode of this <see cref="Interpreter"/>
            /// </summary>
            public CalibrationMode Mode;

            /// <summary>
            /// The keys used to reconstruct the <see cref="CalibrationValues.InputData"/> dictionary.
            /// </summary>
            public List<AxisLabels> InputKeys;
            /// <summary>
            /// The values used to reconstruct the <see cref="CalibrationValues.InputData"/> dictionary.
            /// </summary>
            public List<Data> InputValues;

            /// <summary>
            /// The keys used to reconstruct the <see cref="CalibrationValues.OutputData"/> dictionary.
            /// </summary>
            public List<AxisLabels> OutputKeys;
            /// <summary>
            /// The values used to reconstruct the <see cref="CalibrationValues.OutputData"/> dictionary.
            /// </summary>
            public List<Data> OutputValues;

            /// <summary>
            /// The keys used to reconstruct the <see cref="IAdditiveConverter.MaxSpeed"/> dictionary.
            /// </summary>
            public List<AxisLabels> SpeedKeys;
            /// <summary>
            /// The values used to reconstruct the <see cref="IAdditiveConverter.MaxSpeed"/> dictionary.
            /// </summary>
            public List<float> SpeedValues;
            /// <summary>
            /// The keys used to reconstruct the <see cref="IAdditiveConverter.Tolerance"/> dictionary.
            /// </summary>
            public List<AxisLabels> ToleranceKeys;
            /// <summary>
            /// The values used to reconstruct the <see cref="IAdditiveConverter.Tolerance"/> dictionary.
            /// </summary>
            public List<float> ToleranceValues;
        }

        /// <summary>
        /// Static funtion that adds a single input to the <see cref="AllSingleInputs"/> list
        /// </summary>
        public static void AddSingleInput(SingleInput singleInput)
        {
            if(AllSingleInputs.Contains(singleInput) == false)
                AllSingleInputs.Add(singleInput);
        }
    }
}
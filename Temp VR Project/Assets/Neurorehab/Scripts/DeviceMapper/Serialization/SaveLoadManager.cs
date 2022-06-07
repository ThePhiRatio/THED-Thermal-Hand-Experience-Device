using System;
using System.IO;
using Neurorehab.Scripts.DeviceMapper.Gui;
using Neurorehab.SimpleFileBrowser.Scripts.GracesGames;
using UnityEngine;

namespace Neurorehab.Scripts.DeviceMapper.Serialization
{
    /// <summary>
    ///  Able to save and load files containing serialized data (e.g. text)
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        /// <summary>
        /// The file browser prefab
        /// </summary>
        public GameObject FileBrowserPrefab;

        /// <summary>
        /// The accpeted file extension
        /// </summary>
        public string FileExtension = "json";

        /// <summary>
        /// The latest selected path
        /// </summary>
        private string _curPath;
        /// <summary>
        /// Singleton reference
        /// </summary>
        public static SaveLoadManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (DeviceMapperGuiManager.Instance == null)
                return;
            if (DeviceMapperGuiManager.Instance.IsShowing &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetKeyDown(KeyCode.S))
            {
                OpenFileBrowser(true);
            }
        }

        /// <summary>
        /// Open the file browser using boolean parameter so it can be called in GUI
        /// </summary>
        /// <param name="saving">True if it is saving.</param>
        public void OpenFileBrowser(bool saving)
        {
            if (FindObjectOfType<FileBrowser>() != null)
                FileBrowser.Destroy();

            if(saving)
                SaveLoadData.SerializeData();

            OpenFileBrowser(saving ? FileBrowserMode.Save : FileBrowserMode.Load);
        }

        /// <summary>
        /// Open a file browser to save and load files
        /// </summary>
        /// <param name="fileBrowserMode"></param>
        public void OpenFileBrowser(FileBrowserMode fileBrowserMode)
        {
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            if (fileBrowserMode == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel(this, "SaveFileUsingPath", "ConfigurationName", FileExtension);
            }
            else
            {
                fileBrowserScript.OpenFilePanel(this, "LoadFileUsingPath", FileExtension);
            }
        }

        /// <summary>
        /// Saves a file with the textToSave using a path. A confirmation window will be displayed if the file already exists
        /// </summary>
        /// <param name="path"></param>
        private void SaveFileUsingPath(string path)
        {
            _curPath = path;
            if(File.Exists(path))
                ConfirmationActionManager.Instance.ShowConfirmationWindow(
                    SaveAfterConfirmation,
                    "OVERRIDE SAVE FILE");
            else
                SaveAfterConfirmation(true);
        }

        /// <summary>
        /// After the save action is confirmed, this function is called. By default, nothing is done, but anything can be added here.
        /// </summary>
        /// <param name="confirmation"></param>
        private void SaveAfterConfirmation(bool confirmation)
        {
            if (confirmation == false)
            {
                // add any action here.
                return;
            }

            if (!String.IsNullOrEmpty(_curPath))
            {
                SaveLoadData.SaveData(_curPath);
            }
            else
            {
                Debug.Log("Invalid path or empty file given");
            }
        }

        /// <summary>
        /// Loads a file using a path
        /// </summary>
        /// <param name="path"></param>
        public void LoadFileUsingPath(string path)
        {
            if (path.Length != 0 && File.Exists(path))
            {
                #if UNITY_EDITOR
                Debug.Log(path);
                #endif 
                SaveLoadData.LoadData(path);
            }
            else
            {
                #if UNITY_EDITOR
                Debug.Log("Invalid path given");
                #endif 
            }
        }
    }
}

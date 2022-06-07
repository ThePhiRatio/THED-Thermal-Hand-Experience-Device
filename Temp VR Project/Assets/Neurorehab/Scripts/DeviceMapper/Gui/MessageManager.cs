using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for managing all the Gui messages (shows and hides the message)
    /// </summary>
    public class MessageManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton reference
        /// </summary>
        public static MessageManager Instance { get; set; }

        /// <summary>
        /// MessagePanel Gameobject
        /// </summary>
        public GameObject MessagesPanel;
        /// <summary>
        /// Modal Panel Gameobject
        /// </summary>
        public GameObject ModalPanel;
        /// <summary>
        /// The Message Panel Gameobject
        /// </summary>
        public GameObject MessagePopup;
        
        /// <summary>
        /// The Text Gameobject where the message in the <see cref="MessagePopup"/> is going to be written on
        /// </summary>
        public Text MessagePopupText;
        /// <summary>
        /// The Bottom Message Panel Gameobject
        /// </summary>
        public GameObject BottomMessagePanel;

        /// <summary>
        /// The Text Gameobject where the message in the <see cref="BottomMessagePanel"/>is going to be written on
        /// </summary>
        public Text BottomMessageText;

        /// <summary>
        /// The console window
        /// </summary>
        public GameObject ConsoleWindow;
        /// <summary>
        /// the console whole text
        /// </summary>
        public Text ConsoleText;
        public ScrollRect ConsoleWindowScroll;

        private string _consoleMessages;

        public string ConsoleMessages
        {
            get { return _consoleMessages; }
            set
            {
                _consoleMessages = value;
                ConsoleText.text = value;
            }
        }

        void Awake ()
        {
            Instance = this;

            MessagesPanel.SetActive(false);
            ModalPanel.SetActive(false);
            MessagePopup.SetActive(false);
            BottomMessagePanel.SetActive(false);
            ConsoleWindow.SetActive(false);
            ConsoleText.text = "";
        }

        /// <summary>
        /// Shows the message panel
        /// </summary>
        /// <param name="messageToShow">The message to show</param>
        public void ShowPopupMessage(string messageToShow)
        {
            MessagesPanel.SetActive(true);
            ModalPanel.SetActive(true);
            MessagePopupText.text = messageToShow;
            MessagePopup.SetActive(true);
            StartCoroutine(ShowPopupMessage());
        }

        /// <summary>
        /// Starts the corroutine to wait 3 seconds before hiding the message
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowPopupMessage()
        {
            yield return new WaitForSecondsRealtime(3f);
            MessagePopup.SetActive(false);

            if (ConsoleWindow.activeSelf) yield break;
            MessagesPanel.SetActive(false);
            ModalPanel.SetActive(false);
        }


        /// <summary>
        /// Shows the message panel
        /// </summary>
        /// <param name="messageToShow">The message to show</param>
        public void ShowErrorMessage(string messageToShow)
        {
            MessagesPanel.SetActive(true);
            ModalPanel.SetActive(false);
            ConsoleMessages += "\n \n ERROR - (" + DateTime.Now + "): " + messageToShow;
            BottomMessageText.text = messageToShow;
            BottomMessagePanel.SetActive(true);
            StartCoroutine(ShowErrorMessage());
            Utilities.GuiUtilities.Instance.CanvasUpdate(ConsoleWindowScroll);
        }

        /// <summary>
        /// Starts the corroutine to wait 3 seconds before hiding the message
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowErrorMessage()
        {
            yield return new WaitForSecondsRealtime(3f);
            BottomMessagePanel.SetActive(false);

            if (ConsoleWindow.activeSelf) yield break;
            MessagesPanel.SetActive(false);
        }

        /// <summary>
        /// Shows or hides the <see cref="ConsoleWindow"/>.
        /// </summary>
        public void ToggleConsoleWindow()
        {
            ModalPanel.SetActive(!MessagesPanel.activeSelf);
            ConsoleWindow.SetActive(!MessagesPanel.activeSelf);
            MessagesPanel.SetActive(!MessagesPanel.activeSelf);
        }
    }
}

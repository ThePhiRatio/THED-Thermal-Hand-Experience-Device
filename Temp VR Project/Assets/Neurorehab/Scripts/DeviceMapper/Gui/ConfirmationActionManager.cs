using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Delegate signature for the confirmation action
    /// </summary>
    /// <param name="confirmation"></param>
    public delegate void ConfirmationActionDelegate(bool confirmation);

    /// <summary>
    /// Class responsible to wait for user confirmation and perform an according action once the confirmation is answered
    /// </summary>
    public class ConfirmationActionManager : MonoBehaviour
    {
        /// <summary>
        /// The Title text field for the confirmation window
        /// </summary>
        [Tooltip("The Title text field for the confirmation window")]
        public Text Title;
        /// <summary>
        /// The Message text field for the confirmation window
        /// </summary>
        [Tooltip("The Message text field for the confirmation window")]
        public Text Message;

        /// <summary>
        /// The default Tittle for the confirmation window.
        /// </summary>
        [SerializeField]
        [Tooltip("The default Tittle for the confirmation window.")]
        private string _defaultTitle = "ARE YOU SURE?";
        /// <summary>
        /// The default Message for the confirmation window.
        /// </summary>
        [SerializeField]
        [Tooltip("The default Message for the confirmation window.")]
        private string _defaultMessage = "Are you sure you want to confirm this action? This is irrevertable!";


        /// <summary>
        /// Bool that indicates there is a confirmation pending
        /// </summary>
        [Tooltip("Bool that indicates there is a confirmation pending")]
        public bool WaitingForConfirmation;
        /// <summary>
        /// The value of the last confirmation
        /// </summary>
        [Tooltip("The value of the last confirmation")]
        public bool ConfirmationValue;
        /// <summary>
        /// The Confirmation Window GameObject
        /// </summary>
        [Tooltip("The Confirmation Window GameObject")]
        public RectTransform ConfirmationWindow;

        /// <summary>
        /// Singleton reference
        /// </summary>
        public static ConfirmationActionManager Instance { get; set; }

        private void Awake()
        {
            Instance = this;
            ConfirmationWindow.gameObject.SetActive(false);
        }

        /// <summary>
        /// Function called by the positive confirmation button. Changes the values of the control variables <see cref="ConfirmationValue"/> and <see cref="WaitingForConfirmation"/> to represent a positive confirmation.
        /// </summary>
        public void ClickYes()
        {
            ConfirmationValue = true;
            WaitingForConfirmation = false;
        }

        /// <summary>
        /// Function called by the negative confirmation button. Changes the values of the control variables <see cref="ConfirmationValue"/> and <see cref="WaitingForConfirmation"/> to represent a negative confirmation.
        /// </summary>
        public void ClickNo()
        {
            ConfirmationValue = false;
            WaitingForConfirmation = false;
        }

        /// <summary>
        /// Function used to start the confirmation process. The <see cref="ConfirmationActionDelegate"/> receives a Bool indicating if the confirmation was positive or not.
        /// </summary>
        /// <param name="action">The action to be performed with the confirmation value.</param>
        /// <param name="title">OPTIONAL. String to override the default title. </param>
        /// <param name="message">OPTIONAL. String to override the default message. </param>
        public void ShowConfirmationWindow(ConfirmationActionDelegate action, string title = "", string message = "")
        {
            Title.text = title == "" ? _defaultTitle : title;
            Message.text = message == "" ? _defaultMessage : message;

            ConfirmationWindow.gameObject.SetActive(true);
            WaitingForConfirmation = true;
            StartCoroutine(WaitingForUserToConfirmAction(action));
        }
        
        /// <summary>
        /// Coroutine that waits for the user confirmation answer. This coroutine runs if the game is paused.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator WaitingForUserToConfirmAction(ConfirmationActionDelegate action)
        {
            yield return null;
            while (WaitingForConfirmation)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    WaitingForConfirmation = ConfirmationValue = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    WaitingForConfirmation = false;
                    ConfirmationValue = true;
                    break;
                }
                yield return null;
            }

            action.Invoke(ConfirmationValue);
            ConfirmationWindow.gameObject.SetActive(false);
        }

    }
}
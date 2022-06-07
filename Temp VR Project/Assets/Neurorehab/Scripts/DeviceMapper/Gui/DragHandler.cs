using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for handling the GUI drag item interaction
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Initial position of the gameobject in the GUI
        /// </summary>
        private Vector3 _initialPosition;

        /// <summary>
        /// The <see cref="SingleInputGui"/> component of this gameobject
        /// </summary>
        private SingleInputGui _singleInputGui;

        /// <summary>
        /// The parent of the gameobject that this script is associated to
        /// </summary>
        internal Transform InitialParent;

        /// <summary>
        /// The temporary parent that this gameobject will have while is being dragged
        /// </summary>
        private Transform _temporaryParent;

        private void Start()
        {
            _initialPosition = transform.localPosition;
            _singleInputGui = GetComponent<SingleInputGui>();

            InitialParent = transform.parent;
            _temporaryParent = DeviceMapperGuiManager.Instance.transform;
        }

        /// <summary>
        /// Called when the gameobject starts being dragged. Sets the <see cref="MapperManager.ItemBeingDragged"/> as this gameobject.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_singleInputGui.SingleInput.InfoTypes.Contains(InformationType.unknown))
            {
                MessageManager.Instance.ShowErrorMessage("You can't drag an input that has an unknown property.");
                return;
            }

            if (_singleInputGui.SingleInput.Connected() == false)
            {
                MessageManager.Instance.ShowErrorMessage("You can't drag an input that you are not receiving at the moment (red input means you are not receiving it). Try sending it by UDP before dragging again.");
                return;
            }

            MapperManager.ItemBeingDragged = _singleInputGui;
            transform.SetParent(_temporaryParent);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    
        /// <summary>
        /// Called while the gameobject is being dragged. Updates itsposition accrding to the event position (mouse)
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        /// <summary>
        /// Called when the gameobject stops being dragged. Calls <see cref="ResetPosition"/> after.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(InitialParent);
            ResetPosition();
        }

        /// <summary>
        /// Resets the position of the gameobejct to its <see cref="_initialPosition"/>
        /// </summary>
        public void ResetPosition()
        {
            transform.localPosition = _initialPosition;
        }
    }
}

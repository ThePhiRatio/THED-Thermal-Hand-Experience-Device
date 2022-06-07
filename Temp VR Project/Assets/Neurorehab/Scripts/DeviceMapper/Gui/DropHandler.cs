using Neurorehab.Scripts.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Neurorehab.Scripts.DeviceMapper.Gui
{
    /// <summary>
    /// Responsible for handling the GUI drop item interaction
    /// </summary>
    public class DropHandler : MonoBehaviour, IDropHandler
    {
        /// <summary>
        /// The <see cref="GameObjectPropertyGui"/> component in the gameobject
        /// </summary>
        private GameObjectPropertyGui _objectProp;

        private void Start()
        {
            _objectProp = GetComponent<GameObjectPropertyGui>();
        }

        /// <summary>
        /// When a <see cref="SingleInputGui"/> is dropped on top of this gameobject, gets the <see cref="SingleInputGui"/> and Triggers its activation in the <see cref="MapperManager"/>
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData)
        {
            if (MapperManager.ItemBeingDragged == null) return;
            
            MapperManager.TriggerGameObjectPropertyActivation(_objectProp);

        }

        /// <summary>
        /// 
        /// <para>Triggered when a <see cref="SingleInputGui"/> is dropped in a mini Property in the GUI</para>
        /// </summary>
        /// <param name="miniObjectProp"></param>
        public void DropSingleInputInMiniProperty(GameObject miniObjectProp)
        {
            if (MapperManager.ItemBeingDragged == null) return;

            var objectProp = miniObjectProp.transform.parent.parent.GetComponentInChildren<GameObjectPropertyGui>();
            
            var axis = (AxisLabels)(miniObjectProp.transform.GetSiblingIndex() + 2);
            MapperManager.TriggerMiniGameObjectPropertyActivation(objectProp, axis);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*Created By: Alex Hua
 * Purpose of class is to handle the buttons on title screen
 * to work with the mouse or keyboard. */
public class ButtonSelection : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    /*Function to select a button when it is hovered over and it's not currently selected*/
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }
    /*Function to deselect a button when the mouse is no longer hovered over the button*/
    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Color_Change_Controller : MonoBehaviour
{
    public Button[] buttons;

    public void changeColor()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Image>().color = Color.blue;
        }
    }

    public void playerClickedButton()
    {
        //Uses the event system in unity to get the current button selected and get the buttons name.

        //After further evaluation this method sucks and I am dumb.
        string answer = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(answer);
    }
}

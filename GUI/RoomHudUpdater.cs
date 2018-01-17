using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomHudUpdater : MonoBehaviour
{
    public Camera camera;

    void Start()
    {
        camera.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        Debug.Log(name + " triggered by: " + trigger.name);
        if (trigger.GetComponent<PlayerControl>())
        {
            string roomName = name;

            camera.gameObject.SetActive(true);

            //gets text from HUD
            Text worldText = GameObject.Find("World").transform.GetChild(1).GetComponent<Text>();
            Image bossIcon = GameObject.Find("World").transform.GetChild(2).GetComponent<Image>();

            //if entering the boss room:
            if (roomName.Substring(5) == "Boss")
            {
                //Display boss icon in HUD
                worldText.gameObject.SetActive(false);
                bossIcon.gameObject.SetActive(true);
            }
            else
            {
                /* Get room number to display in HUD. This code reads the string starting from 'Room_' 
                 * through the rest of the string. I initially thought it would only read that specific index.
                 * Turns out that is not the case.
                 */
                int roomNumber = System.Int32.Parse(roomName.Substring(5));
                //updates text from hud to reflect room number
                if (roomNumber < 10)
                {
                    worldText.text = "0" + roomNumber.ToString();
                }
                else
                {
                    worldText.text = roomNumber.ToString();
                }
            } 
        }
	}

    void OnTriggerExit2D(Collider2D trigger)
    {
        Debug.Log(trigger.name + " Exited " + name);
        if (trigger.GetComponent<PlayerControl>())
        {
            camera.gameObject.SetActive(false); 
        }
    }
}
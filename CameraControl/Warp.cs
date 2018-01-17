

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Warp : MonoBehaviour
{
	public Camera camToDisable, camToEnable;

    public Transform warpTarget;

	//Switches camera to next room when the player walks through a door
	void OnTriggerEnter2D(Collider2D trigger)
    {
        if(trigger.GetComponent<PlayerControl>())
        {
            trigger.GetComponent<PlayerControl>().AddWarpPosition(warpTarget.position);
            trigger.gameObject.transform.position = warpTarget.position;
            /* Allows PlayerHealth.cs to get the player's current room number in order to properly
             * reassign the player's position when it takes damage
             */
            trigger.GetComponent<PlayerControl>().room = warpTarget.transform.parent.parent;

            camToDisable.gameObject.SetActive(false);
            camToEnable.gameObject.SetActive(true);
        }
	}
}
/* Author Gabriel B. Gallagher <Insert date when I figure it out>
 * 
 * Controls the player character's ability to go through doors or portals and into new rooms. Also
 * used to contain logic which enabled/disabled the camera based on the character's location. This
 * logic is now handled in the RoomHUDupdater but this is a fairly new change so there might be some
 * bugs as a result of it
 */

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

            //This code is deprecated. switching cameras is now handled in the RoomHUDupdater
            camToDisable.gameObject.SetActive(false);
            camToEnable.gameObject.SetActive(true);
        }
	}
}
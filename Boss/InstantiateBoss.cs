/* Author: Gabriel B. Gallagher January 28th, 2017
 * 
 * Spawns the Boss when the player enters the boss room. Is attached to any boss rooms in any level.
 * Requires a boss prefab and a spawn location. Is called when a player first enters the room, and will 
 * only be called once.
 */

using UnityEngine;

public class InstantiateBoss : MonoBehaviour
{
    public GameObject bossPrefab;

    public Transform spawnLocation;

    bool bossInstantiated = false;

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if(trigger.tag == "Player" && !bossInstantiated)
        {
            GameObject bossRoom = trigger.GetComponent<PlayerControl>().room.gameObject;
            GameObject boss = 
                Instantiate(bossPrefab, spawnLocation.transform.position, Quaternion.identity);
            boss.transform.parent = bossRoom.transform;
            bossInstantiated = true; //ensure that boss is only instantiated once. Need for multiplayer
        }
    }
}
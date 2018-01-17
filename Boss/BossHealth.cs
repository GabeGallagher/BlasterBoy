/* Author: Gabriel B. Gallagher August 29th, 2017
 * 
 * This script is used to transition a boss to a new phase. I'm currently leaving this as a resuable 
 * script and naming it BossHealth rather than TutorialBossHealth so it may be reused later
 * 
 * Enjoy!!
 */

using UnityEngine;

public class BossHealth : Health
{
    public GameObject newBossPrefab;

    public override void DealDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            /* If this script is attached to the last phase of a boss encounter, dropping the boss to 0
             * or less health will trigger load the Win screen. Otherwise, the next phase will begin.
             * 
             * Simply instantiates a new phase for now. Instantiation will be handled in the animator
             * when this encounter is released, and this method will be for triggering the animation
             */
            if (!GetComponent<BossControl>().triggerPhaseChange && 
                GetComponent<BossControl>().isFinalBoss)
            {
                GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadLevel(
                    "Win Screen");
            }
            else if (GetComponent<BossControl>().triggerPhaseChange)
            {
                GameObject newBoss = Instantiate(newBossPrefab, transform.position, Quaternion.identity);
                newBoss.transform.parent = gameObject.transform.parent;
                Destroy(transform.gameObject); 
            }
        }
    }
}

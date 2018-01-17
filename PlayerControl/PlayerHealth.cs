/* Author: Gabriel B. Gallagher September 14th, 2017
 * 
 * Script which inherits Health.cs and overrides DealDamage for behavior specifically for the player
 * character
 */

using UnityEngine;

public class PlayerHealth : Health
{
    PlayerControl player;

    private void Start()
    {
        player = gameObject.GetComponent<PlayerControl>();
    }

    public override void DealDamage(int damage)
    {
        if(!player.isInvincible)
        {
            health -= damage;

            /* If damage sets the players' health to 0, reset bomb range and load the game over screen.
             * Will also trigger death animation when implemented in animator
             */
            if (health <= 0 && player.room.tag != "BossRoom")
            {
                player.bombRange = 1;
                GameObject bombParent = player.bombParent;
                //Destroy any bombs still in the scene after the player character dies
                for (int i = 0; i < bombParent.transform.childCount; i++)
                {
                    Destroy(bombParent.transform.GetChild(i));
                }
                GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadLevel(
                    "Game Over Screen");
                Destroy(gameObject);
            }
            else if (health <= 0 && player.room.tag == "BossRoom")
            {
                player.bombRange = 1;
                GameObject bombParent = player.bombParent;
                //Destroy any bombs still in the scene after the player character dies
                for (int i = 0; i < bombParent.transform.childCount; i++)
                {
                    Destroy(bombParent.transform.GetChild(i));
                }
                player.transform.position = 
                    player.room.gameObject.GetComponent<OnPlayAgain>().respawnPoint;
                GameObject.Find("LevelManager").GetComponent<LevelManager>().LoadLevel(
                    "GameOverScreen_Boss");
            }
            else
            {
                player.isInvincible = true;

                //Player character should not move if it is hurt in the boss room
                if (player.room.tag != "BossRoom")
                {
                    player.ReturnToPreviousWarpPosition();
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public class DestructableRockControl : MonoBehaviour
{
	//Class used for spawning an accessable checkpoint when a destructable block is destroyed
	public GameObject checkpointToDestroy, checkpointToSpawn, checkpointParent;

	public List<GameObject> powerUpsList;

    public int health = 1, powerUpDropChance = 1;

	public bool dropsPowerUp = false;

	void DropPowerUp()
    {
		int rNum = Random.Range(0, powerUpDropChance);

		if(rNum == (powerUpDropChance - 1))
        {
			int powerUpIndex = Random.Range(0, powerUpsList.Count-1);
			GameObject powerUp = Instantiate(powerUpsList[powerUpIndex], transform.position, 
				Quaternion.identity) as GameObject;
		}
	}

	public virtual void DealDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(checkpointToDestroy);
            GameObject checkpoint = Instantiate(checkpointToSpawn, transform.position,
                Quaternion.identity) as GameObject;
            /*organizes new checkpoints under Checkpoints in the hierarchy. Must assign a
            * checkpoint parent. Otherwise, the block will not be destroyed.
            */
            checkpoint.transform.parent = checkpointParent.transform;

            if (dropsPowerUp)
            {
                DropPowerUp();
            }

            Destroy(gameObject);
        }
	}
}
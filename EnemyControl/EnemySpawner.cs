using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemyPrefab;

	private bool enemiesSpawned = false;

    //Delegate/event candidate
	void SpawnEnemies()
    {
		foreach (Transform child in transform)
        {
			GameObject enemy = Instantiate(enemyPrefab, child.transform.position, 
				Quaternion.identity) as GameObject;
			enemy.transform.parent = child;
		}
		enemiesSpawned = true;
	}

    //Destroys all enemies in spawning group when the player leaves the room containing the group
	void ShredEnemies()
    {
		int count = 0;
		foreach (Transform child in transform)
        {
			if (transform.GetChild(count).childCount > 0)
            {
				GameObject enemy = transform.GetChild(count).GetChild(0).gameObject;
				Destroy(enemy);
			}
			count++;
		}
	}

	void OnTriggerEnter2D(Collider2D trigger)
    {
		if(!enemiesSpawned)
        {
			SpawnEnemies();
		}
	}

	void OnTriggerExit2D(Collider2D trigger)
    {
		if(enemiesSpawned)
        {
			ShredEnemies();
		}
		enemiesSpawned = false;
	}
}
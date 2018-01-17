/* Author: Gabriel B. Gallagher November 30th, 2016
 * 
 * Contains the behavior for the bomb explosions. Determines when to deal damage to an object, and when
 * to expire.
 */

using UnityEngine;

public class Explosion : MonoBehaviour
{
    //Determines the length of time that the explosion will last and inflict damage on characters
    public float explosionTime = 3.0f;

	void OnTriggerEnter2D(Collider2D trigger)
    {
		if (trigger.GetComponent<Health>())
        {
            trigger.GetComponent<Health>().DealDamage(1);
		}

        //Destroy bombs
        if (trigger.GetComponent<BombControl>())
        {
			BombControl bomb = trigger.GetComponent<BombControl>();
			bomb.Explode();
		}
	}

    //Destroys explosion game object from the animator. Currently disabled because explosion animations
    //were lost
	void ExplosionEnd()
    {
		Destroy(gameObject);
	}

    /* This script is intended to be used in the animation controller. After explosion
     * animations are complete, set the ExplosionEnd() method as an animation event at the end of
     * of the explosion animation and delete the Update() method, as it will not be needed.
     */
    void Update()
    {
		if (explosionTime > 0)
        {
            Destroy(gameObject, explosionTime);
        }
	}
}
/* Author: Gabriel B. Gallagher November 30th, 2016
 * 
 * Key script which controls the behavior for the players' bombs. Contains behavior such as when and
 * when not to spawn an explosion object, and how the bombs interact with various characters in the
 * game
 * 
 * Enjoy!!
 */

using UnityEngine;
using System.Collections;

public class BombControl : MonoBehaviour
{
	public GameObject explosionPrefab, creator;
	public BoxCollider2D bombBoxCollider, centralExplosionCollider;
	public LayerMask levelMask;

	GameObject bombParent, explosionParent;
	BoxCollider2D boxCollider;

    float instantiationTime;

	bool isCollider = false;
	bool isExploding = false;

    private void Start()
    {
        bombParent = GameObject.Find("Bombs");
        instantiationTime = Time.timeSinceLevelLoad;
    }

    //Coroutine for spawning the explosion objects
    private IEnumerator CreateExplosions(Vector3 direction)
    {
        float maxRange = creator.GetComponent<PlayerControl>().bombRange;

		for (int i = 1; i <= maxRange; i++)
        {
			RaycastHit2D hit = Physics2D.Raycast (transform.position + new Vector3(0.0f, 0.0f, 0.0f), 
                direction, i, levelMask);

			if (!hit.collider)
            {
				if (direction == Vector3.up)
                {
                    GameObject explosionUp = Instantiate(explosionPrefab, transform.position + (
                        i * direction), Quaternion.Euler(new Vector3(0.0f, 0.0f, 90.0f)),
                        explosionParent.transform);
                    explosionUp.name = "explosionUp";
                }
                else if (direction == Vector3.down)
                {
                    GameObject explosionDown = Instantiate(explosionPrefab, transform.position + (
                        i * direction), Quaternion.Euler(new Vector3(0.0f, 0.0f, 270.0f)),
                        explosionParent.transform);
                    explosionDown.name = "explosionDown";
                }
                else if (direction == Vector3.left)
                {
                    GameObject explosionLeft = Instantiate(explosionPrefab, transform.position + (
                        i * direction), Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f)),
                        explosionParent.transform);
                    explosionLeft.name = "explosionLeft";
                }
                else //explosionRight
                {
                    GameObject explosionRight = Instantiate(explosionPrefab, transform.position + (
                        i * direction), Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)), 
                        explosionParent.transform);
                    explosionRight.name = "explosionRight";
                }
			}
            else
            {
				Collider2D col = hit.collider;
				if (col.GetComponent<DestructableRockControl>())
                {
					col.GetComponent<DestructableRockControl>().DealDamage(1);
				}
                break;  //prevents the script from evaluating and instantiating an explosion beyond an
                        //object that has been hit
			}
		}
		yield return new WaitForSeconds (0.005f);
	}

    public void Explode()
    {
		if(!isExploding)
        {
            explosionParent = new GameObject(name);
            explosionParent.transform.parent = bombParent.transform;
            explosionParent.AddComponent<ExplosionParent>();
			isExploding = true;
			CameraControl.Shake();

			Instantiate(centralExplosionCollider, transform.position, Quaternion.identity, 
                explosionParent.transform);

			StartCoroutine(CreateExplosions(Vector3.up));
	        StartCoroutine(CreateExplosions(Vector3.right));
	        StartCoroutine(CreateExplosions(Vector3.down));
	        StartCoroutine(CreateExplosions(Vector3.left));

			Destroy (gameObject, 0.005f);
			//GUIScript.IncreaseBombTotal();
		}
	}

    //Get age of this bomb
    public float GetAge()
    {
        return Time.timeSinceLevelLoad - instantiationTime;
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {
        /* Hack solution to issue where player can stack bombs if bomb placement is pressed quickly enough
         * this solution MAY be causing frame stuttering problems, but it will have to be tested in the
         * built executable project. Solution works by seeing if two bombs are sitting on top of each 
         * other and destroying the newer of the two bombs.
         */
        if (trigger.tag == "Bomb" && trigger.transform.position == this.transform.position)
        {
            if (this.GetAge() > trigger.GetComponent<BombControl>().GetAge())
            {
                Destroy(trigger.gameObject);
            }
        }
    }

    //Creates a 2D box collider on the bomb once the players steps off of it
    void OnTriggerExit2D (Collider2D collider)
    {
		if(!isCollider)
        {
			boxCollider = Instantiate(bombBoxCollider, transform.position, Quaternion.identity) 
				as BoxCollider2D;
			boxCollider.transform.parent = transform;
			isCollider = true;
		}
	}
}
/* Author: Gabriel B. Gallagher November 30th 2016
 *
 * This is the Enemy Controller script, which is a rudimentary AI designed to be used
 * by the basic GUMBot enemy, an later inherited by future, more complex enemies, as I
 * add them into the game. 
 * 
 * The AI begins after the enemy is spawned, which occurs when the player character 
 * enters a room. Enemies are spawned using a separate, enemy spawner script, which is 
 * simply a boolean value as whether or not the player character is in the room. After 
 * the enemy is spawned, it automatically moves randomly eith up, right, down, or left.
 * Each time the enemy hits one of the node triggers within the matrix, it will again
 * decide to move either up, right, down, or left. However, this decision is only passed
 * if the script determines that the enemy has moved as far as it can in the current
 * direction (ie, the enemy will only change direction if it collides with a wall, bomb,
 * or player character). Additionally, enemies will not go in the direction which they
 * just arrived from, unless it is the only direction they may go (ie, if the enemy is
 * moving to the left and hits a bomb, it will prioritize moving up or down before it
 * turns around and goes right).
 * 
 * Enjoy!!
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyControl : MonoBehaviour
{
	//Public Variables
	public LayerMask layerMask;

	public float speed = 1.0f;
	public float rayDistance = 0.5f;

	public int temp;

	public int direction = 1;

	public bool aggroState = true;

    //Private Variables
    private List<GameObject> rayPoints;

    private List<Ray2D> raysUp;
    private List<Ray2D> raysDown;
    private List<Ray2D> raysLeft;
    private List<Ray2D> raysRight;

    private List<int> possibleDirectionsList;

    private int up = 1;
	private int down = -1;
	private int right = 2;
	private int left = -2;

	//Raycasting booleans
	private bool collisionUp = false;
	private bool collisionRight = false;
	private bool collisionDown = false;
	private bool collisionLeft = false;

    //Finding Player booleans
    private bool checker = false;

    private bool playerUp = false;
	private bool playerRight = false;
	private bool playerDown = false;
	private bool playerLeft = false;

	void GetRays() {
		//Get the object named Raycasting
		List<GameObject> children = gameObject.GetChildren();
		
		//Get the children inside Raycasting
		List<GameObject> children2 = new List<GameObject>();
		
		/*Check object for RayCasting child (inside Room_<current room>/Room_<current room>_Enemies/
			<this.name>(Clone)/RayCasting)*/
		for (int i = 0; i < children.Count; i++) {
			if (children[i].name == "RayCasting") {
				children2 = children[i].GetChildren();
			}
		}

		//Cycle through the Children of RayCasting and assigns them to a list called rayPoints
		for (int i = 0; i < children2.Count; i++) {
			rayPoints.Add(children2[i]);
		}
	}

	void Start() {
		rayPoints = new List<GameObject>();
		GetRays();
		possibleDirectionsList = new List<int>();
	}

	/* Generates a list of lists of rays. Rays will collide with objects on the checkpoints, 
     * bombs, and player layers to get important feedback on what the enemy is colliding with
     * at any given time.
	 */
	void AssignRaysToList()
    {
		raysUp = new List<Ray2D>();
		raysDown = new List<Ray2D>();
		raysLeft = new List<Ray2D>();
		raysRight = new List<Ray2D>();

		for (int i = 0; i < rayPoints.Count; i++)
        {
			//up
			if (rayPoints[i].gameObject.name == "up")
            {
				raysUp.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.up));
			}

			//down
			if (rayPoints[i].gameObject.name == "down")
            {
				raysDown.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.down));
			}

			//right
			if (rayPoints[i].gameObject.name == "right")
            {
				raysRight.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.right));
			}

			//left
			if (rayPoints[i].gameObject.name == "left")
            {
				raysLeft.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.left));
			}
		}
	}

	//Generates a list of possible directions in which there are no immediate collisions
	void AddDirectionToList()
    {
		if (!collisionUp)
        {
			possibleDirectionsList.Add(up);
		} 

		if (!collisionRight)
        {
			possibleDirectionsList.Add(right);
		}

		if (!collisionDown)
        {
			possibleDirectionsList.Add(down);
		}

		if (!collisionLeft)
        {
			possibleDirectionsList.Add(left);
		}
	}

	/* Takes a direction ray from the previously generated list, and determines what the ray hit,
     * if anything.
	 */
	bool IsCollision (List<Ray2D> rayList)
    {
		for (int i = 0; i < rayList.Count; i++)
        {
			RaycastHit2D hit = Physics2D.Raycast(rayList[i].origin, rayList[i].direction, 
				rayDistance + .001f, layerMask);

			if (hit.collider)
            {
				return true;
			}
		}
		return false;
	}

	//Generates a random direction from the list of possible directions for the enemy to go
	void RandomDirection()
    {
		temp = Random.Range(0, possibleDirectionsList.Count);
		direction = possibleDirectionsList[temp];
	}

	/* Uses the rays list and collision data to determine how the enemy should respond in the
     * event of a collision.
	 */
	void CheckCollision()
    {
		AssignRaysToList();

		collisionDown = IsCollision(raysDown);
	
		collisionUp = IsCollision(raysUp);

		collisionLeft = IsCollision(raysLeft);
	
		collisionRight = IsCollision(raysRight);

		AddDirectionToList();

		if (checker == false)
        {
			RandomDirection();
			possibleDirectionsList.Clear();
		}
        else
        {
			if (possibleDirectionsList.Count > 1)
            {
				for (int i = 0; i < possibleDirectionsList.Count; i++)
                {
				   /* NOTE: This is the most important block of code in this entire script!
                    * 
                    * The following line of code looks at the direction int and the int of
                    * possibleDirectionsList at element i. If the two ints are opposite each other, or
                    * are opposite directions, in other words, the int at element i will be removed so
                    * that the character cannot move in the direction from where it just came.
				    */
					if (direction + possibleDirectionsList[i] == 0)
                    {
						if (possibleDirectionsList[i] == up)
                        {
							possibleDirectionsList.Remove(up);
						}
                        else if (possibleDirectionsList[i] == down)
                        {
							possibleDirectionsList.Remove(down);
						}
                        else if (possibleDirectionsList[i] == right)
                        {
							possibleDirectionsList.Remove(right);
						}
                        else
                        {
							possibleDirectionsList.Remove(left);
						}
					}
				}
				RandomDirection();
				possibleDirectionsList.Clear();
			}
            else
            {
				RandomDirection();
				possibleDirectionsList.Clear();
			}
		}
		checker = true;	
	}

	/* This is an essential method for the aggro state feature which is not included in the current
     * build. This method takes a ray, which represents the enemy's line of sight and checks to see
     * if the ray specifically hits the player character. In the event that it does, the script
     * simply tells us so, and the enemy continues on its merry way. In a future build, however, 
     * the enemy will do something VERY nasty to the player character.
	 */
	bool IsPlayer(List<Ray2D> rayList)
    {
		float distance = Mathf.Infinity;

		for (int i = 0; i < rayList.Count; i++)
        {
			RaycastHit2D hit = Physics2D.Raycast(rayList[i].origin, rayList[i].direction, 
				distance, layerMask);

			if (hit.collider)
            {
				if (hit.collider.tag == "Player")
                {
					Debug.Log("I am " + name + " and I saw: " + hit.collider.name);
					return true;
				}
			}
		}
		return false;
	}

	/* Method utilizes the isPlayer method to determine if there is a player character within the
     * enemy's line of sight in any of the up, right, down, or left directions.
	 */
	void CheckForPlayer()
    {
		AssignRaysToList();

		playerDown = IsPlayer(raysDown);
	
		playerUp = IsPlayer(raysUp);

		playerLeft = IsPlayer(raysLeft);
	
		playerRight = IsPlayer(raysRight);
	}

	/* Switches the enemy state if it sees the player character.
     * 
     * TODO: pretty much everything. The aggro state is a work in progress. The idea is that
     * when the enemy sees the player character, it will play a quick animation to let the
     * player know he/she has been seen. The enemy will then increase its speed and immediately
     * move in the player character's direction. There are still some design issues and bugs to
     * work out, but I expect this behavior to be included in a near future version.
	 */
	void Aggro()
    {
		Debug.Log("In aggro");
	}

	/* Basic walking method for the enemy. It uses a switch statement which takes the direction
     * the enemy is currently moving and moves them at the given walk speed in that direction.
     * Called in the update loop.
	 */
	void Walk()
    {
		switch (direction)
        {
			//up
			case 1:
				transform.position += new Vector3(0.0f, speed * Time.deltaTime, 0.0f);
					break;
			//down
			case -1:
				transform.position += new Vector3(0.0f, -speed * Time.deltaTime, 0.0f);
				break;
			//right
			case 2:
				transform.position += new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
				break;
			//left
			case -2:
				transform.position += new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f);
				break;
		}
	}	

	//Draw all rays in the list for testing purposes
	void DrawRaycast()
    {
		for (int i = 0; i < rayPoints.Count; i++)
        {
            switch (rayPoints[i].gameObject.name)
            {
                case "up":
                    Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(
                        rayPoints[i].gameObject.transform.position.x,
                        rayPoints[i].gameObject.transform.position.y + rayDistance,
                        rayPoints[i].gameObject.transform.position.z), Color.green);
                    break;

                case "right":
                    Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(
                        rayPoints[i].gameObject.transform.position.x + rayDistance,
                        rayPoints[i].gameObject.transform.position.y,
                        rayPoints[i].gameObject.transform.position.z), Color.green);
                    break;

                case "down":
                    Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(
                        rayPoints[i].gameObject.transform.position.x,
                        rayPoints[i].gameObject.transform.position.y - rayDistance,
                        rayPoints[i].gameObject.transform.position.z), Color.green);
                    break;

                case "left":
                    Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3(
                        rayPoints[i].gameObject.transform.position.x - rayDistance,
                        rayPoints[i].gameObject.transform.position.y,
                        rayPoints[i].gameObject.transform.position.z), Color.green);
                    break;

                default:
                    Debug.Log("Error drawing rays");
                    break;
            }
		}
	}

	/* Matrix nodes, called checkpoints, exist on every one world unit within the map. Each time
     * the enemy is triggered by one of these nodes, it will call the CheckCollision and 
     * CheckForPlayer methods to determine its next action.
     * 
     * Also tells the character to go back in the direction it just came from when it collides with a
     * player character. This should fix the bug where the enemies would occasionally get knocked off
     * track after making certain collisions.
     * 
     * It's worth noting that this code still allows the player to "sneak up" behind the enemy and 
     * potentially trigger it multiple times in very fast succession with i-frames still up. This
     * exploit will cause the enemy to reverse its direction many times, and can be used to somewhat
     * manipulate the enemy's movements, particularly glitching it through blocks or off of the map. 
     * I haven't fixed this exploit yet because the inclusion of the aggro state will likely make it so
     * the enemy is will always attack the player. In other words, the player will never be able to get
     * behind the enemy, so this exploit won't be possible. In the event that I decide to add enemies
     * that don't aggro to the player, I will take another look at this block.
	 */
	void OnTriggerEnter2D(Collider2D trigger)
    {
        //This Character goes in the opposite direction it was going when it collided with the player
        if (trigger.tag == "Player")
        {
            direction *= -1;
            trigger.GetComponent<Health>().DealDamage(1);
        }

        if (trigger.GetComponent<CheckPoint>())
        {
			CheckCollision();
			CheckForPlayer();
		}
	}

    /* This block only applies to collisions with bombs now. All other "collisions" are handled with
     * triggers, and most of the logic is on other game objects. This was done intentionally to try and
     * fix the free walk bug, which occurs when a collision slightly bumps the enemy off of its track,
     * allowing it to phase through blocks, walls, and off of the entire map.
     */
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionPoint = collision.contacts[collision.contacts.Length - 1].point;
        Vector2 collisionCenter = collision.collider.bounds.center;

        //Collision coordinates
        float topValue = collisionCenter.y - collisionPoint.y;
        float botValue = collisionPoint.y - collisionCenter.y;
        float rightValue = collisionCenter.x - collisionPoint.x;
        float leftValue = collisionPoint.x - collisionCenter.x;

        /* Creates a new list which will be filled with collision difference values. these 
         * values will be sorted in the list from smallest to largest, after which, the last
         * value, which will always be the largest, will be taken, and the character will 
         * change direction to go in the opposite direction from where the collision happened. 
		 */
        List<float> valueList = new List<float>();

        valueList.Add(topValue);
        valueList.Add(botValue);
        valueList.Add(rightValue);
        valueList.Add(leftValue);

        valueList.Sort();
        
        //If the character collides with a bomb, this logic sends them in the opposite direction
        if (valueList[3] == topValue)
        {
            direction = -1;
        }
        else if (valueList[3] == botValue)
        {
            direction = 1;
        }
        else if (valueList[3] == rightValue)
        {
            direction = -2;
        }
        else if (valueList[3] == leftValue)
        {
            direction = 2;
        }
    }

    void Update()
    {
		if(aggroState)
        {
			Aggro();
		}
        else
        {
			Walk();
		}

		DrawRaycast();
	}
}
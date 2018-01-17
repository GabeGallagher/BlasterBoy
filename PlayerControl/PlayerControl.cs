/* Author: Gabriel B. Gallagher November 30th, 2016
 * 
 * This is the player controller script for Blaster Boy. The script is designed to be modular and used
 * by a variety of characters, not just the main character which it is currently used by, Kid Blaster.
 * The script allows for player input for basic up, right, down, left movement as well as movement in
 * the in betweens. The player is also able to drop bombs, which run on a separate script, but are
 * integral to the core gameplay. Finally, this script also calls the Unity Animator for basic movement
 * animations, which are currently disabled as I update them.
 * 
 * Enjoy!!
 */

using UnityEngine;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
	//Public Variables
    public GameObject currentBombPrefab, bombParent;
        
    public Transform room;
    
	public float maxSpeed = 5.0f;

	public int bombMaxCount = 3;
    public int bombMaxRange = 3;
    public int bombRange = 1;

    public bool isInvincible = false;

    //Private Variables
    Animator anim;

    List<Vector2> warpPositionList;

    float xmin;
	float xmax;
	float ymin;
	float ymax;

	void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();

        //instantiate warp position list and add the player's initial position to the list
        warpPositionList = new List<Vector2>{ transform.position };

        //Creates a parent for bomb objects in the hierarchy
        bombParent = GameObject.Find("Bombs");
		if(!bombParent)
        {
			bombParent = new GameObject("Bombs");
		}
	}

	/* Controls players movements. Inputs are WASD keys on the keyboard.
     * TODO GamePad and arrow key controls
	 */
	void Movement()
    {
		//Down
		if (Input.GetKey (KeyCode.S))
        {
			transform.position += new Vector3 (0.0f, -maxSpeed*Time.deltaTime, 0.0f);
        }

        //Up
        if (Input.GetKey (KeyCode.W))
        {
			transform.position += new Vector3 (0.0f, maxSpeed*Time.deltaTime, 0.0f);
        }

        //Left
        if (Input.GetKey (KeyCode.A))
        {
			transform.position += new Vector3 (-maxSpeed*Time.deltaTime, 0.0f, 0.0f);
        }

        //Right
        if (Input.GetKey (KeyCode.D))
        {
			transform.position += new Vector3 (maxSpeed*Time.deltaTime, 0.0f, 0.0f);
		}
	}

    //Method to handle animation transitions in the Update method
    //TODO GamePad and arrow key controls
    void MovementAnimationHandler()
    {
		//Down
		if (Input.GetKey (KeyCode.S))
        {
			anim.SetBool("isRunningFront", true);
		} 
		//Transition to Idle
		else if (Input.GetKeyUp (KeyCode.S))
        {
			anim.SetBool ("isRunningFront", false);
		}

		//Up
		if (Input.GetKey (KeyCode.W))
        {
			anim.SetBool("isRunningBack", true);
		} 
		//Transition to Idle
		else if (Input.GetKeyUp (KeyCode.W))
        {
			anim.SetBool ("isRunningBack", false);
		}

		//Left
		if (Input.GetKey (KeyCode.A))
        {
			anim.SetBool("isRunningLeft", true);
		} 
		//Transition to Idle
		else if (Input.GetKeyUp (KeyCode.A))
        {
			anim.SetBool ("isRunningLeft", false);
		}

		//Right
		if (Input.GetKey (KeyCode.D))
        {
			anim.SetBool("isRunningRight", true);
		} 
		//Transition to Idle
		else if (Input.GetKeyUp (KeyCode.D))
        {
			anim.SetBool ("isRunningRight", false);
		}

        /* The following code is used to prevent animation in instances where Up & Down are pressed
         * simultaneously, and Left & Right are pressed Simultaneously
		 */
		//Up/Down
		if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.S))
        {
			anim.SetBool ("isRunningBack", false);
			anim.SetBool ("isRunningFront", false);
		}
		//Left/Right
		if (Input.GetKey (KeyCode.A) && Input.GetKey (KeyCode.D))
        {
			anim.SetBool ("isRunningLeft", false);
			anim.SetBool ("isRunningRight", false);
		}
	}

	/* Controls player character's ability to drop bombs.
     * 
     * TODO GamePad and arrow key controls
	 */
	void BombDrop()
    {
		int bombsOnField = bombParent.transform.childCount;

        if (bombsOnField == 0)
        {
            InstantiateBomb(bombsOnField + 1);
            //GUIScript.DecreaseBombTotal();
        }
        else
        {
            /* Loop checks to see if Player's location is within one space of any bombs on the map
             * If it is, dropping bombs will be prevented. This solves the bomb stacking bug.
             */
            for (int i = 0; i < bombsOnField; i++)
            {
                if (!((transform.position.x <=
                        bombParent.transform.GetChild(i).transform.position.x + 0.7f &&
                    transform.position.x >=
                        bombParent.transform.GetChild(i).transform.position.x - 0.7f) &&
                    (transform.position.y <=
                        bombParent.transform.GetChild(i).transform.position.y + 0.7f &&
                    transform.position.y >=
                        bombParent.transform.GetChild(i).transform.position.y - 0.7f)))
                {
                    InstantiateBomb(bombsOnField + 1);
                    //GUIScript.DecreaseBombTotal();
                }
            } 
        }
	}

	//method for creating the bomb in the BombDrop method
	void InstantiateBomb(int bombNumber)
    {
		GameObject currentBomb = Instantiate (currentBombPrefab, new Vector3 (Mathf.RoundToInt(
			transform.position.x), Mathf.RoundToInt(transform.position.y), 0.0f), Quaternion.identity) as 
            GameObject;

        currentBomb.GetComponent<BombControl>().creator = this.gameObject;

        //Hack solution to an issue where player can drop more than the allotted number of bombs if 
        //bomb placement is pressed quickly enough
        currentBomb.transform.parent = bombParent.transform;

        if(bombParent.transform.childCount > bombMaxCount)
        {
            Destroy(currentBomb);
        }
        else
        {
            currentBomb.name = "Bomb_Basic(Clone)_0" + bombNumber; 
        }
	}

    //Adds a position the player has warped to to the stack
    public void AddWarpPosition(Vector2 warpTarget)
    {
        warpPositionList.Add(warpTarget);
        transform.position = warpTarget;
    }

    public void ReturnToPreviousWarpPosition()
    {
        transform.position = warpPositionList[warpPositionList.Count - 1];
    }

    //Turn on iFrames after player takes damage
    //Delegate/Event Candidate
    void Invincibility()
    {
        if (isInvincible)
        {
            anim.SetBool("isInvincible", true);
        }
        else
        {
            anim.SetBool("isInvincible", false);
        }
    }

    /* NOTE: Currently using Update because while using FixedUpdate, there is no guarantee that GetKeyUp()
     * from the Movement method will be captured and may cause the player to get stuck in one of his 
     * movement animtions despite being in Idle state. However, Update causes camera jitter for any
     * collision not on the edges of the screen. Need to find a workaround so that the character won't 
     * jitter or get stuck in an animation while in idle state. Separating the animation states from the 
     * movement method might be best way to handle this problem.
     */
    void FixedUpdate()
    {
		Movement();
	}

	//Method for animation. GetKeyUp has to be handled here
	void Update()
    {
        //MovementAnimationHandler();   
        //Would like this method called alongside Movement in FixedUpdate if possible

        if (Input.GetKeyDown(KeyCode.Space) && (bombParent.transform.childCount < bombMaxCount))
        {
            BombDrop();
        }
        Invincibility();
	}
}
/* Author: Gabriel B. Gallagher January 28th, 2017
 * 
 * This script controls the tutorial boss. The tutorial boss has two phases. In the first phase, it moves
 * around the room and periodically attempts to step on the player character. The player deals damage by
 * dropping bombs where the boss is about to step, and quickly moving out of the way. After a certain 
 * amount of damage, the boss shifts to phase two, where he gains three arms which he will use to grab 
 * the player, adding an extra layer of complexity to his mechanics.
 */

using UnityEngine;

public class BossControl : MonoBehaviour
{
    //Public varables
	public GameObject footPrefab, player, bossRoom, targetPrefab, targetParent;

    public AudioClip damageAudio;

	public float moveDist = 3.0f;

	public int moveSpeed = 1;

	public bool isMoving, isCheckingPlayerLocation, isAttacking, isFiringProjectile, targetTriggered,
        isFinalBoss, triggerPhaseChange;

    //Protected variables used in inherited classes
    protected Animator anim;

    protected Vector3 playerPingedPosition;

    //movement booleans
    protected bool isTop, isCenter, isBot;

    //Private variables
    AudioSource audioSource;

    //position information
    Vector3 topPos, centerPos, botPos, playerPos;

	//animation booleans
	bool moveUp, moveRight, moveDown, moveLeft;

    public virtual void Start()
    {
		//Get game objects
		player = GameObject.Find("Player");
		bossRoom = GameObject.Find("Room_Boss");
        targetParent = GameObject.Find("Targets");
        audioSource = GetComponent<AudioSource>();

		//Get animator
		anim = transform.GetChild(0).GetComponent<Animator>();

		//Boss positions
		topPos = new Vector3(0.0f, moveDist, 0.0f);
		centerPos = new Vector3(0.0f, 0.0f, 0.0f);
		botPos = new Vector3(0.0f, -(moveDist), 0.0f);

        //Player Position
        playerPingedPosition = player.transform.position;

        //Set the boss to move on instantiation
        isMoving = true;

        //Boss movement booleans
        isTop = true;
    }

    protected Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    protected void SetPositionBoolsFalse()
    {
        isCenter = false;
        isTop = false;
        isBot = false;
    }

    void Idle()
    {
        anim.SetBool("attackRight", false);
        anim.SetBool("attackLeft", false);
        anim.SetBool("isIdle", true);
    }

    void MoveDown()
    {
        //Handles movement behavior
        anim.SetBool("isIdle", false);
        anim.SetBool("moveDown", true);
        transform.localPosition += new Vector3(0.0f, -moveSpeed * Time.deltaTime, 0.0f); //Mover

        if (!isCenter)
        {
            //Handles behavior once the boss reaches the center of the room
            if (transform.localPosition.y <= centerPos.y)
            {
                isMoving = false;
                SetPositionBoolsFalse();
                isCenter = true;
                anim.SetBool("moveDown", false);
                Attack();
            }
        }
        else
        {
            if (transform.localPosition.y <= botPos.y)
            {
                isMoving = false;
                SetPositionBoolsFalse();
                isBot = true;
                anim.SetBool("moveDown", false);
                Attack();
            }
        }
    }

    void MoveUp()
    {
        //Handles movement behavior
        anim.SetBool("isIdle", false);
        anim.SetBool("moveUp", true);
        transform.localPosition += new Vector3(0.0f, moveSpeed * Time.deltaTime, 0.0f); //Mover

        if (!isCenter)
        {
            //Handles behavior once the boss reaches the center of the room
            if (transform.localPosition.y >= centerPos.y)
            {
                isMoving = false;
                SetPositionBoolsFalse();
                isCenter = true;
                anim.SetBool("moveUp", false);
                Attack();
            }
        }
        else
        {
            if (transform.localPosition.y >= topPos.y)
            {
                isMoving = false;
                SetPositionBoolsFalse();
                isTop = true;
                anim.SetBool("moveUp", false);
                Attack();
            }
        }
    }

    protected void GetMovementState()
    {
        if (isMoving)
        {
            if ((isTop || isCenter) && playerPingedPosition.y < bossRoom.transform.position.y)
            {
                MoveDown();
            }
            else if ((isBot || isCenter) && playerPingedPosition.y > bossRoom.transform.position.y)
            {
                MoveUp();
            }
            /* In the event that the Players' position dictates that the boss cannot or should not move,
             * the Boss should just restart its attack sequence
             */
            else
            {
                isAttacking = true;
                isMoving = false;
                Attack();
            }
        }
        //TODO create a delegate that broadcasts when boss is attacking.
        else if (isAttacking)
        {
            Attack();
        }
    }

    /* Fires the foot prefab at the player's location at the time of the method call. Floats are based on
     * the position/rotation required to make the prefab appear as if it is detaching from the boss body.
     */
    protected virtual void Attack()
    {
        isCheckingPlayerLocation = true;
        playerPingedPosition = GetPlayerPosition();
        isCheckingPlayerLocation = false;
        
        if (playerPingedPosition.x >= bossRoom.transform.position.x)
        {
            anim.SetBool("attackRight", true);
		}
        else
        {
            anim.SetBool("attackLeft", true);
        }
        isAttacking = false;
	}

    protected virtual GameObject InstantiateLimb(GameObject limbPrefab, float x, float y, 
        float rotation, bool left)
    {
        GameObject limb = Instantiate(limbPrefab, transform.position +
            new Vector3(x, y, 0.0f), Quaternion.Euler(0.0f, 0.0f, rotation), transform) as GameObject;
        if (left)
        {
            Vector3 currentScale = limb.transform.localScale;
            limb.transform.localScale = Vector3.Scale(currentScale, new Vector3(-1.0f, 1.0f, 1.0f)); 
        }
        return limb;
    }

	protected virtual void InstantiateTarget()
    {
		Vector3 playerPos = GameObject.Find("Player").transform.position;
		GameObject target = Instantiate(targetPrefab, playerPos, Quaternion.identity) as GameObject;
        target.name = "footTarget";
        target.transform.parent = targetParent.transform;
        targetTriggered = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player" && 
            !collision.gameObject.GetComponent<PlayerControl>().isInvincible)
        {
            collision.gameObject.GetComponent<Health>().DealDamage(1);
        }
    }

    public virtual void Update()
    {
        //Delegate/Event candidate
        if (isCheckingPlayerLocation)
        {
            playerPingedPosition = GetPlayerPosition();
            isCheckingPlayerLocation = false;
        }

        //Move to separate class specifically for tutorial boss phase 1
        GetMovementState();

        //Delegate/Event candidate. Move to separate class specifically for tutorial boss phase 1
        if (targetTriggered) { InstantiateTarget(); }

        //Delegate/Event candidate. Move to separate class specifically for tutorial boss phase 1
        if (isFiringProjectile)
        {
            if (anim.GetBool("attackRight"))
            {
                GameObject foot = InstantiateLimb(footPrefab, 1.294f, -0.421f, 32.381f, false);
                foot.name = "foot";
            }
            else
            {
                GameObject foot = InstantiateLimb(footPrefab, -1.286f, -0.427f, -32.625f, true);
                foot.name = "foot";
            }
            isFiringProjectile = false;
        }
    }
}
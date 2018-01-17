/* Author: Gabriel B. Gallagher Spetember 6th, 2017
 * 
 * Controls the 2nd phase of the tutorial boss encounter. Inherits the BossControl class, which will
 * eventually get split into BossControl and TutorialBossControl by refactoring the more generic boss code
 * into a BossControl class which can be inherited by every future boss, and refactoring the Tutorial Boss
 * specific code, such as limb rotation and instantiation, into a separate class that will inherit the
 * basic BossControl class.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoss_Phase_02 : BossControl
{
    public List<GameObject> limbPrefabs;

    public GameObject IndestructableTargetPrefab;

    public AudioClip deflectAudio;

    public float timeBetweenProjectiles, currentTime, startTime;

    public int limbCount, targetCount, vulnerableAttack, retractedLimbCount;

    /* Gets the position of the boss after when phase 2 is instantiated in order to make sure that it
     * moves in the right direction and is in the proper animation state when it begins interacting with
     * the player character.
     */
    void GetAwakePosition()
    {
        int position = Mathf.RoundToInt(transform.localPosition.y);

        switch(position)
        {
            case 3: //Top
                isTop = true;
                break;

            case 0: //Center
                isCenter = true;
                break;

            case -3: //Bottom
                isBot = true;
                break;

            default: //If it gets to default, there was an error in the rounding
                Debug.Log("There was an error with the position calculation");
                break;
        }
    }

    /* Overrides the Start method from BossControl.cs to prevent the Boss from moving at the start of 
     * phase 2 and to make sure the the position information is properly set before the boss begins 
     * interacting with the player character.
     */
    public override void Start()
    {
        base.Start();
        base.anim.GetComponent<AnimationControl_TutorialBoss>().SetBoolsFalse();
        limbCount = 0;
        targetCount = 0;
        retractedLimbCount = 0;
        isMoving = false;
        SetPositionBoolsFalse();
        anim.SetBool("moveDown", false);
        anim.Play("Idle");
        GetAwakePosition();
    }

    protected override void Attack()
    {
        vulnerableAttack = Random.Range(1, 4);
        base.Attack();
    }

    protected override void InstantiateTarget()
    {
        GameObject target;
        Vector3 playerPos = GameObject.Find("Player").transform.position;

        if (targetCount == vulnerableAttack)
        {
            target = Instantiate(IndestructableTargetPrefab, playerPos, Quaternion.identity)
                as GameObject;
            NameTarget(target);
        }
        else
        {
            target = Instantiate(targetPrefab, playerPos, Quaternion.identity) as GameObject;
            NameTarget(target);
        }

        target.transform.parent = targetParent.transform;
        targetCount += 1;
        targetTriggered = false;
    }

    void NameTarget(GameObject target)
    {
        switch (targetCount)
        {
            case 0: //target that's supposed to get hit by the foot
                target.name = "footTarget";
                break;

            case 1: //target that's supposed to get hit by the left hand
                target.name = "leftHandTarget";
                break;

            case 2: //target that's supposed to get hit by the right hand
                target.name = "rightHandTarget";
                break;

            case 3: //target that's supposed to get hit by the back hand
                target.name = "backHandTarget";
                break;

            default: //Error
                Debug.Log("Target count " + targetCount + " is out of range");
                break;
        }
    }

    protected override GameObject InstantiateLimb(GameObject limbPrefab, float x, float y, float rotation,
        bool left)
    {
        limbCount += 1;
        return base.InstantiateLimb(limbPrefab, x, y, rotation, left);
    }

    public override void Update()
    {
        currentTime = Time.time;
        //Delegate/Event candidate
        if (isCheckingPlayerLocation)
        {
            playerPingedPosition = base.GetPlayerPosition();
            isCheckingPlayerLocation = false;
        }

        GetMovementState();

        //Delegate/Event candidate
        if (targetTriggered) { InstantiateTarget(); }

        //Delegate/Event candidate
        if (isFiringProjectile)
        {
            if (anim.GetBool("attackRight"))
            {
                switch (limbCount)
                {
                    case 0:
                        GameObject foot = InstantiateLimb(limbPrefabs[limbCount], 1.294f, -0.421f, 
                            32.381f, false);
                        foot.name = "foot";
                        break;

                    case 1:
                        GameObject leftHand = InstantiateLimb(limbPrefabs[limbCount], 1.318164f, 
                            0.8326443f, 26.455f, false);
                        leftHand.name = "leftHand";
                        break;

                    case 2:
                        GameObject rightHand = InstantiateLimb(limbPrefabs[limbCount], -1.805f, -0.973f,
                            42.642f, false);
                        rightHand.name = "rightHand";
                        break;

                    case 3:
                        GameObject backHand = InstantiateLimb(limbPrefabs[limbCount], -1.07f, 1.147638f,
                            30.865f, false);
                        backHand.name = "backHand";
                        break;

                    default:
                        Debug.Log("Error Instantiating Limb Prefab");
                        break;
                }
            }
            else //Attack left
            {
                switch (limbCount)
                {
                    case 0:
                        GameObject foot = InstantiateLimb(limbPrefabs[limbCount], -1.286f, -0.427f, 
                            -32.625f, true);
                        foot.name = "foot";
                        break;

                    case 1:
                        GameObject leftHand = InstantiateLimb(limbPrefabs[limbCount], 1.83f, -1.077f,
                            -36.13f, false);
                        leftHand.name = "leftHand";
                        break;

                    case 2:
                        GameObject rightHand = InstantiateLimb(limbPrefabs[limbCount], -1.21f, 0.864f,
                            -19.943f, false);
                        rightHand.name = "rightHand";
                        break;

                    case 3:
                        GameObject backHand = InstantiateLimb(limbPrefabs[limbCount], 1.011f, 1.189f,
                            -31.72f, false);
                        backHand.name = "backHand";
                        break;

                    default:
                        Debug.Log("Error Instantiating Limb Prefab");
                        break;
                }
            }
            isFiringProjectile = false;
        }
    }
}

/* Author: Gabriel B. Gallagher January 28th 2017
 * 
 * Class is attached to each of the Tutorial Boss' limbs. Contains the boss' attack mechanics
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_TutorialBoss : MonoBehaviour
{
    public Animator anim;

    public GameObject destructableBlockParent, targetsParent;

    public float attackSpeed;

    public bool isExtending;

    new AudioSource audio;

    GameObject body;

    Vector3 playerPos, currentPos, startingPos, targetPos;

    int count;

	void Start()
    {
        body = transform.parent.gameObject;
        anim = body.transform.Find("Animation").GetComponent<Animator>();
		startingPos = transform.position;
		targetsParent = GameObject.Find("Targets");

        for (int i = 0; i < targetsParent.transform.childCount; i++)
        {
            Transform child = targetsParent.transform.GetChild(i);
            /* Gets the substring count by subtracting the number of characters in the string 'Target' (6)
             * from the total number of characters in the childName string.
             */
            int subString = child.name.Length - 6;

            if (child.name.Substring(0, subString) == name)
            {
                targetPos = child.position;
                Vector3 targetLocalPosition = child.localPosition;
                Debug.Log(child.name + " Position: " + targetLocalPosition);
            }
        }
        //GameObject.Find("Target(Clone)").transform.name = "Old Target";
		isExtending = true;

        if (body.name == "TutorialBoss_Phase_02(Clone)")
        {
            count = body.GetComponent<TutorialBoss_Phase_02>().limbCount;
        }
    }

	void Extend()
    {
		transform.position = Vector3.Lerp(currentPos, targetPos, attackSpeed);
    }

    private void SetAttackToIdleAnimation()
    {
        if (anim.GetBool("attackRight"))
        {
            anim.SetBool("attackRight", false);
            anim.SetBool("rightBackToIdle", true);
        }

        if (anim.GetBool("attackLeft"))
        {
            anim.SetBool("attackLeft", false);
            anim.SetBool("leftBackToIdle", true);
        }
        Destroy(gameObject);
    }

    void Retract()
    {
		transform.position = Vector3.Lerp(currentPos, startingPos, attackSpeed);

        if(body.name == "TutorialBoss_Phase_02(Clone)")
        {
            if (Vector3.Distance(startingPos, currentPos) < 0.1f && count ==
                        body.GetComponent<TutorialBoss_Phase_02>().limbPrefabs.Count)
            {
                body.GetComponent<TutorialBoss_Phase_02>().retractedLimbCount += 1;
                if (body.GetComponent<TutorialBoss_Phase_02>().retractedLimbCount >= 3 &&
                    !anim.GetBool("isDamaged"))
                {
                    //Find Boss Room Destructable Block parent and use the attached script to spawn blocks
                    destructableBlockParent = GameObject.Find("Room_Boss").transform.Find(
                        "Destructable_Blocks").gameObject;
                    destructableBlockParent.GetComponent<Tutorial_Boss_Room_Destructable_Blocks>().
                       SpawnIndestructableBlocks(
                       GameObject.Find("TutorialBoss_Phase_02(Clone)").GetComponent<Health>().health);
                }
                SetAttackToIdleAnimation();
            }
            else if (Vector3.Distance(startingPos, currentPos) < 0.1f)
            {
                body.GetComponent<TutorialBoss_Phase_02>().retractedLimbCount += 1;
                Destroy(gameObject);
            }
        }

		else if(Vector3.Distance(startingPos, currentPos) < 0.1f)
        {
            SetAttackToIdleAnimation();
        }
	}

	void OnTriggerEnter2D(Collider2D trigger)
    {
        switch (trigger.tag)
        {
            case "Target":
                Destroy(trigger.gameObject);
                isExtending = false;
                break;

            case "Player":
                trigger.gameObject.GetComponent<Health>().DealDamage(1);
                break;

            case "Bomb":
                //Retract the attacking limb
                isExtending = false;

                //Destroy all existing targets
                GameObject targets = GameObject.Find("Targets");
                for (int i = targets.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(targets.transform.GetChild(i).gameObject);
                }

                //Destroy the bomb
                trigger.gameObject.GetComponent<BombControl>().Explode();

                //Deal damage
                /* When the boss goes from 1 to 0 health and is destroyed, the audio source is 
                 * also destroyed before it gets a chance to finish playing. This is fine, for 
                 * now, since a different sound will be played when the boss transitions to a 
                 * new phase or gets killed. The new sound will be triggered through the death 
                 * animation.
                 */
                if (body.GetComponent<BossHealth>())
                {
                    anim.SetBool("isDamaged", true);
                    AudioSource audioSource = body.GetComponent<AudioSource>();
                    audioSource.PlayOneShot(body.GetComponent<BossControl>().damageAudio, 1.0f);
                    body.GetComponent<BossHealth>().DealDamage(1);
                }
                /* Boss can only take damage if it collides with a bomb placed on the randomly selected
                 * vulnerable target. This block determines which one that is by counting the number of
                 * limbs that have been instantiated and comparing that number to the randomly generated
                 * vulnerable attack.
                 */
                else
                {
                    int limbCount = body.GetComponent<TutorialBoss_Phase_02>().limbCount;
                    int vulnerableAttack = body.GetComponent<TutorialBoss_Phase_02>().vulnerableAttack;
                    if (limbCount - 1 == vulnerableAttack)
                    {
                        anim.SetBool("isDamaged", true);
                        AudioSource audioSource = body.GetComponent<AudioSource>();
                        audioSource.PlayOneShot(body.GetComponent<BossControl>().damageAudio, 1.0f);
                        body.GetComponent<Health>().DealDamage(1);
                    }
                    else
                    {
                        AudioSource audioSource = body.GetComponent<AudioSource>();
                        audioSource.PlayOneShot(body.GetComponent<TutorialBoss_Phase_02>().deflectAudio, 
                            1.0f);
                        destructableBlockParent = GameObject.Find("Room_Boss").transform.Find(
                            "Destructable_Blocks").gameObject;
                         destructableBlockParent.GetComponent<Tutorial_Boss_Room_Destructable_Blocks>().
                            SpawnDestructableBlocks(
                            GameObject.Find("TutorialBoss_Phase_02(Clone)").GetComponent<Health>().health);
                        SetAttackToIdleAnimation();
                    }
                }
                break;

            default:
                Debug.Log(trigger.name + " does not effect movement");
                break;
        }
	}

    void Update()
    {
		currentPos = transform.position;

		if(isExtending)
        {
			Extend();
		}
        else
        {
			Retract();
		}
	}
}
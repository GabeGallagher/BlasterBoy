/* Author: Gabriel B. Gallagher January 28th 2017
 * 
 * Animation controller for the tutorial boss. Communicates with the animator and manages the animation
 * states.
 */

using UnityEngine;

public class AnimationControl_TutorialBoss : MonoBehaviour
{
    void Attack()
    {
        transform.parent.GetComponent<BossControl>().isFiringProjectile = true;
	}

	void InstantiateTarget()
    {
        transform.parent.GetComponent<BossControl>().targetTriggered = true;
	}

    void ResetCount()
    {
        transform.parent.GetComponent<TutorialBoss_Phase_02>().limbCount = 0;
        transform.parent.GetComponent<TutorialBoss_Phase_02>().targetCount = 0;
        transform.parent.GetComponent<TutorialBoss_Phase_02>().retractedLimbCount = 0;
    }

	void BeginMoving()
    {
        transform.parent.GetComponent<BossControl>().isCheckingPlayerLocation = true;
        transform.parent.GetComponent<BossControl>().isMoving = true;
        GetComponent<Animator>().SetBool("isIdle", false);
    }

    void Idle()
    {
        GetComponent<Animator>().SetBool("isIdle", true);
    }

    public void SetBoolsFalse()
    {
		Animator anim = GetComponent<Animator>();
		anim.SetBool("hasMoved", false);
		anim.SetBool("rightBackToIdle", false);
		anim.SetBool("leftBackToIdle", false);
		anim.SetBool("attackRight", false);
		anim.SetBool("attackLeft", false);
        anim.SetBool("isIdle", false);
        anim.SetBool("isDamaged", false);
	}
}
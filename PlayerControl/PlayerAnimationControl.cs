/* Author: Gabriel B. Gallagher August 11th, 2016
 * 
 * Script which is attached to the Player Animator. Facilitates transitioning states in the animator.
 */

using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{
    //Turn off iFrames from the animator
    void Vulnerability()
    {
        transform.parent.GetComponent<PlayerControl>().isInvincible = false;
    }
}

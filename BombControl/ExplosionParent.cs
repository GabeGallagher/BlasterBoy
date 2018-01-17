/* Author: Gabriel B. Gallagher August 1st, 2017
 * 
 * Creates an empty parent game object to store the explosions. Helps to organize the explosions in the
 * hierarchy. Destroys itself after the child explosion objects expire.
 */

using UnityEngine;

public class ExplosionParent : MonoBehaviour
{
    float GetChildExplosionTime()
    {
        float explosionTime = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Explosion>().explosionTime > explosionTime)
            {
                explosionTime = transform.GetChild(i).GetComponent<Explosion>().explosionTime;
            }
        }
        return explosionTime;
    }
    private void Update()
    {
        Destroy(gameObject, (GetChildExplosionTime() + 0.01f));
    }
}

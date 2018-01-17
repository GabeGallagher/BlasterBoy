using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
	public int health;

	public virtual void DealDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
	}

	//public void DestroyObject()
    //{
	//	//This is for testing. Once death animations exist, remove the Destroy() command and just use the
	//	//animator to trigger the death animation event.
	//}
}
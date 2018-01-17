using UnityEngine;
using System.Collections;

public class DrawWireSphere : MonoBehaviour {
	public float size = 0.05f;
	//Draws circles on the enemy spawn position so the game designer can easily see and move the position
	void OnDrawGizmos () {
		Gizmos.DrawWireSphere(transform.position, size);
	}
}
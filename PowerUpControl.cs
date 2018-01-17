using UnityEngine;
using System.Collections;

public class PowerUpControl : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D trigger)
    {
		if(trigger.tag == "Player")
        {
            switch (tag)
            {
                case "BombLengthPowerUp":
                    //PlayerControl.bombRange += 1;
                    if (trigger.GetComponent<PlayerControl>().bombRange + 1 <=
                        trigger.GetComponent<PlayerControl>().bombMaxRange)
                    {
                        trigger.GetComponent<PlayerControl>().bombRange += 1;
                    }
                    GameObject.Find("GUICanvas").GetComponent<GUIScript>().ExplosionDistance();
                    Destroy(gameObject);
                    break;

                default:
                    Debug.Log("Power Up Type Not Found");
                    break;
            }
		}
	}
}

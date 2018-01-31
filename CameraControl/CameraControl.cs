using UnityEngine;

public class CameraControl : MonoBehaviour
{
	public GameObject player;

	public Transform highWall, rightWall, lowWall, leftWall;

    public bool isEnabled;

    //Change architecture to make these public variables
    public static float shakeTimer, shakeIntensity;

	float yMax, yMin, xMax, xMin;

    void Start()
    {
        GetEnabled();
    }

    private void GetEnabled()
    {
        if (isEnabled)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    //Change architecture to make this a public method
    public static void Shake()
    {
		shakeIntensity = 0.2f;
		shakeTimer = 0.8f;
	}
	
	void Update()
    {
		yMax = highWall.transform.position.y;
        xMax = rightWall.transform.position.x;
        yMin = lowWall.transform.position.y;
		xMin = leftWall.transform.position.x;

		//if within the bounds, camera locks onto player
		if(player.transform.position.y < yMax && player.transform.position.y > yMin)
        {
			transform.position = new Vector3(player.transform.position.x, player.transform.position.y,
                -110.0f);
		}

		//if player is above/below the y axis binders, camera locks to player on xAxis and stays stationary 
		//on yAxis
		if(player.transform.position.y > yMax)
        {
			transform.position = new Vector3(player.transform.position.x, yMax, -110.0f);
		}
        else if(player.transform.position.y < yMin)
        {
			transform.position = new Vector3(player.transform.position.x, yMin, -110.0f);
		}

        /* if player is right/left of the xAxis binders, camera locks to player on yAxis and stays
         * stationary on xAxis
        */
		if(player.transform.position.x > xMax)
        {
			transform.position = new Vector3(xMax, player.transform.position.y, -110.0f);
		}
        else if(player.transform.position.x < xMin)
        {
			transform.position = new Vector3(xMin, player.transform.position.y, -110.0f);
		}

		//if player is above the yAxis binder, and to the right of the xAxis, the camera stays stationary
		if(player.transform.position.y > yMax && player.transform.position.x > xMax)
        {
			transform.position = new Vector3(xMax, yMax, -110.0f);
		}
		//if player is above the yAxis binder, and to the left of the xAxis, the camera stays stationary
		if(player.transform.position.y > yMax && player.transform.position.x < xMin)
        {
			transform.position = new Vector3(xMin, yMax, -110.0f);
		}
		//if player is below the yAxis binder, and to the right of the xAxis, the camera stays stationary
		if(player.transform.position.y < yMin && player.transform.position.x > xMax)
        {
			transform.position = new Vector3(xMax, yMin, -110.0f);
		}
		//if player is below the yAxis binder, and to the left of the xAxis, the camera stays stationary
		if(player.transform.position.y < yMin && player.transform.position.x < xMin)
        {
			transform.position = new Vector3(xMin, yMin, -110.0f);
		}

		//Camera shake
		if(shakeTimer >= 0)
        {
			Vector3 shakePosition = Random.insideUnitCircle * shakeIntensity;
			transform.position = new Vector3(transform.position.x + shakePosition.x, 
                transform.position.y + shakePosition.y, transform.position.z);
			shakeTimer -= Time.deltaTime;
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyControlTester : MonoBehaviour {
	//Public Variables
	public LayerMask layerMask;

	public List<GameObject> rayPoints;
	
	public List<Ray2D> raysUp;
	public List<Ray2D> raysDown;
	public List<Ray2D> raysLeft;
	public List<Ray2D> raysRight;

	public List<int> possibleDirectionsList;

	public float speed = 1.0f;
	public float rayDistance = 0.5f;

	public int temp;

	public int direction = 1;
	public int up = 1;
	public int down = -1;
	public int right = 2;
	public int left = -2;

	public bool collisionUp = false;
	public bool collisionRight = false;
	public bool collisionDown = false;
	public bool collisionLeft = false;
	public bool checker = false;

	void GetRays() {
		//Get the object named Raycasting
		List<GameObject> children = gameObject.GetChildren();
		
		//Get the children inside Raycasting
		List<GameObject> children2 = new List<GameObject>();
		
		//Check inside raycasting object for the children (children are inside the 
		//raycasting folder)
		for (int i = 0; i < children.Count; i++) {
			if (children[i].name == "RayCasting") {
				children2 = children[i].GetChildren();
			}
		}
		
		for (int i = 0; i < children2.Count; i++) {
			rayPoints.Add(children2[i]);
		}
	}

	void Start() {
		rayPoints = new List<GameObject>();
		GetRays();
		possibleDirectionsList = new List<int>();
		Debug.Log(rayPoints.Count);
	}

	void AssignRaysToList() {
		raysUp = new List<Ray2D>();
		raysDown = new List<Ray2D>();
		raysLeft = new List<Ray2D>();
		raysRight = new List<Ray2D>();

		for (int i = 0; i < rayPoints.Count; i++) {

			//up
			if (rayPoints[i].gameObject.name == "up") {
				raysUp.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.up));
			}

			//down
			if (rayPoints[i].gameObject.name == "down") {
				raysDown.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.down));
			}

			//right
			if (rayPoints[i].gameObject.name == "right") {
				raysRight.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.right));
			}

			//left
			if (rayPoints[i].gameObject.name == "left") {
				raysLeft.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.left));
			}
		}
	}

	void AddDirectionToList() {

		if (!collisionUp) {
			possibleDirectionsList.Add(up);
		} 

		if (!collisionRight) {
			possibleDirectionsList.Add(right);
		}

		if (!collisionDown) {
			possibleDirectionsList.Add(down);
		}

		if (!collisionLeft) {
			possibleDirectionsList.Add(left);
		}
	}

	bool IsCollision (List<Ray2D> rayList) {
		for (int i = 0; i < rayList.Count; i++) {
			RaycastHit2D hit = Physics2D.Raycast(rayList[i].origin, rayList[i].direction, 
				rayDistance + .001f, layerMask);

			if (hit.collider) {
				return true;
			}
		}
		return false;
	}

	void RandomDirection() {
		temp = Random.Range(0, possibleDirectionsList.Count);
		direction = possibleDirectionsList[temp];
	}

	void CheckCollision() {
		AssignRaysToList();

		collisionDown = IsCollision(raysDown);
	
		collisionUp = IsCollision(raysUp);

		collisionLeft = IsCollision(raysLeft);
	
		collisionRight = IsCollision(raysRight);

		AddDirectionToList();

		if (checker == false) {
			RandomDirection();
			possibleDirectionsList.Clear();

		} else {
			if (possibleDirectionsList.Count > 1) {
				for (int i = 0; i < possibleDirectionsList.Count; i++) {
				/*
				* The following line of code looks at the direction int and the int of 
				* possibleDirectionsList at element i. If the two ints are opposite each other, or
				* are opposite directions, in other words, the int at element i will be removed so
				* that the character cannot move in the direction from where it just came.
				*/
					if (direction + possibleDirectionsList[i] == 0) {

						if (possibleDirectionsList[i] == up) {
							possibleDirectionsList.Remove(up);

						} else if (possibleDirectionsList[i] == down) {
							possibleDirectionsList.Remove(down);

						} else if (possibleDirectionsList[i] == right) {
							possibleDirectionsList.Remove(right);

						} else {
							possibleDirectionsList.Remove(left);
						}
					}
				}
				RandomDirection();
				possibleDirectionsList.Clear();

			} else {
				RandomDirection();
				possibleDirectionsList.Clear();
			}
		}
		checker = true;	
	}

	void Walk() {

		switch(direction) {
			//up
			case 1:
				transform.position += new Vector3(0.0f, speed * Time.deltaTime, 0.0f);
					break;
			//down
			case -1:
				transform.position += new Vector3(0.0f, -speed * Time.deltaTime, 0.0f);
				break;
			//right
			case 2:
				transform.position += new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
				break;
			//left
			case -2:
				transform.position += new Vector3(-speed * Time.deltaTime, 0.0f, 0.0f);
				break;
		}
	}	

	void DrawRaycast() {
		//draw all rays in list
		for (int i = 0; i < rayPoints.Count; i++) {
			
			//draw up
			if (rayPoints[i].gameObject.name == "up")
				Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3 
					(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y + rayDistance, 
					rayPoints[i].gameObject.transform.position.z), Color.green);
			
			//draw down
			if (rayPoints[i].gameObject.name == "down")
				Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3 
					(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y - rayDistance, 
					rayPoints[i].gameObject.transform.position.z), Color.green);
			
			//draw left
			if (rayPoints[i].gameObject.name == "left")
				Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3 
					(rayPoints[i].gameObject.transform.position.x - rayDistance, 
					rayPoints[i].gameObject.transform.position.y, 
					rayPoints[i].gameObject.transform.position.z), Color.green);
			
			//draw right
			if (rayPoints[i].gameObject.name == "right")
				Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3 
					(rayPoints[i].gameObject.transform.position.x + rayDistance, 
					rayPoints[i].gameObject.transform.position.y, 
					rayPoints[i].gameObject.transform.position.z), Color.green);
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.GetComponent<CheckPoint>()) {
			CheckCollision();
			Debug.Log("Checking Collisions");

		} else if (collider.GetComponent<Explosion>()) {
			Debug.Log("Hit " + collider.name);
		}
	}

	void Update() {
		Walk();
		DrawRaycast();
	}
}
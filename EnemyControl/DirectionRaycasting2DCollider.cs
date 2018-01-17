using UnityEngine;
using System.Collections;
using System.Collections.Generic;

	/*
	* This code is VERY IMPORTANT. It is the backup script for the enemy control. When I inevitably screw with
	* EnemyControl.cs and break, this code is my savior. DO NOT FUCK WITH IT!! Thank you.
	*/
public class DirectionRaycasting2DCollider : MonoBehaviour {
	//Public Variables
	public float speed = 1.0f;
	public float rayDistance = 0.5f;

	//Private Variables
	//the ray that hit something
	private RaycastHit2D hit;
	
	//raycast related
	private List<GameObject> rayPoints;
	private List<Ray2D> rays;
	
	private List<Ray2D> raysUp;
	private List<Ray2D> raysDown;
	private List<Ray2D> raysLeft;
	private List<Ray2D> raysRight;

	private List<int> possibleDirectionsList = new List<int>();

	private int direction = 9;
	private int up = 1;
	private int down = -1;
	private int right = 2;
	private int left = -2;

	private int temp;

	private bool collisionUp = false;
	private bool collisionDown = false;
	private bool collisionLeft = false;
	private bool collisionRight = false;
	private bool checker = false;

	void GetRays() {
		//Get the object named Raycasting
		List<GameObject> children = gameObject.GetChildren();
		
		//Get the children inside Raycasting
		List<GameObject> children2 = new List<GameObject>();
		
		//cCheck inside raycasting object for the children (children are inside the raycasting folder)
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
		rayPoints = new List<GameObject> ();
		GetRays();
	}

	void AddDrirectionToList() {
		if (!collisionDown) {
			possibleDirectionsList.Add(down);
		} 

		if (!collisionUp) {
			possibleDirectionsList.Add(up);
		}

		if (!collisionRight) {
			possibleDirectionsList.Add(right);
		}

		if (!collisionLeft) {
			possibleDirectionsList.Add(left);
		}
	}

	void RandomDirection() {
		temp = Random.Range(0, possibleDirectionsList.Count);
		direction = possibleDirectionsList[temp];
	}
	
	public void CheckCollision() {
		List<Ray2D> raysUp = new List<Ray2D>();
		List<Ray2D> raysDown = new List<Ray2D>();
		List<Ray2D> raysLeft = new List<Ray2D>();
		List<Ray2D> raysRight = new List<Ray2D>();

		//assign rays to list
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


			//left
			if (rayPoints[i].gameObject.name == "left") {
				raysLeft.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.left));
			}

			//right
			if (rayPoints[i].gameObject.name == "right") {
				raysRight.Add(new Ray2D(new Vector2(rayPoints[i].gameObject.transform.position.x, 
					rayPoints[i].gameObject.transform.position.y), Vector2.right));
			}
		}

		collisionDown = CheckCollision(raysDown);
	
		collisionUp = CheckCollision(raysUp);

		collisionLeft = CheckCollision(raysLeft);
	
		collisionRight = CheckCollision(raysRight);

		AddDrirectionToList();

		if (checker == false) {
			RandomDirection();
			possibleDirectionsList.Clear();

		} else {
			if (possibleDirectionsList.Count > 1) {
				for (int i = 0; i < possibleDirectionsList.Count; i++) {
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

	public void Walk() {

		switch(direction) {
			case 1:
				transform.position += new Vector3(0.0f, speed * Time.deltaTime, 0.0f);
					break;
			case -1:
				transform.position += new Vector3(0.0f, -speed * Time.deltaTime, 0.0f);
				break;
			case 2:
				transform.position += new Vector3(speed * Time.deltaTime, 0.0f, 0.0f);
				break;
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
					(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y + 
						rayDistance, rayPoints[i].gameObject.transform.position.z), Color.green);
			
			//draw down
			if (rayPoints[i].gameObject.name == "down")
				Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3 
					(rayPoints[i].gameObject.transform.position.x, rayPoints[i].gameObject.transform.position.y - 
					rayDistance, rayPoints[i].gameObject.transform.position.z), Color.green);
			
			//draw left
			if (rayPoints[i].gameObject.name == "left")
				Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3 
					(rayPoints[i].gameObject.transform.position.x - rayDistance, 
					rayPoints[i].gameObject.transform.position.y, rayPoints[i].gameObject.transform.position.z), 
					Color.green);
			
			//draw right
			if (rayPoints[i].gameObject.name == "right")
				Debug.DrawLine(rayPoints[i].gameObject.transform.position, new Vector3 
					(rayPoints[i].gameObject.transform.position.x + rayDistance, 
					rayPoints[i].gameObject.transform.position.y, rayPoints[i].gameObject.transform.position.z), 
					Color.green);
		}
	}

	bool CheckCollision(List<Ray2D> rayList) {
		for (int i = 0; i < rayList.Count; i++) {
			hit = Physics2D.Raycast(rayList[i].origin, rayList[i].direction, rayDistance + .001f);

			if (hit.collider) {
				return true;
			}
		}
		return false;
	}

	/*void OnTriggerEnter2D(Collider2D collider) {
		Debug.Log ("I'm GUMBot and I'm Triggered");
		CheckCollision();
	}*/

	void Update() {
		Walk();
		DrawRaycast();
	}

	void OnTriggerEnter(Collider other) {
		CheckCollision();
	}
}
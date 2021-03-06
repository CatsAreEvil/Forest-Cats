﻿using UnityEngine;
using System.Collections;

public class Cat_Control : MonoBehaviour {
	private GameObject managerObject;
	public Cat_Sprite_Control spriteControl; 
	public Rigidbody2D physics;
	private TextMesh text;

	//Control variables
	[HideInInspector] public bool goRight;
	[HideInInspector] public bool goLeft;
	[HideInInspector] public bool goUp;
	[HideInInspector] public bool goDown;
	[HideInInspector] public bool goShift;
	[HideInInspector] public bool goCtrl;
	[HideInInspector] public bool sit;
	
	//Physics variables
	private float walkSpeed = 1.5f;
	private float runBoost = 1.5f;
	private float speed = 0;
	private float climb = 1;
	private bool onGround = false;
	private Vector2 jump = new Vector2(1f,2.5f);
	private int counter = 0;
	private float xPos;
	private float yPos;
	public bool onTree;
	public bool leftOfTree;
	//
	public string catName;
	public string alligence;
	public float furRed;
	public float furGreen;
	public float furBlue;
	
	public Globals.RANK rank;
	public int age; //Moons

	private 
	// Use this for initialization
	void Start () {
		managerObject = GameObject.FindGameObjectWithTag("Manager");
		physics = GetComponent<Rigidbody2D> ();
		text = GetComponent<TextMesh> ();
		text.text = catName;
		age = 6;
	}

	// Update is called once per frame
	void FixedUpdate () {
		text.text = catName + "\n";//Hacky. Find a better way.
		xPos = physics.position.x;
		yPos = physics.position.y;
		//Control X movement
		if (goLeft) {
			
			if (goShift) {
				spriteControl.runLeft ();
				speed = -(walkSpeed + runBoost);
			} else {
				spriteControl.walkLeft ();
				speed = -walkSpeed;
			}
		} 
		else if (goRight) {
			
			if (goShift) {
				spriteControl.runRight ();
				speed = (walkSpeed + runBoost);
			} else {
				spriteControl.walkRight ();
				speed = walkSpeed;
			}
		} 
		else {
			spriteControl.idle ();
			speed = 0;
		}

		//Control jumping/climbing
		if (!onTree){
			if(goUp){
				if (onGround){
					physics.AddForce(jump, ForceMode2D.Impulse);
				}
			}
		}
		else{// on a tree
			if (goUp){
				climb = 1.4f;
			}
			else if (!onGround){
				if (goDown){
					climb = -1f;
				}
				else{// hold position
					climb = 0.4f;
				}
			}
		}
		if (onTree && !onGround){//climbing sprite control
			if (leftOfTree){
				if (physics.velocity.y > 0){
					spriteControl.overrideFlipX(true);
					spriteControl.rotateSprite(90);
					//spriteControl.walkRight();
				}
				else if (physics.velocity.y < 0){
					spriteControl.overrideFlipX(false);
					spriteControl.rotateSprite(90);
				}
			}
			else{
				if (physics.velocity.y > 0){
					spriteControl.overrideFlipX(false);
					spriteControl.rotateSprite(270);
				}
				else if (physics.velocity.y < 0){
					spriteControl.overrideFlipX(true);
					spriteControl.rotateSprite(270);
				}
			}
		}

		//
		if (sit){
			spriteControl.sit();
		}

		if (onTree){
			physics.velocity = new Vector2 (speed, climb);	
		}
		else{
			physics.velocity = new Vector2 (speed, physics.velocity.y);
		}
		
		counter++;
		if (counter >= 0){// Don't run as much.
			rotateToGround ();
			counter = 0;
		}
	}

	void OnTriggerEnter2D(Collider2D collided){
		UnityEngine.GameObject other;
		other = collided.gameObject;
	}


	void rotateToGround(){
		float offset = 0.17f;
		float yOffset = 0.12f;
		float threshold= 0.02f;
		float scale = 150f;
		float offsetScale = 0.004f;
		float leftRotate;
		float rightRotate;
		float spriteOffset;

		Vector2 leftPoint = new Vector2(xPos - offset, yPos - yOffset);
		RaycastHit2D leftHit = Physics2D.Raycast(leftPoint, -Vector2.up);
		leftRotate = leftHit.distance;
		bool left = leftRotate < threshold;
		
		Vector2 rightPoint = new Vector2(xPos + offset, yPos - yOffset);
		RaycastHit2D rightHit = Physics2D.Raycast(rightPoint, -Vector2.up);
		rightRotate = rightHit.distance;
		bool right = rightRotate < threshold;
		
		leftRotate = Mathf.Clamp(leftRotate * scale,0f,30f);
		
		rightRotate = Mathf.Clamp(rightRotate * scale,0f,30f); //Clamp to avoid extreme values, such as when hanging off a ledge
		
		onGround = true; //In 3/4 cases, set to false in fourth
		if (left && right){
			spriteOffset = 0f;
			spriteControl.rotateSprite(0);
			spriteControl.offsetSprite(spriteOffset, spriteOffset);
		}
		else if (!left && right){ //right is on ground
			spriteOffset = -leftRotate * offsetScale;
			spriteControl.rotateSprite(leftRotate);
			spriteControl.offsetSprite(0f, spriteOffset);
		}
		else if (left && !right){
			spriteOffset = -rightRotate * offsetScale;
			spriteControl.rotateSprite(-rightRotate);
			spriteControl.offsetSprite(0f, spriteOffset);
		}
		else if (!left && !right){
			spriteOffset = 0f;
			spriteControl.offsetSprite(spriteOffset, spriteOffset);
			onGround = false;
		}
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public class BuildTool : LaserHeldObject {

	public float previewScale = 0.15f;
	public float previewRotateSpeed = 100f;
	public float previewAlpha = 0.3f;
	public Material previewMat;

	Transform barrel;
	Transform previewArea;

	private int currentID ;
	public int CurrentID {
		get {
			return currentID;
		}
		set {
			if(value >= 0){
				currentID = value;

				StartPreviewBuildable ();
			}
		}
	}

	protected override void Start(){
		base.Start ();

		barrel = transform.Find ("EndOfBarrel");
		previewArea = transform.Find ("BuildPreview");

	}

	#region HeldObject
	GameCube lastPointedAt;
	protected override void HandAttachedUpdate (Hand hand){
		base.HandAttachedUpdate (hand);

		//Preview spin
		if (previewObj != null) {
			previewObj.transform.Rotate(new Vector3(0, Time.deltaTime * previewRotateSpeed, 0));
		}

		//Raycasting coms last as returning form the function is an option

		RaycastHit hitInfo;

		if (Physics.Raycast (barrel.transform.position, barrel.transform.forward, out hitInfo)) {
			if (hitInfo.collider != null) {
				//we hit a collider
				GameCube cube = hitInfo.collider.GetComponent<GameCube> ();

				if (cube != null) {

					if (lastPointedAt != cube) {
						if (lastPointedAt != null) {
							lastPointedAt.OnPointedAway ();
						}

						cube.OnPointedAt ();
					}

					lastPointedAt = cube;

					if (Input.GetKeyDown (BuildManager.Instance.buildKey)) {

						//hitInfo.collider.GetComponent<GameCube> ();
						cube.Occupying = Instantiate(BuildManager.Instance.buildables[currentID]);
					}

					return;
				} 
			}
		} 

		//missed cube tihs frame, but last frame was hitting a box
		if (lastPointedAt != null) {
			lastPointedAt.OnPointedAway ();

			lastPointedAt = null;
		}
	}

	int lastIDSelected = 0;
	protected override void OnAttachedToHand (Hand hand)
	{
		base.OnAttachedToHand (hand);

		this.CurrentID = lastIDSelected;
	}

	protected override void OnDetachedFromHand (Hand hand)
	{
		base.OnDetachedFromHand (hand);

		lastIDSelected = this.CurrentID;

		RemovePreviewBuild ();
	} 
	#endregion

	#region Building Functionality
	void SelectBuildable(){

		//temp code - this needs to be VR friendly!!

		if (Input.GetKeyDown (KeyCode.R)) {
			this.CurrentID = (currentID + 1) % BuildManager.Instance.buildables.Length;
		} 
	}

	GameObject previewObj = null;
	void StartPreviewBuildable(){

		if (previewObj != null) {
			RemovePreviewBuild();
		}


		previewObj = Instantiate (BuildManager.Instance.buildables[currentID], previewArea);

		previewObj.transform.localPosition = Vector3.zero;
		previewObj.transform.localRotation = Quaternion.identity;

		previewObj.transform.localScale = previewObj.transform.lossyScale *  previewScale;

		//preview obj shouldnt shot at enemies!
		previewObj.GetComponent<Turret> ().enabled = false;

		foreach (MeshRenderer mr in previewObj.GetComponentsInChildren<MeshRenderer>()) {
			mr.material = previewMat;
		}
	}

	void RemovePreviewBuild(){
		//animate object leaving, for now destroy
		Destroy (previewObj);
	}


	IEnumerator RotatePreview(){
		while (true) {
		}
	}

	#endregion
}

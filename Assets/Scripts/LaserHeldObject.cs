﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;

public abstract class LaserHeldObject : HeldObject {

	public string[] layerNamesToHit;

	LineRenderer lr;

	protected override void Start(){
		lr = GetComponentInChildren<LineRenderer> ();

		lr.useWorldSpace = true;
		lr.positionCount = 2;
		lr.enabled = false;

	}

	protected override void HandAttachedUpdate( Hand hand ){

		base.HandAttachedUpdate (hand);

		lr.SetPosition (0, lr.transform.position);

		RaycastHit hitInfo;

		if (Physics.Raycast (lr.transform.position, lr.transform.forward, out hitInfo, Mathf.Infinity, LayerMask.GetMask(layerNamesToHit))) {

			if (hitInfo.collider != null) {
				lr.SetPosition (1, hitInfo.point);
				return;
			}
		} 

		lr.SetPosition (1, transform.forward * 1000f);
	}

	protected override void OnAttachedToHand( Hand hand) {

		lr.enabled = true;

	}

	protected override void OnDetachedFromHand( Hand hand ){

		lr.enabled = false;
	}
}
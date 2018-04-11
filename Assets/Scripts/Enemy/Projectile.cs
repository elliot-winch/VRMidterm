﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

	public float speed = 1f;

	private float damage;

	string[] layersToHit;
	Transform rootParent;

	void Start(){
		Transform t = transform;
		while(t.parent != null){
			t = t.parent;
		}

		rootParent = t;
	}

	public void Launch(float damage, Vector3 position, GameObject dontCollideWith, string[] layersToHit = null){
		//there is a case where something was firing at an object that is destroyed before the projectile is launched
		this.damage = damage;

		this.transform.LookAt (position);

		this.layersToHit = layersToHit;

		Launch (dontCollideWith);

	}

	public void Launch(float damage, Transform inLineWith, GameObject dontCollideWith, string[] layersToHit = null)
	{
		this.damage = damage;

		this.transform.forward = inLineWith.forward;

		this.layersToHit = layersToHit;

		Launch(dontCollideWith);

	}

	private void Launch(GameObject dontCollideWith){

		foreach (Collider c in dontCollideWith.GetComponentsInChildren<Collider>()) {
			foreach (Collider mc in GetComponentsInChildren<Collider>()) {
				Physics.IgnoreCollision (c, mc);
			}
		}

		GetComponent<Rigidbody> ().velocity = transform.forward * speed;

		StartCoroutine (DestroyOnDelay (10f));
	}

	void OnCollisionEnter(Collision col){
		//we have defined layers and this isn't one of them
		Debug.Log("Projectile collides");
		Debug.Log ("Looking for layers: " + layersToHit [0] + " (only + " + layersToHit.Length + ") and this collider is in layer " + LayerMask.LayerToName (col.gameObject.layer));
		if (this.layersToHit != null && layersToHit.Contains(LayerMask.LayerToName(col.gameObject.layer)))
		{ 
			Debug.Log (LayerMask.LayerToName(col.gameObject.layer) + " " + layersToHit [0]);

			if (col.collider.GetComponentInParent<DamagableObject>() != null)
			{
				Vector3 hitDirection = transform.InverseTransformDirection (GetComponent<Rigidbody> ().velocity);
				col.collider.GetComponentInParent<DamagableObject>().Hit(col.contacts[0].point, hitDirection, damage);
			}
		}

		Destroy (rootParent.gameObject);
	}

	IEnumerator DestroyOnDelay(float delay){
		yield return new WaitForSeconds (delay);

		Destroy (rootParent.gameObject);
	}
}

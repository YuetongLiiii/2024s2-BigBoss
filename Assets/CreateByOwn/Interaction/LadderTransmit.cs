using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTransmit : MonoBehaviour
{
	private SphereCollider bottomCollider;
	private SphereCollider upCollider;
	public Transform upPos;
	public Transform bottomPos;
	private void Awake()
	{
		GameObject bottomGameObject = new GameObject();
		bottomGameObject.transform.SetParent(transform,false);
		bottomGameObject.name = "Ladder_bottomCollider";
		bottomCollider= bottomGameObject.AddComponent<SphereCollider>();
		bottomCollider.isTrigger = true;
		bottomCollider.transform.localPosition=Vector3.zero;
		bottomCollider.radius = 1f;
		
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		float height = meshCollider.bounds.size.y;
		
		GameObject upGameObject = new GameObject();
		upGameObject.transform.SetParent(transform,false);
		upGameObject.name = "Ladder_upCollider";
		upCollider= upGameObject.AddComponent<SphereCollider>();
		upCollider.isTrigger = true;
		upCollider.transform.localPosition=Vector3.up*height;
		upCollider.radius = 1f;
	}
}

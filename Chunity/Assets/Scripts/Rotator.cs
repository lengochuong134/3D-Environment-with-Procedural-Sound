using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public bool rotateRandomly = false;
	
	// Update is called once per frame
	void Update()
	{
		Vector3 rotateAmount;
		if (rotateRandomly)
		{
			rotateAmount = new Vector3(
				Random.Range(100, 800),
				Random.Range(100, 800),
				Random.Range(100, 800)

			);
		}
        else
        {
			rotateAmount = new Vector3(15, 30, 45);
        }
		transform.Rotate( rotateAmount* Time.deltaTime );
	}
}

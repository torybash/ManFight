using UnityEngine;
using System.Collections;

public class StartPosition : MonoBehaviour {

	public Color color;



	public void Init(Color clr){
		color = clr;

		GetComponent<Renderer>().material.color = color;
	}

}

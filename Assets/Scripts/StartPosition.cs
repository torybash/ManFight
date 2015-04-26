using UnityEngine;
using System.Collections;

public class StartPosition : MonoBehaviour {

	public Color color;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void Init(Color clr){
		color = clr;

		GetComponent<Renderer>().material.color = color;
	}

}

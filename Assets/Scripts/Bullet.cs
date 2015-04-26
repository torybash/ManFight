using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void Initialize(Vector2 velocity){
		GetComponent<Rigidbody2D>().velocity = velocity;
	}
}

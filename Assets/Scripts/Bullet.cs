using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {


	public void Initialize(Vector2 velocity){
		GetComponent<Rigidbody2D>().velocity = velocity;
	}
}

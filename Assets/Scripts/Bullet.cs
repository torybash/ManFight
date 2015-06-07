using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public int dmg;
	public int playerID;

	public void Init(int playerID, int roboID, int weaponDmg, float angle)
	{
		dmg = weaponDmg;
		this.playerID = playerID;

		float radAngle = Mathf.Deg2Rad * (angle + 90f);
		Vector2 vec = new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));

//		Debug.Log("playerID: " + playerID + ", angle: " + angle + ", vec: " + vec);

		GetComponent<Rigidbody2D>().velocity = vec * 3f;
	}


}

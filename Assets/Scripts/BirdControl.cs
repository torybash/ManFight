using UnityEngine;
using System.Collections;

public class BirdControl : MonoBehaviour {

	Vector2 velocity = new Vector2();

	Rigidbody2D rigiB;


	[SerializeField] float K_moveSpeed;
	[SerializeField] float K_jumpSpeed;
	[SerializeField] float K_jumpMaxDuration;
	[SerializeField] float K_shootCooldownDuration;
	[SerializeField] float K_gunRotationMax;
	[SerializeField] float K_gunRotationMin;
	[SerializeField] float K_gunRotationSpeed;
	[SerializeField] float K_bulletSpeed;

	[SerializeField] Transform shotPrefab;


	Transform gunTrans;
	Transform shootPos;

	
	float shootCooldown = 0;
	float gunRotation = 0;

	// Use this for initialization
	void Awake () {
		rigiB = GetComponent<Rigidbody2D>();


		foreach (Transform child in transform) {
			if (child.name.Equals("GunContainer")){
				gunTrans = child;

				foreach (Transform grandChild in child) {
					if (grandChild.name.Equals("Gun")){
						foreach (Transform grandGrandChild in grandChild) {
							if (grandGrandChild.name.Equals("ShootPos")){
								shootPos = grandGrandChild;
							}
						}
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		velocity = rigiB.velocity;

		Movement();

		Jumping();

		Aiming();

		Shooting();

		Anim();



		rigiB.velocity = velocity;
	}



	void Movement(){
		float xIn = Input.GetAxis("Horizontal");


		velocity.x = xIn * K_moveSpeed;

//		velocity.y -= 10f * Time.deltaTime;

//		transform.Translate(velocity * Time.deltaTime);




	}

	bool jumping = false;
	float jumpTimer = 0;

	void Jumping(){
		if (jumping) jumpTimer += Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Space) ){
			jumping = true;
			jumpTimer = 0;
//			rigiB.velocity = new Vector2(rigiB.velocity.x, 5f);
			velocity.y = K_jumpSpeed;
//			rigiB.AddForce(new Vector2(0, 40));
		}else if (Input.GetKey(KeyCode.Space) && jumping && jumpTimer < K_jumpMaxDuration){
//			rigiB.AddForce(new Vector2(0, 40));
//			rigiB.velocity = new Vector2(rigiB.velocity.x, 3f);
			velocity.y = K_jumpSpeed;
		}
	}


	void Aiming(){
		float aimIn = Input.GetAxis("Vertical");

		gunRotation -= aimIn * K_gunRotationSpeed * Time.deltaTime;

//		print ("gunRotation: " + gunRotation);
		if (gunRotation < K_gunRotationMin) gunRotation = K_gunRotationMin;
		if (gunRotation > K_gunRotationMax) gunRotation = K_gunRotationMax;

		gunTrans.rotation = Quaternion.AngleAxis(gunRotation, Vector3.forward);

//		gunTrans.localRotation.eulerAngles.Set(0, 0, gunRotation);

	}

	void Shooting(){
		shootCooldown -= Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.LeftShift) && shootCooldown < 0){
			Shoot();
			shootCooldown = K_shootCooldownDuration;
		}
	}

	void Shoot(){
		Bullet bullet = ((Transform) Instantiate(shotPrefab, shootPos.position, Quaternion.identity)).GetComponent<Bullet>();
//		gunTrans.rotation.
	
		float rev = -transform.localScale.x;
		Vector2 bulletVelocity = gunTrans.rotation * Vector3.forward * K_bulletSpeed * rev;
		Vector2 bulletVelocity2 = gunTrans.rotation * Vector3.right * K_bulletSpeed * rev;
		Vector2 bulletVelocity3 = gunTrans.rotation * Vector3.up * K_bulletSpeed * rev;

		bulletVelocity2.y *= -rev;
		//		Quaternion.AngleAxis(gunTrans.rotation.eulerAngles.z, 

		print ("bulletVelocity: " + bulletVelocity + ", gunTrans.rotation: " + gunTrans.rotation + ", bulletVelocity2: " + bulletVelocity2 + ", bulletVelocity3: " + bulletVelocity3);

		bullet.Initialize(bulletVelocity2);
	}


	void Anim(){
		if (velocity.x > 0){
			transform.localScale = new Vector3(-1, 1, 1);
		}else if (velocity.x < 0){
			transform.localScale = new Vector3(1, 1, 1);
		}
	}


	void OnCollsion(Collision coll){
		jumping = false;
	}
}

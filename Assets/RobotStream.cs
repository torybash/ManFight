using UnityEngine;
using System.Collections;

public class RobotStream : MonoBehaviour {


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 pos = Vector3.zero;
		if (stream.isWriting) {
			pos = transform.position;
			stream.Serialize(ref pos);
		} else {
			stream.Serialize(ref pos);
			transform.position = pos;
		}
	}
}

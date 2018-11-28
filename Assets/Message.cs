using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour {

	public Text text;

	string message;
	float st;
	float remaining_time = 3.0f;
	
	public void SetMessage(string message)
	{
		st = Time.time;
		this.message = message;

		Debug.Log(message);
	}

	void Update () {
		float diff = Time.time - st;
		if (diff <= remaining_time) {
			gameObject.SetActive(true);
			text.text = message;
		}
		else {
			gameObject.SetActive(false);
		}
	}
}

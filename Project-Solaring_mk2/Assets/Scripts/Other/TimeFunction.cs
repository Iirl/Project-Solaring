using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFunction : MonoBehaviour
{
	[SerializeField]
	KeyCode TimeSpeedKey;
	
	private void TimeSpeed(KeyCode input, float t = 3){
		if (Input.GetKey(input)) Time.timeScale = t;
		else Time.timeScale =1;
	}
	protected void Update()
	{
		TimeSpeed(TimeSpeedKey);
	}
}
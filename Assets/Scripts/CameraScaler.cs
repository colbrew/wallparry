using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
	public float borderAspectRatio = 1.5f;
	public float borderHalfHeight = 6.0f;

	int resWidth = 0;
	int resHeight = 0;

	void Start()
	{
		CheckResizing();
	}

	void Update()
	{
		CheckResizing();
	}

	void CheckResizing()
    {
		if(resWidth != Screen.width || resHeight != Screen.height)
		{
			resWidth = Screen.width;
			resHeight = Screen.height;

			var cam = this.GetComponent<Camera>();
			var aspectHxW = (float)resHeight / (float)resWidth; //1.0f / cam.aspect;
			if (aspectHxW > borderAspectRatio)
			{
				cam.orthographicSize = (aspectHxW / borderAspectRatio) * borderHalfHeight;
			}
			else
			{
				cam.orthographicSize = borderHalfHeight;
			}
		}
	}


}

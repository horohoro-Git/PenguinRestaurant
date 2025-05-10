using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseCamera : MonoBehaviour
{
    Camera cam;
    public Camera cachingCamera { get { if(cam == null) cam = Camera.main; return cam; } }



    public void ScreenShot(string path)
    {
        ScreenCapture.CaptureScreenshot(path);
    }
}

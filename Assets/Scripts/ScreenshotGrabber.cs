#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ScreenshotGrabber
{
    [MenuItem("Screenshot/Grab %g")]
    public static void Grab()
    {
        ScreenCapture.CaptureScreenshot("Assets/Screenshots/Screenshot" + GetCh() + GetCh() + GetCh() +  "_" + Screen.width+"x"+Screen.height+".png", 1);
    }
 
    public static char GetCh()
    {
        return (char)UnityEngine.Random.Range('A', 'Z');
    }
}
#endif
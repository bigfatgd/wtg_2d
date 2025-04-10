using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveStep = 2f;  // Stały krok przesuwania kamery
    public Camera mainCamera;

    public void MoveCameraRight()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position += new Vector3(moveStep, 0f, 0f);
        }
    }
    public void MoveCameraLeft()
{
    if (mainCamera != null)
    {
        mainCamera.transform.position += new Vector3(-moveStep, 0f, 0f);
    }
}

}

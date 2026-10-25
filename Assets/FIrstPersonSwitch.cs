
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;

    public float cameraHeight = 1.7f;

    public void SwitchToFirstPerson()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("Kamera nebyla nalezena!");
            return;
        }

        // Pøesun kamery na X=1, Z=1.
        mainCamera.transform.position = new Vector3(
            1f,
            cameraHeight,
            1f
        );

        // Pohled vodorovnì dopøedu.
        mainCamera.transform.rotation = Quaternion.identity;

        // Perspektivní pohled.
        mainCamera.orthographic = false;

        mainCamera.fieldOfView = 70f;
    }
}

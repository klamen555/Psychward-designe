
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{

    public float cameraHeight = 1.7f;
    public GameObject hrac;

    public void SwitchToFirstPerson()
    {
        hrac.SetActive(true);
    }

}

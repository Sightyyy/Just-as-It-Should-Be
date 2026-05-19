using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera realCam;
    public Camera innerCam;

    public void ActivateInner()
    {
        innerCam.rect = new Rect(0, 0, 1, 1);
        innerCam.gameObject.SetActive(true);

        realCam.rect = new Rect(0.02f, 0.7f, 0.25f, 0.25f);
        realCam.gameObject.SetActive(true);
    }

    public void ActivateReal()
    {
        realCam.rect = new Rect(0, 0, 1, 1);
        realCam.gameObject.SetActive(true);

        innerCam.gameObject.SetActive(false);
    }
}
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera realCam;
    [SerializeField] private Camera innerCam;
    [SerializeField] private Rect innerWorldRealCameraInset = new Rect(0.02f, 0.7f, 0.25f, 0.25f);

    public Camera RealCamera => realCam;
    public Camera InnerCamera => innerCam;

    public void ActivateInner()
    {
        innerCam.rect = new Rect(0, 0, 1, 1);
        innerCam.gameObject.SetActive(true);

        realCam.rect = innerWorldRealCameraInset;
        realCam.gameObject.SetActive(true);
    }

    public void ActivateReal()
    {
        realCam.rect = new Rect(0, 0, 1, 1);
        realCam.gameObject.SetActive(true);

        innerCam.gameObject.SetActive(false);
    }
}

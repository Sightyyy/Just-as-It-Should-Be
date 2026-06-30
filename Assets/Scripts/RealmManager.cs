using UnityEngine;

public class RealmManager : MonoBehaviour
{
    public PlayerMovement realPlayer;
    public PlayerMovement innerPlayer;
    [SerializeField] private CameraManager camManager;

    void Start()
    {
        ActivateReal();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ActivateInner();
        }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ActivateReal();
        }
    }

    void ActivateReal()
    {
        realPlayer.SetControl(true);
        innerPlayer.SetControl(false);
        camManager.ActivateReal();
    }

    void ActivateInner()
    {
        realPlayer.SetControl(false);
        innerPlayer.SetControl(true);
        camManager.ActivateInner();
    }
}
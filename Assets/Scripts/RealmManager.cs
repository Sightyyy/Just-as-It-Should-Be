using UnityEngine;

public class RealmManager : MonoBehaviour
{
    public enum Realm
    {
        Real,
        Inner
    }

    [SerializeField] private PlayerMovement realPlayer;
    [SerializeField] private PlayerMovement innerPlayer;
    [SerializeField] private CameraManager camManager;

    public Realm CurrentRealm { get; private set; }

    void Start()
    {
        ActivateReal();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ActivateRealm(Realm.Inner);
        }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ActivateRealm(Realm.Real);
        }
    }

    public void ActivateRealm(Realm realm)
    {
        if (realm == Realm.Real)
        {
            ActivateReal();
        }
        else
        {
            ActivateInner();
        }
    }

    public void ActivateReal()
    {
        CurrentRealm = Realm.Real;
        realPlayer.SetControl(true);
        innerPlayer.SetControl(false);
        camManager.ActivateReal();
    }

    public void ActivateInner()
    {
        CurrentRealm = Realm.Inner;
        realPlayer.SetControl(false);
        innerPlayer.SetControl(true);
        camManager.ActivateInner();
    }
}

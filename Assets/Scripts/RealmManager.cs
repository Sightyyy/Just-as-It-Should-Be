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
        if (CanSwitchRealm() == false) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateRealm(Realm.Inner);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateRealm(Realm.Real);
        }
    }

    public void ActivateRealm(Realm realm)
    {
        if (realm == CurrentRealm) return;
        if (CanSwitchRealm() == false) return;

        if (realm == Realm.Real)
        {
            ActivateReal();
        }
        else
        {
            ActivateInner();
        }
    }

    private static bool CanSwitchRealm()
    {
        return !GameplayInputLock.IsLocked
            && !TypingMiniGameController.IsAnyMinigameActive
            && !PatchingUpMinigameController.IsAnyPatchingActive;
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

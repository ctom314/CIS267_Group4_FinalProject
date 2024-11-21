using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonHandler : MonoBehaviour
{
    private PauseManager pm;
    public GameObject cursorObj;
    public LayerMask interactiveLayer;
    public PersistentData pd;
    private PlayerControls controls;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PauseManager>();
    }

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0) || controls.Player.rightTrigger.triggered)
        {
            clickAction();
        }
    }

    public void resumeGame()
    {
        pm.togglePause();
    }

    public void returnToMainMenu()
    {
        // Load main menu scene
        // TODO: Make main menu scene
    }


    private void clickAction()
    {
        Vector3 cursorPos = cursorObj.transform.position;
        CircleCollider2D cursorCollider = cursorObj.GetComponent<CircleCollider2D>();
        Collider2D tester = Physics2D.OverlapCircle(cursorPos, cursorCollider.radius*2, interactiveLayer);

        if(tester != null)
        {
            if(tester.GetComponent<fieldData>() != null)
            {
                fieldData fd = tester.GetComponent<fieldData>();

                //if crops are ready to harvest, harvest them
                if(fd.isPlanted && pd.getDay() >= fd.dayPlanted + fd.cropInfo.growTime)
                {
                    tester.GetComponent<SpriteRenderer>().color = Color.yellow;
                    Debug.Log("You got " + fd.cropInfo.harvestAmount + " " +fd.cropInfo.cropName);
                }
            }
        }
    }
}

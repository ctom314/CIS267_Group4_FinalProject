using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameButtonHandler : MonoBehaviour
{
    public GameObject cursorObj;
    public LayerMask interactiveLayer;
    public PersistentData pd;

    // UI : Controls Page
    public GameObject controlsMenu;
    public GameObject controlsFirstButton;
    public GameObject controlsClosedButton;

    private PauseManager pm;
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
        if(Input.GetMouseButtonDown(0) || controls.Player.rightTrigger.triggered && !pm.isPaused)
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
        SceneManager.LoadScene("MainMenu");
    }

    // Controls Page
    public void showControlsMenu()
    {
        PauseManager.canPause = false;

        // Hide pause menu
        pm.pauseMenu.SetActive(false);

        // Show controls menu
        controlsMenu.SetActive(true);

        // Setup controls first button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsFirstButton);
    }

    public void hideControlsMenu()
    {
        PauseManager.canPause = true;

        // Hide controls menu
        controlsMenu.SetActive(false);

        // Show pause menu
        pm.pauseMenu.SetActive(true);

        // Setup controls closed button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsClosedButton);
    }

    private void clickAction()
    {
        Vector3 cursorPos = cursorObj.transform.position;
        CircleCollider2D cursorCollider = cursorObj.GetComponent<CircleCollider2D>();
        Collider2D tester = Physics2D.OverlapCircle(cursorPos, cursorCollider.radius*2, interactiveLayer);

        if(tester != null)
        {
            if(tester.gameObject.GetComponent<ObjectData>() != null)
            {
                tester.gameObject.GetComponent<ObjectData>().runLogic();
            }
        }

        if (tester != null)
        {
            Debug.Log("Object detected: " + tester.gameObject.name);
            if (tester.gameObject.GetComponent<ObjectData>() != null)
            {
                tester.gameObject.GetComponent<ObjectData>().runLogic();
            }
        }

    }
}

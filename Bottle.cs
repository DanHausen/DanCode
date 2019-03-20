using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Bottle))]
public class Bottle : MonoBehaviour
{
    private Vector2     _startSwiping;
    private Vector2     _endSwiping;
    private Vector2     _swipeEndVector;

    [SerializeField]
    [Tooltip("This private field is created to control the turns for the bottle object rotation")]
    private bool        _torqueBool = false;

    [Header("Set the amount of force to be applied")]
    public  float       forceToAdd = 5f;

    [SerializeField]
    [Header("Set the timer to get the point on table")]
    private float timerToPoint = 0.175f;

    [Tooltip("This private field shows the rotation value applied to each swip the player does")]
    private float       _torqueRotate;
    private float       _touchTimeStarted, touchTimeEnded, touchTimeDifference;

    int scorePoint = 10;
    bool onTable = false;


    private Rigidbody   _rb;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    private void Update()
    {
        if (onTable && transform.eulerAngles.z >= -0.5f && transform.eulerAngles.z <= 0.5f) //For when the bottle is stable on the table, it will call the PointBasedOnRotation() Method
        {
            PointBasedOnRotation();
        }
        
        else if (onTable && (transform.eulerAngles.z <= -15f || transform.eulerAngles.z >= 15f))
        {
            RespawnSequence();
        }
    }

    private void FixedUpdate()
    {        
        TouchInputAndGetInput();
    }

    /// <summary>
    /// This Method is created to change the values of the private bool value of torque, for than change its torque rotation direction
    /// </summary>
    void BoolCHanger()
    {
        _torqueBool = !_torqueBool ? true : false;
    }

    /// <summary>
    /// Method to get and use input from the player, on the screen and with the mouse
    /// </summary>
    void TouchInputAndGetInput()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0)) //To get input from touch and mouse
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            _startSwiping = Camera.main.ScreenToViewportPoint(Input.mousePosition);
#else
            _startSwiping = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position);

#endif
            _touchTimeStarted = Time.time;
        }
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0)) //To release input from touch and mouse
        {
            _rb.isKinematic = false;

#if UNITY_EDITOR || UNITY_STANDALONE
            _endSwiping = Camera.main.ScreenToViewportPoint(Input.mousePosition);
#else
            _endSwiping = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position);

#endif
            BoolCHanger();

            _torqueRotate = (_torqueBool) ? -10 : 10; //Created so that in each swipe, the bottle will flip to a different side

            touchTimeEnded = Time.time;
            _swipeEndVector = (((_endSwiping - _startSwiping) / (touchTimeEnded - _touchTimeStarted)) * forceToAdd); //Dividided the position vector by the time vector to get a more precise point, and then already applied the designer controlled force

            _rb.AddForce(0, _swipeEndVector.y,0, ForceMode.Impulse);
            _rb.AddTorque(0, 0, _torqueRotate, ForceMode.Impulse); //To rotate the bottle in the air

        }
    }

    /// <summary>
    /// Method to send points to the manager, and manage a secure timer to give player the points so that the developers is sure that the player earned its points.
    /// </summary>
    void PointBasedOnRotation()
    {
        float _timerToPoint = timerToPoint;
        _timerToPoint -= Time.deltaTime * 9;

        if(_timerToPoint <= 0)
        {
            ProjectBottle.AddToScore(scorePoint);
            _timerToPoint = timerToPoint;
            onTable = false;
        }
    }

    /// <summary>
    /// Method to quickly reset scene when fails to land object
    /// </summary>
    private void RespawnSequence()
    {
        float _timerToRespawn = timerToPoint;
        _timerToRespawn -= Time.deltaTime * 8;

        if (_timerToRespawn <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Table")
        {
            onTable = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Table")
        {
            onTable = false;
        }
    }

}


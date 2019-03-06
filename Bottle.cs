using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Tooltip("This private field shows the rotation value applied to each swip the player does")]
    private float       _torqueRotate;
    private float       _touchTimeStarted, touchTimeEnded, touchTimeDifference;

    private Rigidbody   _rb;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        TouchInputAndGetInput();
    }

    void BoolCHanger()
    {
        if (!_torqueBool)
            _torqueBool = true;
        else if (_torqueBool)
            _torqueBool = false;
    }

    void TouchInputAndGetInput()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            _startSwiping = Camera.main.ScreenToViewportPoint(Input.mousePosition);
#else
            startSwiping = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position);

#endif
            _touchTimeStarted = Time.time;
        }
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetMouseButtonUp(0))
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            _endSwiping = Camera.main.ScreenToViewportPoint(Input.mousePosition);
#else
            endSwiping = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position);

#endif
            BoolCHanger();

            _torqueRotate = (_torqueBool) ? -10 : 10; //Definido aqui para que em cada swipe a garrafa gire para um lado

            touchTimeEnded = Time.time;
            _swipeEndVector = (((_endSwiping - _startSwiping) / (touchTimeEnded - _touchTimeStarted)) * forceToAdd); //Dividided the position vector by the time vector to get a more precise point, and then already applied the designer controlled force

            _rb.AddForce(0, _swipeEndVector.y,0, ForceMode.Impulse);
            _rb.AddTorque(0, 0, _torqueRotate, ForceMode.Impulse); //To rotate the bottle in the air
        }
    }

}


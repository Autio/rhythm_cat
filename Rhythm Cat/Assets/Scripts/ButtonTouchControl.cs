using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;


    [RequireComponent(typeof(Button))]
    public class ButtonTouchControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public UnityEvent onPointerDown;
        public UnityEvent onPointerUp;

        public GameObject buttonObject;

        // gets invoked every frame while pointer is down
        public UnityEvent whilePointerPressed;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

    bool pressed = false;
    private void Update()
    {
        {

        }
    }
    private IEnumerator WhilePressed()
        {
            // this looks strange but is okey in a Coroutine
            // as long as you yield somewhere
            while (true)
            {
            Debug.Log("I'm pressed");
                whilePointerPressed?.Invoke();
                yield return null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // ignore if button not interactable
            if (!_button.interactable) return;
        pressed = true;
            // just to be sure kill all current routines
            // (although there should be none)
            StopAllCoroutines();
        //StartCoroutine(WhilePressed);
        Debug.Log("Down");


        onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopAllCoroutines();
            Debug.Log("Up");
            onPointerUp?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            onPointerUp?.Invoke();
        }
    }

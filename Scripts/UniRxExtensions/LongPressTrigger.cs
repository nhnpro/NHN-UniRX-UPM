using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RxExtension
{
    [AddComponentMenu("Rx/UI/LongPressTrigger")]
    public class LongPressTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float Duration = 0.5f;

        public UnityEvent OnLongPressDown = new UnityEvent();

        public UnityEvent OnLongPressUp = new UnityEvent();

        private bool _pressed;

        private float _pressedTime;

        private IDisposable _disposable;

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;

            if (_disposable == null)
            {
                _disposable = Observable.EveryUpdate().Subscribe(i =>
                {
                    if (_pressed)
                    {    
                        _pressedTime += Time.deltaTime;

                        if (_pressedTime >= Duration)
                        {
                            _pressed = false;
                            OnLongPressDown.Invoke();
                        }
                    }
                }).AddTo(this);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnLongPressUp.Invoke();
            _pressed = false;
            _pressedTime = 0f;
            _disposable.Dispose();
            _disposable = null;
        }
    }
}
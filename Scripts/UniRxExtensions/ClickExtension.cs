using System;
using RxExtension;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ClickExtension : RxBehaviour
{
    //https://github.com/rlatkdgus500/UniRx-Sample
    [SerializeField] private Button button;
    [SerializeField] private float rotationSpeed = 800.0f;

    [SerializeField] private SwipeGesture swipeGesture;
    void Start()
    {
        var clickStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0));

        //Screen
        clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250)))
            .Where(xs => xs.Count >= 2)
            .Subscribe(xs => Debug.Log("DoubleClick Detected! Count:" + xs.Count))
            .AddTo(_Disporsables);
        
        
        
        //Button
        var buttonClickStream = button.onClick.AsObservable()
            .Buffer(button.onClick.AsObservable().Throttle(TimeSpan.FromMilliseconds(250)))
            .Where(x => x.Count >= 2)
            .Subscribe(_ =>
            {
                //  text.text = "Double Clicked";
            })
            .AddTo(_Disporsables);

        var observableMouseTrigger = gameObject.AddComponent<ObservableMouseTrigger>();
        var Rotatestream = Observable.EveryUpdate()
            .SkipUntil(observableMouseTrigger.OnMouseDownAsObservable())
            .Select(_ => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")))
            .TakeUntil(observableMouseTrigger.OnMouseUpAsObservable())
            .Repeat()
            .Subscribe(move =>
            {
                this.transform.rotation =
                    Quaternion.AngleAxis(move.y * rotationSpeed * Time.deltaTime, Vector3.right) *
                    Quaternion.AngleAxis(-move.x * rotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
            })
            .AddTo(_Disporsables);
        
        this.swipeGesture
            .OnSwipeLeft
            .Subscribe(_ =>
            {
                Debug.LogError("Swipe OnSwipeLeft");
                
                this.transform.rotation =
                    Quaternion.AngleAxis(0 * rotationSpeed * Time.deltaTime, Vector3.right) *
                    Quaternion.AngleAxis(-10f * rotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
            }).AddTo(_Disporsables);

        // back
        this.swipeGesture
            .OnSwipeRight
            .Subscribe(_ =>
            {
                this.transform.rotation =
                    Quaternion.AngleAxis(0 * rotationSpeed * Time.deltaTime, Vector3.right) *
                    Quaternion.AngleAxis(10f * rotationSpeed * Time.deltaTime, Vector3.up) * transform.rotation;
            }).AddTo(_Disporsables);
        
        
        var pinchMultiply = 0.001f; //適当に調整するなりしてください
        var pinchEventHandler = gameObject.AddComponent<PinchEventHandler>();
        pinchEventHandler.OnPinch
            .Subscribe(pinchDistanceDelta =>
            {
                //ピンチで拡大・縮小
                transform.localScale += (pinchDistanceDelta * pinchMultiply) * Vector3.one;
            })
            .AddTo(this).AddTo(_Disporsables);
    }
    
}

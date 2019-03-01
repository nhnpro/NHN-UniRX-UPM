using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PinchEventHandler : MonoBehaviour
{
    private Subject<float> onPinch = new Subject<float>();

    public IObservable<float> OnPinch => onPinch;

    void Start()
    {
        //ピンチ終了
        var multiTapEndStream = this.UpdateAsObservable()
            .Where(_ => !MultiTapping());

        //ピンチ
        this.UpdateAsObservable()
            .Where(_ => MultiTapping()) //2箇所以上タップされている
            .Select(_ => GetMultiTapDistance()) //2点間の距離にメッセージを変換
            .Buffer(2, 1) //バッファに詰める
            .Select(distances => distances[1] - distances[0]) //前フレームとの距離の差にメッセージを変換
            .TakeUntil(multiTapEndStream) //ピンチ操作が終わったらバッファをクリアにする(TakeUntil+RepeatUntilDestory)
            .RepeatUntilDestroy(this)
            .Subscribe(delta =>
            {
                onPinch.OnNext(delta);
            });
    }

    private bool MultiTapping()
    {
        return Input.touchCount >= 2;
    }

    private float GetMultiTapDistance()
    {
        return Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
    }
}
using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;

public class RxBehaviour : MonoBehaviour {
    private List<IDisposable> subscriptions = new List<IDisposable>();
	public CompositeDisposable _Disporsables = new CompositeDisposable();
 
    public void AddSubscriptions(params IDisposable[] items)
    {
        subscriptions.AddRange(items);
    }

	public virtual void OnDestroy()
	{
		subscriptions.ForEach(s => s.Dispose());
		 _Disporsables.Clear();
	}
}
using UnityEngine;
using System.Collections;
using Vuforia;
using Colorful;

public class TrackableEffectTrigger : MonoBehaviour,ITrackableEventHandler {

	public float chargingTime;
	public ParticleSystem particlEffect;
	public RadialBlur blurEffect;
	public SlowMotionEffect slowMotionEffect;

	// Use this for initialization
	void Start () {
		blurEffect.enabled = false;
		slowMotionEffect.effectEndingCallback.AddListener (slowMotionEffectEnd);
		TrackableBehaviour mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			particlEffect.Stop (true);
		} else if (Input.GetKeyDown (KeyCode.G)) {
			particlEffect.Play (true);
		} else if (Input.GetKeyDown (KeyCode.H)) {
			particlEffect.Clear (true);
		}
	}

	bool triggerEffect = true;

	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus) {
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			OnTrackingFound();
		}
		else
		{
			OnTrackingLost();
		}
	}

	void OnTrackingFound() {
		if (triggerEffect) {
			triggerEffect = false;
			particlEffect.Play (true);
			Invoke ("preEffect", chargingTime);
		}

	}

	void preEffect() {
		particlEffect.Stop (true);
		blurEffect.enabled = true;
		Invoke ("startSlowMotionEffect", 1);
	}

	void startSlowMotionEffect() {
		slowMotionEffect.triggerSlowMotion ();
	}

	void slowMotionEffectEnd() {
		blurEffect.enabled = false;
	}

	void OnTrackingLost() {
		triggerEffect = true;
	}
}

using UnityEngine;
using System.Collections;
using Vuforia;
using UnityEngine.Events;

public class SlowMotionEffect : MonoBehaviour {

	public int durationInSecs;
	public int numFramesUsingSameFrame;
	public AudioSource audioSrc;
	[HideInInspector]
	public UnityEvent effectEndingCallback = new UnityEvent();

	Queue texQueue = null;
	bool hasInitedRect = false;
	Rect rectForReadPixels = new Rect();
	VuforiaRenderer.Vec2I texSize;
	bool startSlowMotionEffect = false;
	float timerStartPt;
	int frameCounter = 0;
	RenderTexture cachedTex = null;


	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		if (rectForReadPixels.width != 0 && startSlowMotionEffect) {
			RenderTexture renTex = RenderTexture.GetTemporary (texSize.x, texSize.y);
			Graphics.Blit (src, renTex);

			texQueue.Enqueue (renTex);
			if (frameCounter % numFramesUsingSameFrame == 0) {
				if (cachedTex != null) {
					cachedTex.Release ();
				}
				renTex = (RenderTexture)texQueue.Dequeue ();
			} else {
				renTex = cachedTex;
			}

			Graphics.Blit (renTex, dest);
			cachedTex = renTex;
			frameCounter++;

			if (Time.realtimeSinceStartup - timerStartPt >= durationInSecs) {
				stopSlowMotion ();	
			}

		} else {
			Graphics.Blit (src, dest);
		}


	}

	// Update is called once per frame
	void Update () {
		VuforiaRenderer renderer = VuforiaRenderer.Instance;
		if (renderer != null && !hasInitedRect) {
			texSize = renderer.GetVideoTextureInfo ().textureSize;
			if (texSize.x != 0 && texSize.y != 0) {
				rectForReadPixels = new Rect (Vector2.zero, new Vector2 (texSize.x, texSize.y));
				hasInitedRect = true;
			}
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			Debug.Log ("start slow motion");
			triggerSlowMotion ();
		}

	}

	private void stopSlowMotion() {
		startSlowMotionEffect = false;
		//do the clean up
		if (texQueue != null) {
			while (texQueue.Count != 0) {
				((RenderTexture)texQueue.Dequeue ()).Release ();
			}
		}
		audioSrc.Stop ();
		effectEndingCallback.Invoke ();
	}

	public void triggerSlowMotion() {
		if (!startSlowMotionEffect) {
			frameCounter = 0;
			texQueue = new Queue ();
			timerStartPt = Time.realtimeSinceStartup;
			audioSrc.clip = Microphone.Start ("Built-in Microphone", true, 1000, 44100);
			audioSrc.pitch = 1 / (float)numFramesUsingSameFrame;
			audioSrc.Play ();
			startSlowMotionEffect = true;
		}
	}

}

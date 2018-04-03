using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour {

	public Material defaultTransitionMaterial;
	Material transitionMaterial;
	public bool hasEnded = true;
	public void Start () {
		StartCoroutine (DebugTransitions ());
	}

	private void OnRenderImage (RenderTexture source, RenderTexture destination) {
		if (defaultTransitionMaterial != null || transitionMaterial != null)
			Graphics.Blit (source, destination, (transitionMaterial == null ? defaultTransitionMaterial : transitionMaterial));
	}

	public IEnumerator DebugTransitions () {
		yield return StartCoroutine (InOut (defaultTransitionMaterial, 0.7f, 1f, false));
	}

	public IEnumerator FadeIn (float duration, float preDelay) {
		yield return StartCoroutine (MakeTransition (defaultTransitionMaterial, duration, 1f, 0f, preDelay, true));
	}

	public IEnumerator FadeOut (float duration, float preDelay) {
		yield return StartCoroutine (MakeTransition (defaultTransitionMaterial, duration, 0f, 1f, preDelay, true));
	}

	public IEnumerator InOut (Material transitionMaterial_, float duration, float preDelay, bool fade) {
		yield return StartCoroutine (MakeTransition (transitionMaterial_, duration, 0f, 1f, preDelay, fade));
		yield return StartCoroutine (MakeTransition (transitionMaterial_, duration, 1f, 0f, preDelay, fade));
	}

	public IEnumerator MakeTransition (Material transitionMaterial_, float duration, float start, float end, float preDelay, bool fade) {
		hasEnded = false;

		string property = (fade == true ? "_Fade" : "_Cutoff");

		transitionMaterial_.SetFloat (Shader.PropertyToID ("_Cutoff"), start);
		transitionMaterial = transitionMaterial_;
		
		float value = start;
		float t = 0f;

		if (preDelay > 0f) {
			while (t < preDelay) {
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame ();
			}
		}

		t = 0f;
		transitionMaterial_.SetFloat (Shader.PropertyToID ("_Cutoff"), fade ? (start < end ? end : start) : start);

		if (fade)
			transitionMaterial_.SetFloat (Shader.PropertyToID ("_Fade"), start);
		while ((start < end ? value <= end : value >= end) && t < (duration+1f)) {
			t += Time.deltaTime;
			value = Mathf.Lerp (start, end, (t * t) / (duration * duration));
			transitionMaterial_.SetFloat (Shader.PropertyToID (property), value);
			yield return new WaitForEndOfFrame ();
		}

		transitionMaterial_.SetFloat (Shader.PropertyToID (property), end);
		hasEnded = true;
	}

	public void SetTransition (Material transitionMaterial_) {
		transitionMaterial = transitionMaterial_;
	}
}

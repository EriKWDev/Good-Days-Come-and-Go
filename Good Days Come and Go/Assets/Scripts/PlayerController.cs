using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerController : MonoBehaviour {
	public DialogueSpeaker dialogueSpeakerReference;
	public int direction = 0;
	public bool isWalking = false;
	public float moveSpeed = 5f;

	[SerializeField]
	private int pixelsPerUnit = 16;
	private Transform parent;
	private Animator animator;
	private float gridSize = 1f;
	private enum Orientation {
		Horizontal,
		Vertical
	};
	private Orientation gridOrientation = Orientation.Vertical;
	private bool allowDiagonals = false;
	private bool correctDiagonalSpeed = true;
	private Vector2 input;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float t;
	private float factor;

	void Start () {
		animator = GetComponent<Animator> ();
		parent = transform.parent;
	}

	private void LateUpdate () {
		Vector3 newLocalPosition = Vector3.zero;

		newLocalPosition.x = (Mathf.Round (parent.position.x * pixelsPerUnit) / pixelsPerUnit) - parent.position.x;
		newLocalPosition.y = 0.5f + (Mathf.Round (parent.position.y * pixelsPerUnit) / pixelsPerUnit) - parent.position.y;

		transform.localPosition = newLocalPosition;
	}

	private bool continueWalking = false;
	private Vector2 tmpInput;
	public void Update () {
		tmpInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (!isWalking) {
			input = tmpInput;
			if (!allowDiagonals) {
				if (Mathf.Abs (input.x) > Mathf.Abs (input.y)) {
					input.y = 0;
				} else {
					input.x = 0;
				}
			}

			if (input != Vector2.zero) {
				continueWalking = true;

				if (input.x == 0 && input.y == 1)
					direction = 0;
				else if (input.x == 1 && input.y == 0)
					direction = 1;
				else if (input.x == 0 && input.y == -1)
					direction = 2;
				else if (input.x == -1 && input.y == 0)
					direction = 3;

				animator.SetFloat ("Direction", (float)direction);
				StartCoroutine (Move (moveSpeed));
			} 
		}

		if (tmpInput == Vector2.zero) {
			continueWalking = false;
		}

		animator.SetBool ("isWalking", isWalking);
	}

	public IEnumerator Move (float speed) {
		isWalking = true;
		while (continueWalking) {
			startPosition = parent.position;
			t = 0;

			if (gridOrientation == Orientation.Horizontal) {
				endPosition = new Vector3 (startPosition.x + System.Math.Sign (input.x) * gridSize, startPosition.y, startPosition.z + System.Math.Sign (input.y) * gridSize);
			} else {
				endPosition = new Vector3 (startPosition.x + System.Math.Sign (input.x) * gridSize, startPosition.y + System.Math.Sign (input.y) * gridSize, startPosition.z);
			}

			if (allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0) {
				factor = 0.7071f;
			} else {
				factor = 1f;
			}

			while (t < 1f) {
				t += Time.deltaTime * (speed / gridSize) * factor;
				parent.position = Vector3.Lerp (startPosition, endPosition, t);
				yield return null;
			}
		}
		isWalking = false;
		yield return 0;
	}

	private void OnDrawGizmos () {
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube (endPosition, Vector3.one);
	}
}

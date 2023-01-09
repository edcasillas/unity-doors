using CommonUtils;
using CommonUtils.Extensions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Doors {
	public class DoorController : EnhancedMonoBehaviour, IDoor {
		[Serializable]
		private class EventConfiguration {
			public float Time;
			public iTween.EaseType EaseType;
			public AudioClip AudioClip;
			public UnityEvent OnFinished;
		}

		private const string OPEN_DOOR_TWEEN_NAME = "openDoor";
		private const string CLOSE_DOOR_TWEEN_NAME = "closeDoor";

		#region Inspector fields
		[SerializeField] private Transform hingeTransform;

		[SerializeField]
		private EventConfiguration openConfiguration = new() { Time = 1.5f, EaseType = iTween.EaseType.easeInOutSine };

		[SerializeField]
		private EventConfiguration closeConfiguration = new() { Time = 0.9f, EaseType = iTween.EaseType.easeOutSine };

		[Tooltip("If this value is zero, the door won't close automatically.")]
		[SerializeField] private float autoCloseTime;
		#endregion

		#region Properties
		public int Id { get; set; }
		[ShowInInspector] public bool IsOpen { get; private set; }
		#endregion

		#region Fields
		private Coroutine closeCoroutine;
		#endregion

		#region Public methods
		[ShowInInspector]
		public void ToggleState(int dir) {
			if(IsOpen) Close();
			else Open(dir);
		}

		[ShowInInspector]
		public void Open(int dir) {
			if (dir != 1 && dir != -1) {
				this.LogError($"Parameter {nameof(dir)} must be either 1 or -1 to specify the opening direction of the door.");
				return;
			}

			if (closeCoroutine != null) {
				StopCoroutine(closeCoroutine);
				closeCoroutine = null;
			}

			if(openConfiguration.AudioClip) AudioSource.PlayClipAtPoint(openConfiguration.AudioClip, transform.position);
			iTween.StopByName(gameObject, CLOSE_DOOR_TWEEN_NAME);
			var desiredRotation = 90f * dir;
			iTween.RotateTo(hingeTransform.gameObject, iTween.Hash(
				"name", OPEN_DOOR_TWEEN_NAME,
				"y", desiredRotation,
				"isLocal", true,
				"time", openConfiguration.Time,
				"oncomplete", nameof(OnDoorOpened),
				"oncompletetarget", this.gameObject,
				"easetype", openConfiguration.EaseType));
			IsOpen = true;

			if (autoCloseTime > 0) StartCoroutine(waitAndClose());
		}

		[ShowInInspector]
		public void Close() {
			if (closeCoroutine != null) {
				StopCoroutine(closeCoroutine);
				closeCoroutine = null;
			}

			if(closeConfiguration.AudioClip) AudioSource.PlayClipAtPoint(closeConfiguration.AudioClip, transform.position);
			iTween.StopByName(gameObject, OPEN_DOOR_TWEEN_NAME);
			iTween.RotateTo(hingeTransform.gameObject, iTween.Hash(
				"name", CLOSE_DOOR_TWEEN_NAME,
				"y", 0,
				"isLocal", true,
				"time", closeConfiguration.Time,
				"oncomplete", nameof(OnDoorClosed),
				"oncompletetarget", this.gameObject,
				"easetype", closeConfiguration.EaseType));
			IsOpen = false;
		}

		/// <summary>
		/// Given a forward direction from an entity opening the door (<paramref name="forwardDir"/>), this method
		/// returns the direction in which the door must open. The result of this method can then be used as parameter
		/// in the <see cref="Open"/> method.
		/// </summary>
		/// <param name="forwardDir">Forward vector of an entity opening the door (eg. the player's forward vector).</param>
		/// <returns>A value that is either 1 or -1, indicating the required opening direction.</returns>
		[ShowInInspector]
		public int GetOpeningDirection(Vector3 forwardDir) {
			forwardDir.y = 0;
			forwardDir.Normalize();
			var dot = Vector3.Dot(forwardDir, transform.forward);
			return dot == 0 ? 0 : (dot > 0 ? -1 : 1);
		}

		public void SetOpeningSound(AudioClip clip) => openConfiguration.AudioClip = clip;
		public void SetClosingSound(AudioClip clip) => closeConfiguration.AudioClip = clip;
		#endregion

		private void OnDoorOpened() {
			this.DebugLog($"Door {Id} ({name}) finished opening");
			openConfiguration.OnFinished?.Invoke();
		}

		private void OnDoorClosed() {
			this.DebugLog($"Door {Id} ({name}) finished closing");
			closeConfiguration.OnFinished?.Invoke();
		}

		private IEnumerator waitAndClose() {
			yield return new WaitForSeconds(autoCloseTime);
			Close();
			closeCoroutine = null;
		}
	}
}
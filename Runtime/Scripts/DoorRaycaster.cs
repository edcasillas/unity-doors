using CommonUtils;
using CommonUtils.UnityComponents;
using Doors;
using UnityEngine;

/// <summary>
/// This component is meant to be added to the player game object to allow it to interact with doors through the <see cref="DoorProxy"/>
/// </summary>
public class DoorRaycaster : EnhancedMonoBehaviour {
	[SerializeField] private KeyCode interactionKey;
	[SerializeField] private LayerMask doorLayer = ~0;
	[SerializeField] private float maxDistance = 2f;

	[ShowInInspector] private IDoor ActiveDoor { get; set; }

	private void Update() {
		if(!ActiveDoor.IsValid()) return;
		if (Input.GetKeyDown(interactionKey)) {
			var openDirection = ActiveDoor.GetOpeningDirection(transform.forward);
			ActiveDoor.ToggleState(openDirection);
		}
	}

	private void FixedUpdate() {
		RaycastHit hit;

		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxDistance, Color.green);
		ActiveDoor = Physics.Raycast(transform.position,
			transform.TransformDirection(Vector3.forward),
			out hit,
			maxDistance,
			doorLayer) ? hit.collider.gameObject.GetCachedComponent<DoorProxy>() : null;
	}
}

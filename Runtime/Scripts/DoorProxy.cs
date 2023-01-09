using UnityEngine;

namespace Doors {
	public class DoorProxy : MonoBehaviour, IDoor {
		[SerializeField] private DoorController door;

		public void ToggleState(int dir) => door.ToggleState(dir);
		public void Open(int dir) => door.Open(dir);
		public void Close() => door.Close();
		public int GetOpeningDirection(Vector3 forwardDir) => door.GetOpeningDirection(forwardDir);
	}
}

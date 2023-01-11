using UnityEngine;

namespace Doors {
	public class DoorProxy : MonoBehaviour, IDoor {
		[SerializeField] private DoorController door;

		public bool IsLocked { get => door.IsLocked; set => door.IsLocked = value; }

		public bool ToggleState(int dir) => door.ToggleState(dir);
		public bool Open(int dir) => door.Open(dir);
		public void Close() => door.Close();
		public int GetOpeningDirection(Vector3 forwardDir) => door.GetOpeningDirection(forwardDir);
	}
}

using CommonUtils.UnityComponents;
using UnityEngine;

namespace Doors {
	public interface IDoor: IUnityComponent {
		bool IsLocked { get; set; }
		bool ToggleState(int dir);
		public bool Open(int dir);
		public void Close();
		public int GetOpeningDirection(Vector3 forwardDir);
	}
}
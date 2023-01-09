using CommonUtils.UnityComponents;
using UnityEngine;

namespace Doors {
	public interface IDoor: IUnityComponent {
		public void ToggleState(int dir);
		public void Open(int dir);
		public void Close();
		public int GetOpeningDirection(Vector3 forwardDir);
	}
}
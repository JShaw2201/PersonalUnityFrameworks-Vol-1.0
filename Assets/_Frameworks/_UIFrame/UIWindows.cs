using UnityEngine;
using System.Collections;

namespace JXFrame.View { 
	public abstract class UIWindows : UIBase {
	

		protected override void OnRelease()
		{
			if (UIManager.Instance != null)
				UIManager.Instance.RemoveUIFormToCatch(m_uIFormName);			
		}	
	}
}
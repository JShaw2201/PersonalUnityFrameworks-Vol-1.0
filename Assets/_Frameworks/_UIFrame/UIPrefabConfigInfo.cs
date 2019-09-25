
using System.Collections.Generic;
using System;

namespace JXFrame.View
{
    [Serializable]
    public class UIPrefabConfigInfo {
        public List<UIPrefabConfigNode> UIPrefabInfo = null;
	}

	[Serializable]
	public class UIPrefabConfigNode
    {
        public string UIFormName = null;
        public bool UIFormLuaScript = false;
		public string UIFormClassName = null;
		public string UIFormPrefabUrl = null;
        public string UIFormPrefabName = null;
        public string UIFormPrefabDirType = "Resources";   /*Resources,streamingAssetsPath,dataPath,persistentDataPath**/
        public string UIFormLuaScriptUrl = null;     
        public string UIFormLuaScripDirType = "Resources";   /*Resources,streamingAssetsPath,dataPath,persistentDataPath**/
        public bool UIFormLuaScriptAssetBudle = false;
    }
}
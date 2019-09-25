using ResLibary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResLibary
{
    public class ResLibaryFactory
    {


        public ILibaryHandle CreateHandle(string type, System.Action<LibaryStateObj> updateAssetCallback = null)
        {
            switch (type)
            {
                case "FileLibary":
                    FileLibary.Instance.UpdateAssetCallback = updateAssetCallback;
                    return FileLibary.Instance;
                case "StreamingAssetLibary":
                    return new StreamingAssetLibary(updateAssetCallback);
                case "AssetsLibary":
                    AssetsLibary.Instance.UpdateAssetCallback = updateAssetCallback;
                    return AssetsLibary.Instance;
                case "ResourceLibary":
                    return new ResourceLibary(updateAssetCallback);
                case "AssetBundleLibary":
                    AssetBundleLibary.Instance.UpdateAssetCallback = updateAssetCallback;
                    return AssetBundleLibary.Instance;
            }
            return null;
        }
    }
}

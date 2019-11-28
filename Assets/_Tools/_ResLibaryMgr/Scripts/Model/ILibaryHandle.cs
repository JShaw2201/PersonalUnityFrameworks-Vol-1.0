using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResLibary
{
    public interface ILibaryHandle
    {
        string GetTextAsset(string objName);

        Sprite GetSprite(string objName);
        Texture2D GetTexture2d(string objName);

        T GetObject<T>(string objName) where T : UnityEngine.Object;
        UnityEngine.Object GetUnityObject(string _type, string objName);
        LibaryExistStatusEnum TryGetObject(LibaryTypeEnum libaryTypeEnum, string objName, out object data);

        void InsertLibrary(object data);

        void DeleteLiibrary(string type,string name);

        /// <summary>
        /// 释放某个资源
        /// </summary>
        /// <param name="objName"></param>
        void releaseObj(string _type, string objName);
        void releaseObj(LibaryTypeEnum libaryTypeEnum, string objName);
        /// <summary>
        /// 释放全部
        /// </summary>
        void releaseAll();

        void releaseObj(UnityEngine.Object obj);

        void releaseScene();
    }
}
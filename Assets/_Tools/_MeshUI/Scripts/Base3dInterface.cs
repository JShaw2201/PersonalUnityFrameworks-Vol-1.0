using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Canvas3d
{
    public enum StatusEnum3d
    {
        Normal,
        Highlight,
        Pressed,
        Disabled
    }

    public interface OnHighlight
    {
        void OnHighlight();
    }

    public interface OnPresseDown
    {
        void OnPresseDown();
    }

    public interface OnPresseUp
    {
        void OnPresseUp();
    }

    public interface OnDisabled
    {
        void OnDisabled();
    }

    public interface OnNormal
    {
        void OnNormal();
    }

    public interface  Base3dInterface : OnNormal, OnPresseDown, OnHighlight, OnDisabled, OnPresseUp
    {
        bool RaycastTarget { get; set; }
        StatusEnum3d statusEnum3d { get; set; }
        
    }
}
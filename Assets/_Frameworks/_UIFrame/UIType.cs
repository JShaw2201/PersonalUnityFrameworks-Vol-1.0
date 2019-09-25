using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JXFrame.View
{
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public class UIType
	{
        /// <summary>
	    /// 自成新的画布
	    /// </summary>
        public bool IsNewCanvas = false;

	    //是否清空“栈集合”
	    public bool IsClearStack = false;
	    //UI窗体（位置）类型
	    public UIFormType UIForms_Type = UIFormType.Normal;
	    //UI窗体显示类型
	    public UIFormShowMode UIForms_ShowMode = UIFormShowMode.Normal;
	    //UI窗体透明度类型
	    public UIFormLucenyType UIForm_LucencyType = UIFormLucenyType.Pentrate;

        public UIType() { }

    }

    /// <summary>
    /// UI窗体（位置）类型
    /// </summary>
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public enum UIFormType
	{
	    /// <summary>
	    /// 普通窗体
	    /// </summary>
	    Normal=0,

	    /// <summary>
	    /// 固定窗体    
	    /// </summary>
	    Fixed=1,

	    /// <summary>
	    /// 弹出窗体
	    /// </summary>
	    PopUp=2,
	    
	    
	}

    /// <summary>
    /// UI窗体的显示类型
    /// </summary>
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public enum UIFormShowMode
	{
	    //普通
	    Normal=0,
	    //反向切换
	    ReverseChange=2,
	    //隐藏其他
	    HideOther=3
	}

    /// <summary>
    /// UI窗体透明度类型
    /// </summary>
#if HOTFIX_ENABLE
    [XLua.LuaCallCSharp]
#endif
    public enum UIFormLucenyType
	{
        /// <summary>
        /// 完全透明，不能穿透
        /// </summary>
        Lucency=0,

        /// <summary>
        /// 半透明，不能穿透
        /// </summary>
        Translucence=1,

        /// <summary>
        /// 低透明度，不能穿透
        /// </summary>
        ImPenetrable=2,

        /// <summary>
        /// 可以穿透
        /// </summary>
        Pentrate=3
    }
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace JS.Container.Tree
{
    
    public abstract class BaseTreeNodeSingleLeaf<T1, T2> : BaseTreeNode<T1, T2> where T2 : IBaseTreeNode
    {
        protected abstract T1 data { get; set; }
        public BaseTreeNodeSingleLeaf() : base() { }
        public BaseTreeNodeSingleLeaf(T1 leave)
        {
            InsertLeave(leave);
        }
        public BaseTreeNodeSingleLeaf(T2 child) : base(child) { }
        public BaseTreeNodeSingleLeaf(T2[] childs) : base(childs){       }
       
        public BaseTreeNodeSingleLeaf(T1 leave, T2[] childs)
        {
            InsertLeave(leave);
            InsertBranch(childs);
        }

        #region TreeLeave

        public virtual void SetLeave(T1 leave) { m_SetLeave(leave); }
        private void m_SetLeave(T1 leave)
        {
            data = leave;
        }

        /// <summary>
        /// 添加叶子数据
        /// </summary>
        /// <param name="leave"></param>
        public virtual bool InsertLeave(T1 leave) { return m_InsertLeave(leave); }
        private bool m_InsertLeave(T1 leave)
        {
            if (data != null)
                return false;
            m_SetLeave(leave);
            return true;
        }               

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public T1 GetLeave() { return m_GetLeave(); }
        private T1 m_GetLeave()
        {          
           return data;
        }

        

        #endregion
    }
}

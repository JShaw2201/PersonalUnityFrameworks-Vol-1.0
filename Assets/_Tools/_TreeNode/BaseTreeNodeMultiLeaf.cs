using System;
using System.Collections;
using System.Collections.Generic;

namespace JS.Container.Tree
{
   
    public abstract class BaseTreeNodeMultiLeaf<T1, T2> : BaseTreeNode<T1,T2> where T2 : IBaseTreeNode
    {
        protected abstract List<T1> leaves   {   get;  }

        public BaseTreeNodeMultiLeaf() : base() { }
        public BaseTreeNodeMultiLeaf(T1[] leaves)
        {
            for (int i = 0; i < leaves.Length; i++)
            {
                InsertLeave(leaves[i]);
            }
        }
        public BaseTreeNodeMultiLeaf(T2 child) : base(child) { }
        public BaseTreeNodeMultiLeaf(T2[] childs) : base(childs)
        {
           
        }
        public BaseTreeNodeMultiLeaf(T1[] leaves, T2 child)
        {
            for (int i = 0; i < leaves.Length; i++)
            {
                InsertLeave(leaves[i]);
            }
            InsertBranch(child);
        }
        public BaseTreeNodeMultiLeaf(T1[] leaves, T2[] childs)
        {
            for (int i = 0; i < leaves.Length; i++)
            {
                InsertLeave(leaves[i]);
            }
            InsertBranch(childs);
        }

        #region TreeLeave

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="leaves"></param>
        public virtual void SetLeave(T1 leave, int leaveIndex) { }
        private void m_SetLeave(T1 leave, int leaveIndex)
        {
            if (leaveIndex >= 0 && leaveIndex < leaves.Count)
                leaves[leaveIndex] = leave;
        }

        /// <summary>
        /// 添加叶子数据
        /// </summary>
        /// <param name="leave"></param>
        public virtual bool InsertLeave(T1 leave) { return m_InsertLeave(leave); }
        private bool m_InsertLeave(T1 leave)
        {
            if (!leaves.Contains(leave))
            {
                leaves.Add(leave);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 添加叶子数据
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="leaveIndex"></param>
        public virtual bool InsertLeave(T1 leave, int leaveIndex) { return m_InsertLeave(leave, leaveIndex); }
        private bool m_InsertLeave(T1 leave, int leaveIndex)
        {
            if (leaveIndex >= 0 && leaveIndex < leaves.Count)
                leaves.Insert(leaveIndex, leave);
            else
                leaves.Add(leave);
            return true;
        }

        /// <summary>
        /// 删除叶子数据
        /// </summary>
        /// <param name="leave"></param>
        public virtual bool RemoveLeave(T1 leave) { return m_RemoveLeave(leave); }
        private bool m_RemoveLeave(T1 leave)
        {
            return leaves.Remove(leave);
        }

        /// <summary>
        /// 删除叶子数据
        /// </summary>
        /// <param name="leaveIndex"></param>
        public virtual bool RemoveLeaveAt(int leaveIndex) { return m_RemoveLeaveAt(leaveIndex); }
        private bool m_RemoveLeaveAt(int leaveIndex)
        {
            if (leaveIndex >= 0 && leaveIndex < leaves.Count)
            {
                leaves.RemoveAt(leaveIndex);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除叶子数据
        /// </summary>
        public virtual void RemoveLeaveAll() { m_RemoveLeaveAll(); }
        private void m_RemoveLeaveAll()
        {
            leaves.Clear();
        }


        /// <summary>
        /// 获取叶子的数量
        /// </summary>
        /// <returns></returns>
        public int GetLeaveCount() { return m_GetLeaveCount(); }
        private int m_GetLeaveCount()
        {
            return leaves.Count;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public T1 GetLeave(int leaveIndex) { return m_GetLeave(leaveIndex); }
        private T1 m_GetLeave(int leaveIndex)
        {
            if (leaveIndex >= 0 && leaveIndex < leaves.Count)
                return leaves[leaveIndex];
            return default(T1);
        }

        /// <summary>
        /// 获取所有叶子
        /// </summary>
        /// <returns></returns>
        public T1[] GetLeaves() { return m_GetLeaves(); }
        private T1[] m_GetLeaves()
        {
            return leaves.ToArray();
        }

        #endregion

    }
}

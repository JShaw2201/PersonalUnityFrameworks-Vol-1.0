using System;
using System.Collections;
using System.Collections.Generic;

namespace JS.Container.Tree
{
    public interface IBaseTreeNode
    {
        //private int _id = 0;
        //private int m_TreeNodeID
        //{
        //    get
        //    {
        //        if (_id == 0)
        //        {
        //            string idStr = System.DateTime.Now.ToString() + Guid.NewGuid().ToString();
        //            this._id = idStr.GetHashCode();
        //        }
        //        return _id;
        //    }
        //}
        //public int TreeNodeID { get { return m_TreeNodeID; } }

        int TreeNodeID { get; set; }

        void ReviseKeys();
    }

    public abstract class BaseTreeNode<T1, T2> : IBaseTreeNode where T2 : IBaseTreeNode
    {
        #region Private Variable

        private IBaseTreeNode parent;

        private Dictionary<int, T2> _branches;
        private Dictionary<int, T2> branches
        {
            get
            {
                if (_branches == null)
                     _branches = new Dictionary<int, T2>();
                return _branches;
            }
        }

        private int _id = 0;
        private int m_TreeNodeID
        {
            get
            {
                if (_id == 0)
                {
                    string idStr = System.DateTime.Now.ToString() + Guid.NewGuid().ToString();
                    this._id = idStr.GetHashCode();
                }
                return _id;
            }
        }
        #endregion

        #region Construction Parameter
        public BaseTreeNode() { }


        public BaseTreeNode(T2 child) { InitBaseTreeNode(child); }
        private void InitBaseTreeNode(T2 child)
        {
            this.InsertBranch(child);

        }
        public BaseTreeNode(T2[] childs) { InitBaseTreeNode(childs); }
        private void InitBaseTreeNode(T2[] childs)
        {
            this.InsertBranch(childs);
        }


        #endregion

        #region IBaseTreeNode

        public virtual int TreeNodeID
        {
            get { return m_TreeNodeID; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    if (parent != null)
                        parent.ReviseKeys();
                }
            }
        }

        void IBaseTreeNode.ReviseKeys()
        {
            lock (branches)
            {
                List<int> keys = new List<int>(branches.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    IBaseTreeNode t = branches[keys[i]];
                    if (keys[i] != t.TreeNodeID)
                    {
                        branches.Remove(keys[i]);
                        branches[t.TreeNodeID] = (T2)t;
                    }
                }
            }
        }

        #endregion

        #region TreeBranch

        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="pnode"></param>
        public virtual void SetParent(T2 pnode) { m_SetParent(pnode); }
        private void m_SetParent(T2 pnode)
        {
            parent = pnode;
        }

        /// <summary>
        /// 根节点
        /// </summary>
        /// <returns></returns>
        public T2 root { get { return _root; } }
        private T2 _root
        {
            get
            {
                IBaseTreeNode node;
                if (parent == null)
                {
                    node = this;
                }
                else
                {
                    node = ((BaseTreeNode<T1, T2>)parent)._root;
                }
                return (T2)node;
            }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        /// <returns></returns>
        public virtual T2 GetParent() { return m_GetParent(); }
        private T2 m_GetParent()
        {
            return (T2)parent;
        }

        /// <summary>
        /// 改变分支
        /// </summary>
        /// <param name="childIndex"></param>
        /// <param name="child"></param>
        public virtual bool SetBranch(int childIndex, T2 child) { return m_SetBranch(childIndex, child); }
        private bool m_SetBranch(int childIndex, T2 child)
        {
            if (ContainBranch(child))
                return false;
            if (childIndex < branches.Count)
            {
                T2 t2 = m_GetBranch(childIndex);
                BaseTreeNode<T1, T2> _T2 = t2 as BaseTreeNode<T1, T2>;
                _T2.DicConnectParent();
            }
            branches[child.TreeNodeID] = child;
            IBaseTreeNode _P = this as IBaseTreeNode;
            BaseTreeNode<T1, T2> _C = child as BaseTreeNode<T1, T2>;
            _C.SetParent((T2)_P);
            return true;
        }



        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="multiListObj"></param>
        public virtual bool InsertBranch(T2 child) { return m_InsertBranch(child); }
        private bool m_InsertBranch(T2 child)
        {
            if (ContainBranch(child))
                return false;
            branches.Add(child.TreeNodeID, child);
            IBaseTreeNode _P = this as IBaseTreeNode;
            BaseTreeNode<T1, T2> _C = child as BaseTreeNode<T1, T2>;
            _C.SetParent((T2)_P);
            return true;
        }

        /// <summary>
        /// 插入对象
        /// </summary>
        /// <param name="childs"></param>
        public virtual bool InsertBranch(T2[] childs) { return m_InsertBranch(childs); }
        private bool m_InsertBranch(T2[] childs)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                InsertBranch(childs[i]);
            }
            return true;
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual bool RemoveBranch() { return m_RemoveBranch(); }
        private bool m_RemoveBranch()
        {
            if (branches.Count != 0)
            {
                BaseTreeNode<T1, T2> _C = m_GetBranch(branches.Count - 1) as BaseTreeNode<T1, T2>;
                _C.SetParent(default(T2));
                branches.Remove(_C.TreeNodeID);
                return true;
            }
            return false;
        }



        /// <summary>
        /// 删除指定对象
        /// </summary>
        /// <param name="multiListObj"></param>
        public virtual bool RemoveBranch(T2 child) { return m_RemoveBranch(child); }
        private bool m_RemoveBranch(T2 child)
        {
            BaseTreeNode<T1, T2> _C = child as BaseTreeNode<T1, T2>;
            _C.SetParent(default(T2));
            return branches.Remove(child.TreeNodeID);
        }

        /// <summary>
        /// 删除指定位置
        /// </summary>
        public virtual bool RemoveBranchAt(int childIndex) { return m_RemoveBranchAt(childIndex); }
        private bool m_RemoveBranchAt(int childIndex)
        {
            if (childIndex >= 0 && childIndex < branches.Count)
            {
                BaseTreeNode<T1, T2> _C = m_GetBranch(childIndex) as BaseTreeNode<T1, T2>;
                _C.SetParent(default(T2));
                branches.Remove(_C.TreeNodeID);
                return true;
            }
            return false;
        }

        public virtual bool RemoveBranchAtId(int id) { return m_RemoveBranchAtId(id); }
        private bool m_RemoveBranchAtId(int id)
        {
            if (branches.ContainsKey(id))
            {
                BaseTreeNode<T1, T2> _C = branches[id] as BaseTreeNode<T1, T2>;
                _C.SetParent(default(T2));
                branches.Remove(id);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除所有
        /// </summary>
        public virtual void RemoveBranchAll() { m_RemoveBranchAll(); }
        private void m_RemoveBranchAll()
        {
            branches.Clear();
        }

        public virtual bool ContainID(int id) { return m_ContainID(id); }
        private bool m_ContainID(int id)
        {
            if (branches.ContainsKey(id))
            {
                return true;
            }
            else
            {
                List<int> IDList = new List<int>(branches.Keys);
                for (int i = 0; i < IDList.Count; i++)
                {
                    BaseTreeNode<T1, T2> node = branches[IDList[i]] as BaseTreeNode<T1, T2>;
                    if (node.m_ContainID(id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 树形结构中是否已经存在这个分支
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool ContainBranch(T2 t) { return m_ContainBranch(t); }
        private bool m_ContainBranch(T2 t)
        {
            if (branches.ContainsKey(t.TreeNodeID))
            {
                return true;
            }
            else
            {
                List<int> IDList = new List<int>(branches.Keys);
                for (int i = 0; i < IDList.Count; i++)
                {
                    BaseTreeNode<T1, T2> node = branches[IDList[i]] as BaseTreeNode<T1, T2>;
                    if (node.ContainBranch(t))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 与父节点断开
        /// </summary>
        public virtual void DicConnectParent() { m_DicConnectParent(); }
        private void m_DicConnectParent()
        {
            if (parent == null)
            {
                BaseTreeNode<T1, T2> _P = parent as BaseTreeNode<T1, T2>;
                IBaseTreeNode _C = this;
                _P.RemoveBranch((T2)_C);
            }
        }

        /// <summary>
        /// 获取深度
        /// </summary>
        /// <returns></returns>
        public int GetBranchDepth() { return m_GetBranchDepth(); }
        private int m_GetBranchDepth()
        {
            int currentDepth = 0;
            if (branches.Count == 0)
            {
                return 0;
            }
            List<int> branchList = new List<int>(branches.Keys);
            for (int i = 0; i < branchList.Count; i++)
            {
                BaseTreeNode<T1, T2> _C = branches[branchList[i]] as BaseTreeNode<T1, T2>;
                int _curDepth = _C.GetBranchDepth() + 1;
                if (currentDepth < _curDepth)
                {
                    currentDepth = _curDepth;
                }
            }

            return currentDepth;
        }

        /// <summary>
        /// 获取自己所在深度
        /// </summary>
        /// <returns></returns>
        public int GetCurrentDepthInTree() { return m_GetCurrentDepthInTree(); }
        private int m_GetCurrentDepthInTree()
        {
            if (parent == null)
                return 0;
            BaseTreeNode<T1, T2> _P = parent as BaseTreeNode<T1, T2>;
            return _P.GetCurrentDepthInTree() + 1;
        }

        /// <summary>
        /// 获取对应深度的分支数量
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public int GetBranchWidth(int depth) { return m_GetBranchWidth(depth); }
        private int m_GetBranchWidth(int depth)
        {
            if (depth == 0)
            {
                return 1;
            }

            int currentIndex = 0;
            if (depth == 1)
            {
                return branches.Count;
            }
            else if (depth > 1)
            {
                List<int> branchList = new List<int>(branches.Keys);
                for (int i = 0; i < branchList.Count; i++)
                {
                    T2 node = branches[branchList[i]];
                    BaseTreeNode<T1, T2> _C = node as BaseTreeNode<T1, T2>;
                    currentIndex += _C.GetBranchWidth(depth - 1);
                }
            }
            return currentIndex;
        }

        /// <summary>
        /// 大小
        /// </summary>
        /// <returns></returns>
        public int GetBranchCount() { return m_GetBranchCount(); }
        private int m_GetBranchCount()
        {
            return branches.Count;
        }

        public T2 GetBranchAtIdInNextDepth(int id) { return m_GetBranchAtIdInNextDepth(id); }
        private T2 m_GetBranchAtIdInNextDepth(int id)
        {
            if (id == TreeNodeID)
            {
                IBaseTreeNode btn = this;
                return (T2)btn;
            }
            if (branches.ContainsKey(id))
            {
                return branches[id];
            }
            return default(T2);
        }

        public T2 GetBranchAtIdInAll(int id) { return m_GetBranchAtIdInAll(id); }
        private T2 m_GetBranchAtIdInAll(int id)
        {
            T2 t = default(T2);
            if (id == TreeNodeID)
            {
                IBaseTreeNode btn = this;
                return (T2)btn;
            }
            if (branches.ContainsKey(id))
            {
                t = branches[id];
            }
            else
            {
                List<int> branchList = new List<int>(branches.Keys);
                for (int i = 0; i < branchList.Count; i++)
                {
                    T2 cnode = branches[branchList[i]];
                    BaseTreeNode<T1, T2> _C = cnode as BaseTreeNode<T1, T2>;
                    T2 node = _C.GetBranchAtIdInAll(id);
                    if (node != null)
                    {
                        t = node;
                        break;
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// 获取分支
        /// </summary>
        /// <param name="childIndex"></param>
        /// <returns></returns>
        public T2 GetBranch(int childIndex) { return m_GetBranch(childIndex); }
        private T2 m_GetBranch(int childIndex)
        {
            if (childIndex >= 0 && childIndex < branches.Count)
            {
                List<int> branchList = new List<int>(branches.Keys);
                return branches[branchList[childIndex]];
            }
            return default(T2);
        }

        /// <summary>
        /// 通过深度获取分支
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="childIndex"></param>
        /// <returns></returns>
        public int GetBranch(int depth, int childIndex, ref T2 t2) { return m_GetBranch(depth, childIndex, ref t2); }
        private int m_GetBranch(int depth, int childIndex, ref T2 t2)
        {
            if (depth == 0)
            {
                IBaseTreeNode _N = this;
                t2 = (T2)_N;
                return 0;
            }
            int currentIndex = 0;
            List<int> branchList = new List<int>(branches.Keys);
            for (int i = 0; i < branchList.Count; i++)
            {
                T2 node = branches[branchList[i]];
                if (depth == 1)
                {
                    if (childIndex == i)
                    {
                        t2 = node;
                        currentIndex = 0;
                        break;
                    }
                    currentIndex++;
                }
                else if (depth > 1)
                {
                    BaseTreeNode<T1, T2> _C = node as BaseTreeNode<T1, T2>;
                    int _cIndex = _C.m_GetBranch(depth - 1, childIndex - currentIndex, ref t2);

                    if (_cIndex == 0)
                    {
                        currentIndex = 0;
                        break;
                    }
                    currentIndex += _cIndex;
                }
            }
            return currentIndex;
        }

        /// <summary>
        /// 获取分支
        /// </summary>
        /// <returns></returns>
        public T2[] GetBranch() { return m_GetBranch(); }
        private T2[] m_GetBranch()
        {
            List<T2> branchList = new List<T2>(branches.Values);
            return branchList.ToArray();
        }
        #endregion

    }
}

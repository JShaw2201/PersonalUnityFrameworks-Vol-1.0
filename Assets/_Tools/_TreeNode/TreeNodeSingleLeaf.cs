using System.Collections;
using System.Collections.Generic;

namespace JS.Container.Tree
{
    public abstract class TreeNodeSingleLeaf<T> : BaseTreeNodeSingleLeaf<T, TreeNodeSingleLeaf<T>>
    {
        public TreeNodeSingleLeaf() : base() { }
        public TreeNodeSingleLeaf(T leave): base(leave)   {           }
        public TreeNodeSingleLeaf(TreeNodeSingleLeaf<T> child) : base(child) { }
        public TreeNodeSingleLeaf(TreeNodeSingleLeaf<T>[] childs) : base(childs) { }
        public TreeNodeSingleLeaf(T leave, TreeNodeSingleLeaf<T>[] childs): base(leave,childs) {     }

    }
}

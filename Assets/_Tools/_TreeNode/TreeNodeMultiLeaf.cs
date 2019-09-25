using System.Collections;
using System.Collections.Generic;
namespace JS.Container.Tree
{
    public abstract class TreeNodeMultiLeaf<T> : BaseTreeNodeMultiLeaf<T, TreeNodeMultiLeaf<T>>
    {
        public TreeNodeMultiLeaf() : base() { }
        public TreeNodeMultiLeaf(T[] leaves) : base(leaves) { }
        public TreeNodeMultiLeaf(TreeNodeMultiLeaf<T> child) : base(child) { }
        public TreeNodeMultiLeaf(TreeNodeMultiLeaf<T>[] childs) : base(childs) { }
        public TreeNodeMultiLeaf(T[] leaves, TreeNodeMultiLeaf<T> child) : base(leaves, child) { }
        public TreeNodeMultiLeaf(T[] leaves, TreeNodeMultiLeaf<T>[] childs) : base(leaves, childs) { }
    }
}
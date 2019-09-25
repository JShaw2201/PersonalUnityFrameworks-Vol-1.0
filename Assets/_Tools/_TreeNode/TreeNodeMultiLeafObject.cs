using System;
using System.Collections;
using System.Collections.Generic;
namespace JS.Container.Tree
{
    public class TreeNodeMultiLeafObject : BaseTreeNodeMultiLeaf<object, TreeNodeMultiLeafObject> 
    {
        private List<object> _leaves;
        public TreeNodeMultiLeafObject() : base() { }
        public TreeNodeMultiLeafObject(object[] leaves) : base(leaves) { }
        public TreeNodeMultiLeafObject(TreeNodeMultiLeafObject child) : base(child) { }
        public TreeNodeMultiLeafObject(TreeNodeMultiLeafObject[] childs) : base(childs) { }
        public TreeNodeMultiLeafObject(object[] leaves, TreeNodeMultiLeafObject child) : base(leaves, child) { }
        public TreeNodeMultiLeafObject(object[] leaves, TreeNodeMultiLeafObject[] childs) : base(leaves, childs) { }

        protected override List<object> leaves
        {
            get
            {
                if (_leaves == null)
                    _leaves = new List<object>();
                return _leaves;
            }
        }
    }
}
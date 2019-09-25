using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelTool.Model
{
    public class EditorCell : ICell
    {
        
        private ICell _cell;
        public string Colunm
        {
            get { return _cell.Colunm; }
            set { _cell.Colunm = value; }
        }

        public string Value
        {
            get { return _cell.Value; }
            set { _cell.Value = value; }
        }

        public bool CanExport = false;
        public string ExportColId;
        
        public EditorCell()
        {
            this._cell = new Cell();
        }
        
        public EditorCell(ICell _cell)
        {
            this._cell = _cell;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelTool.Model
{
	public class Row : IRow<Cell>
	{
		public int CellCount
		{
			get { return _CellCount;}	
			set {_CellCount = value;}
		}
		private int _CellCount;
				
		public Cell[] Cells
		{
			get { return _Cells;  }
			set { _Cells = value; }
		}
		private Cell[] _Cells;
	}
}

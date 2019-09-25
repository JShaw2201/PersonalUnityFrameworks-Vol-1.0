using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelTool.Model
{
	public class Excel :IExcel<Row,Cell>
	{
		public int ColCount
		{
			get { return _ColCount; }
			set { _ColCount = value; }
		}

		private int _ColCount;
		
		public int RowCount {
			get { return _RowCount; }
			set { _RowCount = value; }
		}

		private int _RowCount;
		
		public string Name {
			get { return _Name; }
			set { _Name = value; }
		}

		private string _Name;
		
		public Row[] Rows {
			get { return _Rows; }
			set { _Rows = value; }
		}

		private Row[] _Rows;

	}
}

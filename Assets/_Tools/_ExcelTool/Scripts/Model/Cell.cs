using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelTool.Model
{
	public class Cell : ICell
	{
		public string Colunm
		{
			get { return _Colunm; }
			set { _Colunm = value; }
		}
		private string _Colunm;
		
		public string Value {
			get { return _Value; }
			set { _Value = value; }
		}
		private string _Value;
	}
}

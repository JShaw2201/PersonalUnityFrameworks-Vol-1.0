using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelTool.Model
{
	public interface IExcel<T1,T2> where T1: IRow<T2> where T2 :ICell
	{
		int ColCount{ get; set; }
		int RowCount{ get; set; }
		string Name { get; set; } 
		T1[] Rows { get; set; }

	}
}

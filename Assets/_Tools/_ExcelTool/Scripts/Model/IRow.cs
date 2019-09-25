using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelTool.Model
{
	public interface IRow<T> where T:ICell
	{
		int CellCount{ get; set; }	
		T[] Cells{ get; set; }	
	}
}

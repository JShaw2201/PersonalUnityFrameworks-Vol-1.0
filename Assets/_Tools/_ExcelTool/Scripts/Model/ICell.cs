using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelTool.Model
{
	public interface ICell
	{
		string Colunm { get; set; }	
		string Value{ get; set; }

	}
}

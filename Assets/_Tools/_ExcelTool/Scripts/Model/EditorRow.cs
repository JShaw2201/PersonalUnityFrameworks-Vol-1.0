using System.Collections;
using System.Collections.Generic;
using ExcelTool.Tool;
using UnityEngine;

namespace ExcelTool.Model
{
	public class EditorRow : IRow<EditorCell>
	{
		public int CellCount
		{
			get { return _CellCount; }
			set { _CellCount = value; }
		}

		private int _CellCount;
		
		public EditorCell[] Cells
		{
			get { return _Cells; }
			set { _Cells = value; }
		}

		private EditorCell[] _Cells;

		public int StartCol;
		public bool CanExport = false;
		
		public EditorRow()
		{
			_CellCount = 0;
			_Cells = new EditorCell[0];
		}

		public EditorRow(Row _row)
		{
			_CellCount = _row.CellCount;
			_Cells = new EditorCell[_row.Cells.Length];
			for (int i = 0; i < _row.Cells.Length; i++)
			{
				_Cells[i] = new EditorCell(_row.Cells[i]);
			}
		}

		public void AddNewCell(string Colunm)
		{
			List<EditorCell> list = new List<EditorCell>(_Cells);
			EditorCell cell = new EditorCell();
			cell.Colunm = Colunm;
			list.Add(cell);
			_Cells = list.ToArray();
		}

		public void SetExportRowExport(bool canExport)
		{
			CanExport = canExport;
		}

		public void SetExportColunmExport(int colunm,bool canExport)
		{
			if (colunm >= StartCol && colunm < Cells.Length && Cells.Length > 0)
			{
				EditorCell cell = (EditorCell)_Cells[colunm];
				cell.CanExport = canExport;
			}
		}
		
		public void SetExportColunmName(int colunm,string exportColunmId)
		{
			if (colunm >= StartCol && colunm < Cells.Length && Cells.Length > 0)
			{
				EditorCell cell = (EditorCell)_Cells[colunm];
				cell.ExportColId= exportColunmId;
			}
		}
		
		public LitJson.JsonData GetExportJson()
		{
			LitJson.JsonData jd = new LitJson.JsonData();
			for (int i = 0; i < Cells.Length; i++)
			{
				EditorCell cell = (EditorCell)_Cells[i];
				if (cell != null && cell.CanExport)
				{
					jd[cell.ExportColId] = cell.Value;
				}
			}
			return jd;
		}
		
		public System.Xml.XmlElement GetExportXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement xe = XmlTool.NewXmlNode(doc,"table");
			for (int i = 0; i < Cells.Length; i++)
			{
				EditorCell cell = (EditorCell)_Cells[i];
				if (cell != null && cell.CanExport)
				{
					xe.SetAttribute(cell.ExportColId,cell.Value);
				}
			}
			return xe;
		}
	}
}

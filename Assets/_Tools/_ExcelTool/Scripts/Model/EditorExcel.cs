using System.Collections;
using System.Collections.Generic;
using ExcelTool.Tool;
using UnityEngine;

namespace ExcelTool.Model
{
	public class EditorExcel : IExcel<EditorRow,EditorCell>
	{
		public int ColCount
		{
			get { return _ColCount; }
			set { _ColCount = value; }
		}
		private int _ColCount;
		
		public int RowCount
		{
			get { return _RowCount; }
			set { _RowCount = value;}
		}
		private int _RowCount;
		
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
		private string _Name;
		
		public EditorRow[] Rows 
		{ 
			get { return _Rows; }
			set { _Rows = value; } 
		}
		private EditorRow[] _Rows;
	
		public int StartRow
		{
			get { return _StartRow; }
			set {
				if (_StartRow != value)
				{
					for (int i = value; i < Rows.Length; i++)
					{
						EditorRow row = _Rows[i];
						if (row != null)
							row.StartCol = _StartCol;
					}
				}
				_StartRow = value;
			}
		}
		public int StartCol
		{
			get { return _StartCol; }
			set {
				if (_StartCol != value)
				{
					for (int i = StartRow; i < Rows.Length; i++)
					{
						EditorRow row = _Rows[i];
						if (row != null)
							row.StartCol = value;
					}
				}

				_StartCol = value;
			}
		}
		
		private int _StartCol = 0;
		private int _StartRow = 1;
		public string ExportName = "root";
		public EditorExcel(Excel excel)
		{
			_Name = excel.Name;
			_ColCount = excel.ColCount;
			_RowCount = excel.RowCount;
			_Rows = new EditorRow[excel.Rows.Length];
			for (int i = 0; i < excel.Rows.Length; i++)
			{
				EditorRow row = new EditorRow(excel.Rows[i]);
				row.StartCol = _StartCol;
				row.CanExport = i >= _StartRow;
				_Rows[i] = row;
			}
		}

		public void AddNewColunm()
		{
			_ColCount++;
			for (int i = 0; i < _Rows.Length; i++)
			{
				EditorRow row = _Rows[i];
				row.CellCount = ColCount;
				row.AddNewCell("");
			}
		}
		public void AddNewRow()
		{
			_RowCount++;
			EditorRow row = new EditorRow();
			row.CellCount = _ColCount;
			row.Cells = new EditorCell[_ColCount];
			List<EditorRow> list = new List<EditorRow>(_Rows);
			for (int i = 0; i < ColCount; i++)
			{
				EditorCell cell = new EditorCell();
				row.Cells[i] = cell;
			}
		}

		public void SetColunmName(int ColIndex,string Name)
		{
			if(ColIndex < 0)
				return;
			for (int i = 0; i < _Rows.Length; i++)
			{
				EditorRow row = _Rows[i];
				if(row == null)
					continue;
				if (row.Cells.Length > 0 && ColIndex < row.Cells.Length)
				{
					ICell cell = row.Cells[ColIndex];
					cell.Colunm = Name;
				}
			}
		}

		public void SetCellValue(int rowIndex, int cellIndex ,string _value)
		{
			if (rowIndex >= 0 && rowIndex < Rows.Length && Rows.Length > 0 && cellIndex >= 0)
			{
				EditorRow _row = _Rows[rowIndex];
				if (_row != null && _row.Cells.Length > 0 && cellIndex < _row.Cells.Length)
				{
					EditorCell cell = _row.Cells[cellIndex];
					if(cell!=null)
						cell.Value = _value;
				}
			}
		}
		
		public void SetExportRowExport(int rowIndex,bool canExport)
		{
			if (rowIndex >= 0 && rowIndex < Rows.Length && Rows.Length > 0)
			{
				EditorRow _row = _Rows[rowIndex];
				if(_row != null)
					_row.SetExportRowExport(canExport);
			}
		}
		
		public void SetExportColunmExport(int colunm,bool canExport)
		{
			for (int i = StartRow; i < _Rows.Length; i++)
			{
				Rows[i].SetExportColunmExport(colunm,canExport);
			}
		}
		
		public void SetExportColunmName(int colunm,string exportColunmId)
		{
			for (int i = StartRow; i < _Rows.Length; i++)
			{
				Rows[i].SetExportColunmName(colunm,exportColunmId);
			}
		}

		public string GetExportJson()
		{
			LitJson.JsonData root = new LitJson.JsonData();	
			LitJson.JsonData jd = new LitJson.JsonData();
			for (int i = 0; i < Rows.Length; i++)
			{
				EditorRow row = _Rows[i];
				if(row == null || !row.CanExport)
					continue;
				jd.Add(row.GetExportJson());
			}
			root[ExportName] = jd;
			return root.ToJson();
		}
		
		public string GetExportXml()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			System.Xml.XmlElement root = XmlTool.NewXmlNode(doc, doc, ExportName);
			for (int i = 0; i < Rows.Length; i++)
			{
				EditorRow row = _Rows[i];
				if(row == null || !row.CanExport)
					continue;
			
				System.Xml.XmlElement xe = row.GetExportXml(doc);
				XmlTool.SetParentXmlNode(root,xe);
			}
			return ExcelFunction.ConvertXmlToString(doc);
		}
	}
}

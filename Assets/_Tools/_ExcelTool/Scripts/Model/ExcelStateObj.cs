using System.Collections;
using System.Collections.Generic;
using ExcelTool.Tool;
using UnityEngine;

namespace ExcelTool.Model
{
	public class ExcelStateObj
	{
		public int StartRow
		{
		get { return _StartRow; }
		set {
			if (_StartRow != value)
			{
				for (int i = value; i < Rows.Length; i++)
				{
					RowStateObj row = Rows[i];
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
						RowStateObj row = Rows[i];
						if (row != null)
							row.StartCol = value;
					}
				}

				_StartCol = value;
			}
		}
		private int _StartCol = 0;
		private int _StartRow = 1;
		public int ColCount;
		public int RowCount;
		public string ExportName = "root";
		public string ExcelName;
		public string ModelName;
		public RowStateObj[] Rows;

		public Dictionary<int, int> modelDict;
		public string[] ColunmModel;
		public ExcelStateObj(int _RowCount,int _ColCount)
		{
			this.ColCount = _ColCount;
			this.RowCount = _RowCount;
			Rows = new RowStateObj[_RowCount];
		}

		public void SetExportRowExport(int rowIndex,bool canExport)
		{
			if (rowIndex >= 0 && rowIndex < Rows.Length && Rows.Length > 0)
			{
				RowStateObj _row = Rows[rowIndex];
				if(_row != null)
					_row.SetExportRowExport(canExport);
			}
		}
		
		public void SetExportColunmExport(int colunm,bool canExport)
		{
			for (int i = StartRow; i < Rows.Length; i++)
			{
				Rows[i].SetExportColunmExport(colunm,canExport);
			}
		}
		
		public void SetExportColunmName(int colunm,string exportColunmId)
		{
			for (int i = StartRow; i < Rows.Length; i++)
			{
				Rows[i].SetExportColunmName(colunm,exportColunmId);
			}
		}

		public string GetExportJson()
		{
			LitJson.JsonData jd = new LitJson.JsonData();
			for (int i = StartRow; i < Rows.Length; i++)
			{
				RowStateObj row = Rows[i];
				if(row == null || !row.CanExport)
					continue;
				jd.Add(row.GetExportJson());
			}
			return jd.ToJson();
		}
		
		public string GetExportXml()
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			System.Xml.XmlElement root = XmlTool.NewXmlNode(doc, doc, ExportName);
			for (int i = StartRow; i < Rows.Length; i++)
			{
				RowStateObj row = Rows[i];
				if(row == null || !row.CanExport)
					continue;
			
				System.Xml.XmlElement xe = row.GetExportXml(doc);
				XmlTool.SetParentXmlNode(root,xe);
			}

			return ExcelFunction.ConvertXmlToString(doc);
		}
	}		
}

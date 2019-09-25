using System.Collections;
using System.Collections.Generic;
using ExcelTool.Tool;
using UnityEngine;

namespace ExcelTool.Model
{
	public class RowStateObj
	{
		public int StartCol;
		public int CellCount;
		public bool CanExport = true;
		public CellStateObj[] Cells;

		public RowStateObj(int _CellCount)
		{
			CellCount = _CellCount;
			Cells = new CellStateObj[_CellCount];
		}

		public void SetExportRowExport(bool canExport)
		{
			CanExport = canExport;
		}

		public void SetExportColunmExport(int colunm,bool canExport)
		{
			if (colunm >= StartCol && colunm < Cells.Length && Cells.Length > 0)
			{
				CellStateObj cell = Cells[colunm];
				cell.CanExport = canExport;
			}
		}
		
		public void SetExportColunmName(int colunm,string exportColunmId)
		{
			if (colunm >= StartCol && colunm < Cells.Length && Cells.Length > 0)
			{
				CellStateObj cell = Cells[colunm];
				cell.ExportColunmId= exportColunmId;
			}
		}
		
		public LitJson.JsonData GetExportJson()
		{
			LitJson.JsonData jd = new LitJson.JsonData();
			for (int i = 0; i < Cells.Length; i++)
			{
				CellStateObj cell = Cells[i];
				if (cell != null && cell.CanExport)
				{
					jd[cell.ExportColunmId] = cell.Value;
				}
			}
			return jd;
		}
		
		public System.Xml.XmlElement GetExportXml(System.Xml.XmlDocument doc)
		{
			System.Xml.XmlElement xe = XmlTool.NewXmlNode(doc,"table");
			for (int i = 0; i < Cells.Length; i++)
			{
				CellStateObj cell = Cells[i];
				if (cell != null && cell.CanExport)
				{
					xe.SetAttribute(cell.ExportColunmId,cell.Value);
				}
			}
			return xe;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using Excel;
using ExcelTool.Model;
using ExcelTool.Tool;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace ExcelTool.Editor
{
	public class ExportConfig : EditorWindow
	{
		private int currentTable = 0;
		private string excelName = "";	
		private ExcelStateObj selectTable = null;
		private string[] tables;
		private Dictionary<string, int> colDict;
		private Dictionary<string, ExcelStateObj> excelDict;
        Vector2 _scrollPos;
        private void OnEnable()
		{
			tables = new string[0];
			colDict = new  Dictionary<string, int>();
			excelDict = new Dictionary<string, ExcelStateObj>();

		}

		[MenuItem("Window/ExcelTool")]
		static void Open()
		{
            //GetWindow<ExportConfig>();
            ExportConfig myWindow = (ExportConfig)EditorWindow.GetWindow(typeof(ExportConfig), false, "ExcelTool", true);//创建窗口
			myWindow.minSize = new Vector2(960,720);
			myWindow.Show();//展示
		}

		void OnGUI()
		{
			//EditorGUILayout.RectField("position:", position);
			DrawExcel();
			EditorGUILayout.Space();			
			if(tables==null|| excelDict.Count == 0 || tables.Length==0 || currentTable<0 || currentTable >= tables.Length)
				return;
			if(tables.Length!=excelDict.Count)
				return;
			DrawSelect();
			EditorGUILayout.Space();
			DrawModel();
			EditorGUILayout.Space();
			DrawTable();
			EditorGUILayout.Space();
			DrawExport();
		}

		private void DrawExport()
		{
			if(selectTable == null)
				return;
			if (GUILayout.Button("ExportJson"))
			{
				string wUrl = EditorUtility.SaveFilePanel("ExportJson","",""+"new File.json","json");
				ExcelFunction.WriteLocal(selectTable.GetExportJson(),0,wUrl);
			}
			
			EditorGUILayout.Space();
			
			if (GUILayout.Button("ExportXml"))
			{
				string wUrl = EditorUtility.SaveFilePanel("ExportXml","",""+"new File.xml","xml");
				ExcelFunction.WriteLocal(selectTable.GetExportXml(),0,wUrl);
			}
		}

		private void DrawExcel()
		{
			float wid = position.width;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Excel:",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3));
			GUILayout.Label(excelName,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3));
			if (GUILayout.Button("Browse",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3)))
			{
				string furl =EditorUtility.OpenFilePanel("选择文件", "new File","");
				readExcel(furl);
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawModel()
		{
			if(selectTable == null)
				return;
			float wid = position.width;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Model:",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3));
			GUILayout.Label(selectTable.ModelName,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3));
			if (GUILayout.Button("Browse",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3)))
			{
				string furl=EditorUtility.OpenFilePanel("选择文件","new File","");
				readModel(furl,selectTable);
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawSelect()
		{
			float wid = position.width;//EditorGUIUtility.currentViewWidth;
			EditorGUILayout.BeginHorizontal();			
			GUILayout.Label("SelectTable:",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3));
			GUILayout.Label(tables[currentTable],GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3));
			int _tableIndex = currentTable;
			_tableIndex = EditorGUILayout.Popup(currentTable,tables,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/3));
			if (selectTable == null || _tableIndex != currentTable)
				selectTable = excelDict[tables[_tableIndex]];
			currentTable = _tableIndex;
			EditorGUILayout.EndHorizontal();
		}

		private void DrawTable()
		{
			ExcelStateObj table = selectTable;
			if(table == null)
				return;
			if(table.modelDict == null)
				table.modelDict = new Dictionary<int, int>();
			if(table.ColunmModel == null)
				return;
			float wid = position.width;
			int StartRow = table.StartRow;
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Start Row:",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/4));
			StartRow = EditorGUILayout.IntField(table.StartRow,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/4));
			int StartCol = table.StartCol;
			GUILayout.Label("Start Colunm:",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/4));
			StartCol = EditorGUILayout.IntField(table.StartCol,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/4));
						
			if (StartRow != table.StartRow)
			{
				table.StartRow = StartRow;
			}
			if (StartCol != table.StartCol)
			{
				table.StartCol = StartCol;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/(table.ColCount+1-table.StartCol)));
			RowStateObj row = table.Rows[0];
			for (int j = table.StartCol; j < table.ColCount; j++)
			{
				CellStateObj cell = row.Cells[j];
				EditorGUILayout.LabelField(cell.Value,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/(table.ColCount+1-table.StartCol)));
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("",GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/(table.ColCount+1-table.StartCol)));
			for (int j = table.StartCol; j < table.ColCount; j++)
			{
				if (!table.modelDict.ContainsKey(j))
					table.modelDict[j] = 0;
				int isShow = table.modelDict[j];
				isShow=EditorGUILayout.Popup(isShow,table.ColunmModel,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/(table.ColCount+1-table.StartCol)));				
				if (isShow != table.modelDict[j])
				{
					if (isShow != 0)
					{
						table.SetExportColunmExport(j,true);
						table.SetExportColunmName(j,table.ColunmModel[isShow]);
					}
					else
					{
						table.SetExportColunmExport(j,false);
					}					
				}

				table.modelDict[j] = isShow;
			}
			EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            for (int i = table.StartRow; i < table.RowCount; i++)
			{
				RowStateObj _row = table.Rows[i];
				DrawRow(_row);
			}
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

        }

		private void DrawRow(RowStateObj row)
		{
			if (row == null)
				return;
			float wid = position.width;
			EditorGUILayout.BeginHorizontal();
			row.CanExport = EditorGUILayout.Toggle(row.CanExport,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/(row.CellCount+1-row.StartCol)));
			for (int i = row.StartCol; i < row.CellCount; i++)
			{
				CellStateObj cell = row.Cells[i];
				string _value = cell == null ||string.IsNullOrEmpty(cell.Value) ? "" : cell.Value;
				EditorGUILayout.LabelField(_value,GUILayout.MinWidth(0),GUILayout.MaxWidth(wid/(row.CellCount+1-row.StartCol)));
			}
			EditorGUILayout.EndHorizontal();
		}

		private void readExcel(string url)
		{
			if(string.IsNullOrEmpty(url))
				return;
			if(excelDict == null)
				excelDict = new Dictionary<string, ExcelStateObj>();
			excelDict.Clear();
			excelName = System.IO.Path.GetFileNameWithoutExtension(url);
			List<ExcelStateObj> excelList = ExcelFunction.ReadExcel (url);
			for (int i = 0; i < excelList.Count; i++)
			{
				excelDict[excelList[i].ExcelName] = excelList[i];
			}
			List<string> list = new List<string>(excelDict.Keys);
			tables = list.ToArray();
			currentTable = 0;
			
			//Debug.Log(excelDict.Count+"--"+tables.Length);
		}
		
		private void readModel(string url,ExcelStateObj table)
		{
			if(string.IsNullOrEmpty(url))
				return;
			table.ModelName = System.IO.Path.GetFileNameWithoutExtension(url);
			FileInfo fi = new FileInfo(url);
			string content = ExcelFunction.LoadFileStr(url);
			switch (fi.Extension)
			{
				case ".xml" :
				case ".XML" :
				case ".Xml" :
					readModelXmlFile(content,table);
					break;
				case ".json" :
				case ".JSON" :
				case ".Json" :
					readModelJsonFile(content,table);
					break;
			}
		}
		
		private void readModelJsonFile(string content,ExcelStateObj table)
		{
			List<string> ColIdList = new List<string>();
			ColIdList.Clear();
			colDict.Clear();
			ColIdList.Add("None");
			JsonData jd = JsonMapper.ToObject(content);
            if (jd.Count == 0)
                return;
			List<string> list = new List<string>(jd[0].Keys);
			foreach (var attrName in list)
			{
				if (!string.IsNullOrEmpty(attrName) && !ColIdList.Contains(attrName))
				{
					ColIdList.Add(attrName);
					colDict[attrName] = 0;
				}
			}

			table.ColunmModel = ColIdList.ToArray();
		}
		
		private void readModelXmlFile(string content,ExcelStateObj table)
		{
			
			List<string> ColIdList = new List<string>();
			ColIdList.Clear();
			colDict.Clear();
			ColIdList.Add("None");
			TextReader tr = new StringReader(content);
			XmlReader reader = new XmlTextReader(tr);
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (!string.IsNullOrEmpty(reader.LocalName))
					{
						for (int i = 0; i < reader.AttributeCount; i++)
						{
							reader.MoveToAttribute(i);							
							string attrName = reader.Name;
							if (!string.IsNullOrEmpty(attrName) && !ColIdList.Contains(attrName))
							{
								ColIdList.Add(attrName);
								colDict[attrName] = 0;
							}
						};
					}
				}
			}
									
			table.ColunmModel = ColIdList.ToArray();
			//Debug.Log(table.ColunmModel);
		}
	}
}

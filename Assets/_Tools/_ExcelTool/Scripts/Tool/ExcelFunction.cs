using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Excel;
using ExcelTool.Model;
using UnityEngine;

namespace ExcelTool.Tool
{
	public class ExcelFunction
	{
		public static List<ExcelStateObj> ReadExcel(string path)  
		{  
			MemoryStream memory = new MemoryStream(LoadFilebytes(path));
			List<ExcelStateObj> list  = new List<ExcelStateObj>();
			IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(memory);
			do{
				// sheet name
				//Debug.Log(excelReader.Name);
				List<RowStateObj> rowList = new List<RowStateObj>();
				int rowCount = 0;
				int colCount = 0;
				while (excelReader.Read())
				{
                    bool isAdd = false;
					RowStateObj row = new RowStateObj(excelReader.FieldCount);
					colCount = Mathf.Max(excelReader.FieldCount, colCount);
					for (int i = 0; i < colCount; i++) {
						CellStateObj cell = new CellStateObj();
						row.Cells[i] = cell;
						if (i < excelReader.FieldCount)
						{
							string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
							cell.Value = value;
                            if (!string.IsNullOrEmpty(value))
                            {
                                isAdd = true;
                            }
                            
						}
					}
                    if (isAdd)
                    {
                        rowCount++;
                        rowList.Add(row);
                    }
				}
				ExcelStateObj excelStateObj = new ExcelStateObj(rowCount,colCount);
				excelStateObj.ExcelName = excelReader.Name;
				excelStateObj.Rows = rowList.ToArray();
				list.Add(excelStateObj);
			}while(excelReader.NextResult());
			//DataSet result = excelReader.AsDataSet(); 
			excelReader.Close();
			excelReader.Dispose();
			memory.Close();
			memory.Dispose();
			return list;  
		}

		
		public static string LoadFileStr(string path)
		{
			string str = "";
			if (!File.Exists(path))
				return null;
			StreamReader sr = new StreamReader(path, Encoding.GetEncoding("gb2312"));
			str = sr.ReadToEnd();
			if (string.IsNullOrEmpty(str))
				return null;
			sr.Dispose();
			sr.Close();
			return str;
		}
		public static string ConvertXmlToString(XmlDocument xmlDoc)
		{
			MemoryStream stream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(stream, null);
			writer.Formatting = Formatting.Indented;
			xmlDoc.Save(writer); 
			StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
			stream.Position = 0;
			string xmlString = sr.ReadToEnd();
			sr.Close();
			stream.Close(); 
			return xmlString;
		}

		public static byte[] LoadFilebytes(string path)
		{
			if (!File.Exists(path))
				return null;

			FileStream fs = File.OpenRead(path);
			byte[] bytes = new byte[fs.Length];
			fs.Read(bytes, 0, bytes.Length);

			// 设置当前流的位置为流的开始   
			fs.Seek(0, SeekOrigin.Begin);
			fs.Dispose();
			fs.Close();

			return bytes;
		}
		
		public static void WriteLocal(string text, int offset, string url, bool ReWrite = true)
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
			WriteLocal(buffer, offset, url, ReWrite);
		}
		public static void WriteLocal(byte[] buffer, int offset, string url, bool ReWrite = true)
		{
			//文件流信息
			FileStream fs;
			FileInfo t = new FileInfo(url);
			if (!t.Exists)
			{
				string str = Path.GetDirectoryName(url);
               
				if (!Directory.Exists(str))
					Directory.CreateDirectory(str);
				fs = new FileStream(url, FileMode.Create);
			}
			else
			{
				if (!ReWrite)
				{
					fs = new FileStream(url, FileMode.Open, FileAccess.ReadWrite);
					fs.Seek(0, SeekOrigin.End);
				}
				else
					fs = new FileStream(url, FileMode.Create);
			}

        

			// fs = new FileStream(url, FileMode.Append,FileAccess.Write);
			// fs.Position = fs.Length;
			//以行的形式写入信息
			fs.Write(buffer, offset, buffer.Length);
			//关闭流
			fs.Close();
			//销毁流
			fs.Dispose();
		}
	}
}

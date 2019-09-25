
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class FileTool{

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
    public static string LoadFileStr(string path)
    {
        string str = "";
        if (!File.Exists(path))
            return null;
        StreamReader sr = new StreamReader(path, Encoding.GetEncoding("gb2312"));
        str = sr.ReadToEnd();
        if (string.IsNullOrEmpty(str))
            return null;
        return str;
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
      
}
	


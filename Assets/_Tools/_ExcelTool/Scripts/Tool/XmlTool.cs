using UnityEngine;
using System.Collections;
using System.Xml;
using UnityEngine.Events;

namespace ExcelTool.Tool
{
	public class XmlTool
	{
		public static void SetParentXmlNode(XmlNode parentnode,XmlNode node)
		{
			if(parentnode != null)
				parentnode.AppendChild(node);
		}

		public static XmlElement GetXmlNode(XmlDocument doc, XmlNode parentnode, string nodename,
			string defaultvalue = "")
		{

			XmlElement xe = parentnode[nodename];
			if (xe == null)
			{
				xe = XmlTool.NewXmlNode(doc, parentnode, nodename, defaultvalue);
			}

			return xe;
		}

		public static XmlElement GetXmlNode(ref bool issave, XmlDocument doc, XmlNode parentnode, string nodename,
			string defaultvalue = "")
		{

			XmlElement xe = parentnode[nodename];
			if (xe == null)
			{
				xe = XmlTool.GetXmlNode(doc, parentnode, nodename, defaultvalue);
				issave = true;

			}

			return xe;
		}

		public static XmlElement NewXmlNode(XmlDocument doc,string _name)
		{			
			return NewXmlNode(doc,null,_name,null);
		}
		
		public static XmlElement NewXmlNode(XmlDocument doc,string _name, string _value )
		{			
			return NewXmlNode(doc,null,_name,_value);
		}

		public static XmlElement NewXmlNode(XmlDocument doc, XmlNode parentnode, string _name)
		{
			return NewXmlNode(doc,parentnode,_name,null);
		}

		public static XmlElement NewXmlNode(XmlDocument doc, XmlNode parentnode, string _name, string _value)
		{
			XmlElement xe = doc.CreateElement(_name);
			if (!string.IsNullOrEmpty(_value))
				xe.InnerText = _value;
			if(parentnode != null)
				parentnode.AppendChild(xe);
			return xe;
		}

		public static void SetObjValue(object obj, UnityAction NullCall, UnityAction NotNullCall)
		{
			if (obj != null)
			{
				if (NotNullCall != null)
					NotNullCall();
			}
			else
			{
				if (NullCall != null)
					NullCall();
			}
		}
	}
}

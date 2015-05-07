using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MonitorSystemClient
{
    /// <summary>
    /// 操作Xml类型
    /// AddLocalData    添加本地数据
    /// AddinterData    添加网络数据
    /// RemovrLocalData 移除本地数据
    /// RemoveinterData 移除网络数据
    /// </summary>
    public enum XmlType
    {
        AddLocalData = 1,
        AddinterData = 2,
        RemovrLocalData = 3,
        RemoveinterData = 4
    };

    /// <summary>
    /// XMl文件操作
    /// </summary>
   public class OperaXml
    {
       /// <summary>
       /// 默认xml路径
       /// </summary>
        private static string xmlPath = @"../../data/data.xml";

        /// <summary>
        /// 获取xml内容
        /// </summary>
        /// <returns>读取到的数据</returns>
        public static IList<MonitorCameraTreeModel> GetXmlData()
        {
            IList<MonitorCameraTreeModel> treeList = new List<MonitorCameraTreeModel>();
            try
            {
                // 加载xml文件
                XmlDocument doc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                // 忽略文档里面的注释
                settings.IgnoreProcessingInstructions = true;
                XmlReader reader = XmlReader.Create(xmlPath, settings);
                doc.Load(reader);
                //doc.Load("data.xml");

                // 得到根节点videoInfo
                XmlNode xn = doc.SelectSingleNode("videoInfo");

                // 得到根节点的所有子节点
                XmlNodeList xnList = xn.ChildNodes;
                foreach (var xnl in xnList)
                {
                    MonitorCameraTreeModel camerModel = new MonitorCameraTreeModel();
                    // 将节点转换为元素，便于得到节点的属性值
                    XmlElement xe = (XmlElement)xnl;
                    // 得到Type和id两个属性的属性值
                    camerModel.Id = xe.GetAttribute("id").ToString();
                    // 得到Book节点的所有子节点
                    XmlNodeList xnl0 = xe.ChildNodes;
                    camerModel.Name = xnl0.Item(0).InnerText;
                    camerModel.IsChecked = Convert.ToBoolean(xnl0.Item(2).InnerText);
                    camerModel.IsExpanded = Convert.ToBoolean(xnl0.Item(3).InnerText);
                    if (xnl0.Item(4).HasChildNodes)
                    {
                        XmlNodeList xnl1 = xnl0.Item(4).ChildNodes;
                        foreach (var xnl2 in xnl1)
                        {
                            MonitorCameraTreeModel child = new MonitorCameraTreeModel();

                            XmlElement xe1 = (XmlElement)xnl2;
                            XmlNodeList xnl3 = xe1.ChildNodes;
                            child.Id = xnl3.Item(0).InnerText;
                            child.Name = xnl3.Item(1).InnerText;
                            child.VideoPath = xnl3.Item(2).InnerText;
                            child.Parent = camerModel;
                            camerModel.Children.Add(child);
                        }
                    }

                    treeList.Add(camerModel);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw new MyException("解析xml文件出错");
            }
            return treeList;
        }

       /// <summary>
       /// 插入子节点数据
       /// </summary>
       /// <param name="model">待添加xml数据</param>
       /// <param name="type">添加类型</param>
        public static void AddDataToXml(XmlModel model,XmlType type)
        {
            // 加载xml文件
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            // 忽略文档里面的注释
            settings.IgnoreProcessingInstructions = true;
            XmlReader reader = XmlReader.Create(xmlPath, settings);
            doc.Load(reader);

            // 得到根节点videoInfo
            XmlNode xn = doc.SelectSingleNode("videoInfo");

            // 得到根节点的所有子节点
            // XmlNodeList xnList = xn.ChildNodes;
            XmlNode xnFnodel = null;
            if (type == XmlType.AddLocalData)
            {
                xnFnodel = xn.FirstChild.LastChild;
            }
            else if (type == XmlType.AddinterData)
            {
                xnFnodel = xn.LastChild.LastChild;
            }

            // 建立一个节点
            XmlElement newTreeModel = doc.CreateElement("children");
            //newTreeModel.SetAttribute("childId", model.ChildId);
            //newTreeModel.SetAttribute("childName", model.ChildName);
            //newTreeModel.SetAttribute("videopath", model.Videopath);
            // newTreeModel.InnerText = "childId";
            XmlElement chilId = doc.CreateElement("childId");
            chilId.InnerText = model.ChildId;
            XmlElement name = doc.CreateElement("childName");
            name.InnerText = model.ChildName;
            XmlElement videoPath = doc.CreateElement("videopath");
            videoPath.InnerText = model.Videopath;
            newTreeModel.AppendChild(chilId);
            newTreeModel.AppendChild(name);
            newTreeModel.AppendChild(videoPath);
            xnFnodel.AppendChild(newTreeModel);

            reader.Close();
            doc.Save(xmlPath);
        }

       /// <summary>
       /// 删除数据
       /// </summary>
       /// <param name="model">待删除的xml</param>
       /// <param name="type">删除类型</param>
        public static void RemoveXmlData(XmlModel model, XmlType type)
        {
            // 加载xml文件
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            // 忽略文档里面的注释
            settings.IgnoreProcessingInstructions = true;
            XmlReader reader = XmlReader.Create(xmlPath, settings);
            doc.Load(reader);

            // 得到根节点videoInfo
            XmlNode xn = doc.SelectSingleNode("videoInfo");

            // 得到根节点的所有子节点
            // XmlNodeList xnList = xn.ChildNodes;
            XmlNode xnFnodel = null;
            if (type == XmlType.RemovrLocalData)
            {
                xnFnodel = xn.FirstChild.LastChild;
            }
            else if (type == XmlType.RemoveinterData)
            {
                xnFnodel = xn.LastChild.LastChild;
            }

            XmlNodeList xmlNodeList = xnFnodel.ChildNodes;
            foreach (var xmlNode in xmlNodeList)
            {
                 XmlElement xe1 = (XmlElement)xmlNode;
                 XmlNodeList xnl3 = xe1.ChildNodes;
                 if (xnl3.Item(1).InnerText == model.ChildName)
                 {
                     xe1.ParentNode.RemoveChild(xe1);
                 }
            }

            reader.Close();
            doc.Save(xmlPath);
        }
    }
}

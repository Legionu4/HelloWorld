using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace sys_passport_configurations
{
    public static class Configurator
    {
        public static string cfgFile = AppDomain.CurrentDomain.BaseDirectory + "configs.xml";

        public static string GetCfg(cfgType cfg)
        {
            XmlDocument d = new XmlDocument();
            d.LoadXml(File.ReadAllText(Configurator.cfgFile).Replace("\n", "").Replace("\r", ""));
            var elements = d.GetElementsByTagName(cfg.ToString());

            var a = elements[0].FirstChild;
            return (a != null) ? string.IsNullOrEmpty(a.Value) ? "" : a.Value.Trim() : "";
        }

        public static void updateCfg(cfgType cfg, string value)
        {
            XmlDocument d = new XmlDocument();
            d.Load(cfgFile);

            XmlNodeList nodes = d.GetElementsByTagName(cfg.ToString());
            foreach (XmlElement el in nodes)
                if (el.InnerText != value)
                {
                    el.InnerText = value;
                    d.Save(cfgFile);
                }
        }

        public enum cfgType
        {
            BaseDir = 0
        }
    }
}

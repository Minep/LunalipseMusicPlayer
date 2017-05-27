using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace I18N
{
    public class I18NHelper
    {
        public volatile static I18NHelper _I18N_INSTANCE = null;
        public static readonly object I18N_OBJ = new object();

        Languages langs;

        IDictionary<string, PageLang> _applang;
        public static I18NHelper INSTANCE
        {
            get
            {
                if (_I18N_INSTANCE == null)
                {
                    lock (I18N_OBJ)
                    {
                        if (_I18N_INSTANCE == null)
                        {
                            return _I18N_INSTANCE = new I18NHelper();
                        }
                    }
                }
                return _I18N_INSTANCE;
            }
        }

        private I18NHelper()
        {
            _applang = new Dictionary<string, PageLang>();
        }

        public Languages LANGUAGES
        {
            get
            {
                return langs;
            }
            set
            {
                langs = value;
            }
        }

        public bool parseLangFile()
        {
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.IgnoreComments = true;
            string final = "";
            switch (langs)
            {
                case Languages.CHINESE:
                    if (File.Exists("i18n/CHN.lnpl")) final = "i18n/CHN.lnpl";
                    else return false;
                    break;
                case Languages.ENGLISH:
                    if (File.Exists("i18n/ENG.lnpl")) final = "i18n/ENG.lnpl";
                    else return false;
                    break;
                case Languages.RUSSIAN:
                    if (File.Exists("i18n/RUS.lnpl")) final = "i18n/RUS.lnpl";
                    else return false;
                    break;
                default:
                    final = "i18n/CHN.lnpl";
                    break;
            }
            doc.Load(XmlReader.Create(final, xrs));
            try
            {
                XmlNode xmln = doc.SelectSingleNode("Lunalipse");
                foreach (XmlNode xn in xmln.ChildNodes)
                {
                    XmlElement xe = (XmlElement)xn;
                    string name = xe.GetAttribute("CompntName");
                    PageLang pl = new PageLang();
                    foreach (XmlNode xn_ in xe.ChildNodes)
                    {
                        XmlElement xe_ = (XmlElement)xn_;
                        pl.AddToLang(xe_.Name, xe_.InnerText);
                    }
                    _applang.Add(name, pl);
                }
                return true;
            }
            catch (Exception e)
            {
                //LogFile.WriteLog("ERROR", e.Message);
                return false;
            }
        }

        public PageLang GetReferrence(string ComponentName)
        {
            return _applang[ComponentName];
        }

        public void AddReferrence(string inx,PageLang pl)
        {
            _applang.Add(inx, pl);
        }
    }

    public class PageLang
    {
        IDictionary<string, string> _lang;
        public PageLang()
        {
            _lang = new Dictionary<string, string>();
        }

        public void AddToLang(string k, string v)
        {
            _lang.Add(k, v);
        }

        public string GetContent(string indexer)
        {
            return _lang[indexer];
        }

        public IDictionary<string, string> AllLang
        {
            get { return _lang; }
        }
    }
}

using I18N;
using LunalipseAPI;
using LunalipseAPI.Generic;
using LunalipseAPI.Graphics;
using LunalipseAPI.LunalipxPlugin;
using LunalipseAPI.PlayMode;
using NewMediaPlayer.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NewMediaPlayer.PluginHoster
{
    public delegate void Loaded(string p1);
    public delegate void UnLoaded(string p2);
    public class PluginHelper
    {
        public static volatile PluginHelper _PH_INSTANCE;
        public static readonly object _OBJ = new object();

        public event Loaded PluginLoaded;
        public event UnLoaded PluginUnload;

        PageLang pl;
        IDictionary<string, PluginEntity> _methods;
        IDictionary<string, LunalipsePluginInfo> INFO;
        IDictionary<string, Assembly> asms;
        ArrayList LunapxExtend = new ArrayList();
        ArrayList LpxCustomMode = new ArrayList();

        I18NBridge i18nb;

        public UIDrawable UIDW;

        private IDictionary<string, bool> IsActivated;
        public static PluginHelper INSTANCE
        {
            get
            {
                if (_PH_INSTANCE == null)
                {
                    lock(_OBJ)
                    {
                        if(_PH_INSTANCE == null)
                        {
                            return _PH_INSTANCE = new PluginHelper();
                        }
                    }
                }
                return _PH_INSTANCE;
            }
        }

        public IDictionary<string, LunalipsePluginInfo> Plugins
        {
            get
            {
                return INFO;
            }
        }

        private PluginHelper()
        {
            if (!Directory.Exists("plugins")) Directory.CreateDirectory("plugins");
            pl = I18NHelper.INSTANCE.GetReferrence("plugin");
            INFO = new Dictionary<string, LunalipsePluginInfo>();
            _methods = new Dictionary<string, PluginEntity>();
            IsActivated = new Dictionary<string, bool>();
            asms = new Dictionary<string, Assembly>();
            UIDW = new UIDrawable();
            i18nb = new I18NBridge(this);
        }

        public ArrayList LUNAPXEXTEND
        {
            get
            {
                return LunapxExtend;
            }
        }

        public ArrayList LUNA_MODEEXTEND
        {
            get
            {
                return LpxCustomMode;
            }
        }

        public void GetPluginList()
        {
            try
            {
                foreach (string s in Directory.GetFiles("plugins\\"))
                {
                    if (!Path.GetExtension(s).Equals(".lxpg")) continue;
                    string ___p = Path.GetFileNameWithoutExtension(s);
                    asms.Add(___p,Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory+s));
                    foreach (Type t in asms[___p].GetExportedTypes())
                    {
                        Attribute a;
                        if ((a = t.GetCustomAttribute(typeof(LunalipsePluginInfo))) != null)
                        {                         
                            INFO.Add(___p, (LunalipsePluginInfo)a);
                            IsActivated.Add(___p, false);
                            _methods.Add(___p, null);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source);
            }
        }

        public IDictionary<string, bool> ACTIVATED
        {
            get
            {
                return IsActivated;
            }
        }

        public IDictionary<string, PluginEntity> ENTITIES
        {
            get
            {
                return _methods;
            }
        }

        bool Found;
        public void LoadPlugin(string plugin)
        {
            try
            {
                foreach (Type t in asms[plugin].GetExportedTypes())
                {
                    if (t.GetInterface("MainUI") != null && t.Name.Equals("LXPLMAIN"))
                    {
                        Found = true;
                        IsActivated[plugin] = true;
                        _methods[plugin] = new PluginEntity();
                        ParseFunction(t, plugin);
                        PluginLoaded(plugin);
                        break;
                    }
                }
                if (!Found)
                {
                    new LDailog(controler.LunalipsContentUI.TIPMESSAGE, pl.GetContent("noEntP"), string.Format(pl.GetContent("noEntPc"), plugin + ".lxpg")).ShowDialog();
                }
            }
            catch(Exception e)
            {
                new LDailog(controler.LunalipsContentUI.TIPMESSAGE, pl.GetContent("err"), string.Format(pl.GetContent("errC"), e.Message)).ShowDialog();
            }
        }

        public void Unload(string plugin)
        {
            //asm = null;
            IsActivated[plugin] = false;
            PluginUnload(plugin);
            _methods[plugin] = null;
        }

        private void ParseFunction(Type t_,string plgName)
        {
            _methods[plgName].ENTRY = (MainUI)Activator.CreateInstance(t_);
            foreach(Type __t in asms[plgName].GetExportedTypes())
            {
                Attribute abt;
                if (__t.GetInterface("ILunalipseDrawing") != null && __t.GetCustomAttribute(typeof(LunalipseDrawing))!=null)
                {
                    _methods[plgName].LDrawing = (ILunalipseDrawing)Activator.CreateInstance(__t);
                }
                else if(__t.GetInterface("FFT") != null)
                {
                    _methods[plgName].LFFT = (FFT)Activator.CreateInstance(__t);
                }
                else if (__t.GetInterface("ILunalipx") != null && (abt = __t.GetCustomAttribute(typeof(LunalipxExtend))) != null)
                {
                    if(((LunalipxExtend)abt).SupportVersion.Equals(Assembly.GetExecutingAssembly().GetName().Version.ToString()))
                    {
                        LunapxExtend.Add(plgName);
                        _methods[plgName].LPX = (ILunalipx)Activator.CreateInstance(__t);
                    }
                    else
                    {
                        new LDailog(controler.LunalipsContentUI.TIPMESSAGE, pl.GetContent("errP"), string.Format(pl.GetContent("errPC"), plgName)).ShowDialog();
                    }
                }
                else if (__t.GetInterface("IMode") != null && (abt = __t.GetCustomAttribute(typeof(LunalipseCustomMode))) != null)
                {
                    LpxCustomMode.Add(plgName);
                    _methods[plgName].LMode = (IMode)Activator.CreateInstance(__t);
                    _methods[plgName].modeI18NReq = (abt as LunalipseCustomMode).I18Npresent;
                }
            }
        }

        public void FireSequence(APIBridge s,params object[] a)
        {
            foreach(var i in _methods)
            {
                switch(s)
                {
                    case APIBridge.MUSIC_C:
                        i.Value?.ENTRY.MusicChange(a[0] as PlayInfo);
                        break;
                    case APIBridge.MODE_C:
                        i.Value?.ENTRY.ModeChange(Convert.ToInt32(a[0]));
                        break;
                    case APIBridge.VOL_C:
                        i.Value?.ENTRY.VolumeChange(Convert.ToDouble(a[0]));
                        break;
                    case APIBridge.LUNALIPSE_SHUTDOWN:
                        i.Value?.ENTRY.LunalipseExit();
                        break;
                }
            }
        }
    }
}
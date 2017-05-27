using LunalipseAPI;
using LunalipseAPI.Generic;
using LunalipseAPI.Graphics;
using LunalipseAPI.LunalipxPlugin;
using LunalipseAPI.PlayMode;
using NewMediaPlayer.Dialog;
using NewMediaPlayer.Generic;
using NewMediaPlayer.Lunalipx;
using NewMediaPlayer.PluginHoster;
using NullStudio.Utils.Keyboardhook;
using System;
using System.Collections;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace NewMediaPlayer
{
    //主窗体程序的局部类
    partial class MainWindow
    {
        public void ApplyingUIText()
        {
            savePpt.Content = PL.GetContent("saveCfg");
            setmpath.Content = PL.GetContent("MusicDir");
            programe.Content = PL.GetContent("lpxCfg");
            equzer.Content = PL.GetContent("btnList4");
            loadscript.Content = PL.GetContent("btnList3");
            Edlpx.Content = PL.GetContent("btnList2");
            netMusic.Content = PL.GetContent("btnList1");
            playmode.Content = PL.GetContent("orderPlay");
        }

        public void RegEvent()
        {
            MainUIEvent.PlayMusic += (a) =>
            {
                if (a != -1) global.SELECTED_MUSIC = (a - 1);
                Ps_opsc();
                ModeChang(false);
            };
            MainUIEvent.SetPlayMode += (a) =>
            {
                global.PLAY_MODE = a;
            };
            MainUIEvent.SetVolume += (a) =>
            {
                vol_adj.Value = a;
            };
            PH.PluginLoaded += (x) =>
            {
                PH.ENTITIES[x].ENTRY.Initialize();
                LogFile.WriteLog("INFO", "Plugin：" + x + " load successfully.");
                PH.ENTITIES[x].ENTRY.GrabMusicList(al);
                PH.ENTITIES[x].LDrawing?.InitialDraw();
                PH.ENTITIES[x].LPX?.LPxInitialize();
                PH.ENTITIES[x].LMode?.BeingInitialize();

            };
            PH.PluginUnload += (x) =>
            {
                LogFile.WriteLog("INFO", "Plugin：" + x + " unload successfully.");
                PH.ENTITIES[x]?.ENTRY.Destroy();
            };

            DrawingManager.OnDrawButton += (lt, lb) =>
            {
                PH.UIDW.DrawButton(this, lb, lt);
            };
            DrawingManager.OnDrawLabel += (lt, ll) =>
            {
                PH.UIDW.DrawLabel(this, ll, lt);
            };
            DrawingManager.OnDrawControl += (lt, c) =>
            {
                PH.UIDW.DrawVanilla(this, lt, c);
            };

            LunalipxEx.RCommand += (c) =>
            {
                try
                {
                    if (Command.cmd.IndexOf(c) == -1) Command.cmd.Add(c);
                    else return false;
                    return true;
                }
                catch { return false; }
            };

            LunalipxEx.URCommand += (c) =>
            {
                try
                {
                    Command.cmd.Remove(c);
                    return true;
                }
                catch { return false; }
            };

            LunalipxEx.EScript += (a) =>
            {
                Command.SCRIPT_range = a > 6 ? 6 : a;
            };

            ModeManager.RegMODE += (a, b) =>
            {
                CustomModeCollection.collection.Add(new CustomMode { Identification = a, Key = b });
                Command.MAX_MODE++;
                return true;
            };

            LunalipseContainer.OpenD += (a, b, c) =>
            {
                try
                {
                    new LDailog(a, b, c).ShowDialog();
                    return true;
                }
                catch
                {
                    return false;
                }
            };

            LunalipseContainer.ODeft += (a, b) =>
            {
                try
                {
                    new LDailog(a, b).ShowDialog();
                    return true;
                }
                catch
                {
                    return false;
                }
            };
        }

        void ModeChang(bool __)
        {
            if (global.PLAY_MODE < Command.MAX_MODE && __)
            {
                global.PLAY_MODE++;
            }
            else if (__)
            {
                global.PLAY_MODE = 0;
            }
            switch (global.PLAY_MODE)
            {
                case 0:
                    playmode.Content = PL.GetContent("orderPlay");
                    break;
                case 1:
                    playmode.Content = PL.GetContent("singleLoop");
                    break;
                case 2:
                    playmode.Content = PL.GetContent("randomPlay");
                    break;
                default:
                    if(global.PLAY_MODE<= Command.SCRIPT_range)
                        playmode.Content = string.Format(PL.GetContent("program"), (global.PLAY_MODE - 2));
                    else
                    {
                        CustomMode cm = CustomModeCollection.collection[global.PLAY_MODE - (Command.SCRIPT_range+1)] as CustomMode;
                        foreach (string s in PH.LUNA_MODEEXTEND)
                        {
                            if(PH.ENTITIES[s].modeI18NReq)
                            {
                                playmode.Content = PL.GetContent(cm.Key) ?? "NAN";
                            }
                            else
                            {
                                playmode.Content = cm.Key ?? "NAN";
                            }
                        }
                    }
                    break;
            }
            PH.FireSequence(APIBridge.MODE_C, global.PLAY_MODE);
        }

        public void Ps_opsc()
        {

            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
            MusicList.Dispatcher.Invoke(new Action(() =>
            {
                switch (global.PLAY_MODE)
                {
                    case 0:
                        if (global.SELECTED_MUSIC < MusicList.Items.Count)
                        {
                            global.SELECTED_MUSIC++;
                        }
                        else
                        {
                            global.SELECTED_MUSIC = 0;
                        }
                        loadscript.IsEnabled = false;
                        break;
                    case 1:
                        Random r = new Random();
                        global.SELECTED_MUSIC = r.Next(0, MusicList.Items.Count - 1);
                        loadscript.IsEnabled = false;
                        break;
                    case 2:
                        loadscript.IsEnabled = false;
                        // paly cycle. Do nothing.
                        break;
                    default:
                        if(global.PLAY_MODE<Command.SCRIPT_range)
                        {
                            loadscript.IsEnabled = true;
                            int pnum = global.PLAY_MODE - 2;
                            bool isSuccess = false;
                            if (sp == null)
                            {
                                sp = new SyntaxParser(pnum, MusicList.Items.Count);
                                isSuccess = sp.Parse();
                            }
                            else if (sp.GetProgrameSeq != pnum)
                            {
                                sp = new SyntaxParser(pnum, MusicList.Items.Count);
                                isSuccess = sp.Parse();
                            }
                            else isSuccess = true;
                            if (isSuccess && pressed)
                            {
                                sp.PROGRAME_COUNTER = 0;
                                sp.Excute(MusicList);
                                pressed = false;
                            }
                            else if (isSuccess && !pressed)
                            {
                                sp.Excute(MusicList);
                            }
                            else { global.PLAY_MODE = 0; global.SELECTED_MUSIC = 0; }
                            OnExecuteCompletely?.Invoke();
                        }
                        else
                        {
                            CustomMode cm = CustomModeCollection.collection[global.PLAY_MODE - (Command.SCRIPT_range + 1)] as CustomMode;
                            foreach (string s in PH.LUNA_MODEEXTEND)
                            {
                                PH.ENTITIES[s].LMode?.ModeBehavior(ref global.SELECTED_MUSIC, 
                                    cm.Identification
                                    );
                            }
                        }
                        break;

                }
                MusicList.SelectedIndex = global.SELECTED_MUSIC;
            }));
        }
        public void GetList()
        {
            if (String.IsNullOrEmpty(global.MUSIC_PATH)) return;
            if (!Directory.Exists(global.MUSIC_PATH)) return;
            al = new ArrayList();
            foreach (string fi in Directory.GetFiles(global.MUSIC_PATH))
            {
                string _ = Path.GetFileName(fi);
                MusicList.Items.Add(_);
                al.Add(_);
            }
        }

        private void Ps_OnSoundLoadedComplete(double newDuration)
        {
            duration.Dispatcher.Invoke(new Action(() =>
            {
                //Reset the value to 0
                duration.Value = 0;
                //Applying new duration
                duration.Maximum = newDuration;
                Mainbtn.Fill = new SolidColorBrush(Color.FromArgb(193, 217, 83, 79));
                PlayInfo pi = new PlayInfo();
                pi.Name = MusicList.Items[global.SELECTED_MUSIC].ToString();
                pi.musicPath = global.MUSIC_PATH + @"\" + pi.Name;
                pi.TotalDuration = (long)newDuration;
                PH.FireSequence(APIBridge.MUSIC_C, pi);
            }));
        }

        private void Ps_OnProgressUpdated(long newPosition)
        {
            duration.Dispatcher.Invoke(new Action(() =>
            {
                //Update the thumb
                duration.Value = newPosition;
            }));
        }

        bool isPrivate = false;
        public void RegistKeyPress()
        {
            kh = new key_hook();
            kh.KeyDownEvent += (s, e) =>
            {
                if (e.KeyValue == (int)System.Windows.Forms.Keys.P && (int)System.Windows.Forms.Control.ModifierKeys == (int)System.Windows.Forms.Keys.Alt)
                {
                    if (!isPrivate)
                    {
                        isPrivate = true;
                        bf.BeginAnimation(BlurEffect.RadiusProperty, new DoubleAnimation(0, 15, TimeSpan.FromSeconds(2.5)));
                    }
                    else
                    {
                        isPrivate = false;
                        bf.BeginAnimation(BlurEffect.RadiusProperty, new DoubleAnimation(15, 0, TimeSpan.FromSeconds(2.5)));
                    }
                }
                else if ((int)System.Windows.Forms.Control.ModifierKeys == (int)System.Windows.Forms.Keys.Alt)
                {
                    if (e.KeyValue == (int)System.Windows.Forms.Keys.S)
                    {
                        Mainbtn.Fill = ps.ChangePlayStatus() ? new SolidColorBrush(Color.FromArgb(193, 92, 184, 92)) : new SolidColorBrush(Color.FromArgb(193, 217, 83, 79));
                    }
                }
            };
            kh.Start();
        }

        //进入私密模式
        BlurEffect bf = new BlurEffect();
        private void InitialPrivateMode()
        {
            bf.KernelType = KernelType.Gaussian;
            bf.Radius = 0;
            outershell.Effect = bf;
        }

    }
}

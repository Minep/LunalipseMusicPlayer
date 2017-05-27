using CSCore.Streams.Effects;
using NewMediaPlayer.PluginHoster;
using NewMediaPlayer.Sound;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace NewMediaPlayer.Lunalipx
{
    public class SyntaxParser
    {
        PluginHelper PH;
        ArrayList commands = new ArrayList();
        
            //{
            //"luna.play",
            //"luna.setVol",
            //"luna.setEquer",
            //"LLOOP",
            //"REP",
            //"PASS",
            //"#RAND",
            //"luna.playN",
            //"NightmareMoon"};
        string fname;
        Regex rx = new Regex(@"(?is)(?<=\()[^\)]+(?=\))");
        int programe = 0, songsc = 0;
        Random r;
        //循环次数计数器
        int REP_COUNTER = 0;
        //程序指针累计器
        int PRG_COUNTER = 0;
        //REP(COMMAND(TIMES))
        public SyntaxParser(int prg,int songsC)
        {
            programe = prg;
            songsc = songsC;
            r = new Random();
            PH = PluginHelper.INSTANCE;
            Command.cmd.Add("luna.play");
            Command.cmd.Add("luna.setVol");
            Command.cmd.Add("luna.setEquer");
            Command.cmd.Add("LLOOP");
            Command.cmd.Add("REP");
            Command.cmd.Add("PASS");
            Command.cmd.Add("#RAND");
            Command.cmd.Add("luna.playN");
            Command.cmd.Add("NightmareMoon");
        }

        public int GetProgrameSeq
        {
            get
            {
                return programe;
            }
        }

        public bool Parse()
        {
            int ix = 0;
            string line;
            string path = "Scripts/prg" + programe + ".lunapx";
            fname = Path.GetFileName(path);
            using (StreamReader sr = new StreamReader(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    ix++;
                    try
                    {
                        if(line.IndexOf('!')!=0)
                        {
                            commands.Add(parseStatment(line, ix));
                        }
                    }
                    catch(Exception e)
                    {
                        Error ee = new Error(e.Message.Split('|'));
                        LogFile.WriteLog("ERROR", e.Message);
                        ee.ShowDialog();
                        return false;
                    }
                }
                LogFile.WriteLog("INFO", "Load " + ix + " lines in " + fname);
            }
            return true;
        }

        private LUNALIPS_Expression parseStatment(string s, int line)
        {
            bool NonUsual = false;
            bool allMiss = true;
            foreach(string i in Command.cmd)
            {
                if (s.Contains(i)) { allMiss = false; break; }
            }
            if(allMiss)
            {
                allMiss = false;
                throw new Exception("Lunapx Syntax Error | " + s + "| Command not found.| At lines " + line + " in " + fname);
            }

            LUNALIPS_Expression lnx_exp = new LUNALIPS_Expression();
            lnx_exp.hasREP = s.StartsWith("REP");
            if(!rx.IsMatch(s))
            {
                if (!s.Contains("()"))
                {
                    throw new Exception("Lunapx Syntax Error | " + s + "| Bracket not in pattern or invalid character.| At lines " + line + " in " + fname);
                }
                else NonUsual = true;
            }
            MatchCollection m = rx.Matches(s);
            if (s.Equals("PASS"))
            {
                lnx_exp.exe_CMD = TranslateCMD("PASS");
                return lnx_exp;
            }
            foreach(Match mt in m)
            {
                if(lnx_exp.hasREP)
                {
                    if (mt.Value.Contains("("))
                    {
                        string[] spt = mt.Value.Split('(');
                        lnx_exp.exe_CMD = TranslateCMD(spt[0]);
                        if(lnx_exp.exe_CMD == Command.LUNA_PLAYN)
                        {
                            if(spt[1].Equals("#RAND"))
                            {
                                lnx_exp.SongID = r.Next(songsc);
                            }
                            else
                            {
                                lnx_exp.SongID = Convert.ToInt32(spt[1]);
                            }
                        }
                        else if (lnx_exp.exe_CMD == Command.LUNA_PLAYS)
                        {
                            lnx_exp.songsName = spt[1];
                        }
                    }
                    else
                    {
                        lnx_exp.REP_Times = Convert.ToInt32(mt.Value);
                    }
                }
                else
                {
                    int CurrentCmd = TranslateCMD(s.Split('(')[0]);
                    switch(CurrentCmd)
                    {
                        case Command.LUNA_PLAYS:
                            lnx_exp.songsName = mt.Value;                 
                            break;
                        case Command.LUNA_PLAYN:
                            if(mt.Value.Contains("#RAND"))
                            {
                                lnx_exp.SongID = r.Next(songsc);
                            }
                            else
                            {
                                try
                                {
                                    lnx_exp.SongID = Convert.ToInt32(mt.Value);
                                }
                                catch(FormatException)
                                {
                                    throw new Exception("Lunapx Type Error|" + s + "|Error type. Integer expected!|At lines " + line + " in " + fname);
                                }
                            }
                            break;
                        case Command.LUNA_SETVOL:
                            try
                            {
                                double d = Convert.ToDouble(mt.Value);
                                if(d>1||d<0)
                                {
                                    throw new Exception("Lunapx Argument Error|" + s + "|Invalid Arguments. Must between 1 and 0 !|At lines " + line + " in " + fname);
                                }
                                lnx_exp.Vol = d;
                            }
                            catch (FormatException)
                            {
                                throw new Exception("Lunapx Type Error|" + s + "|Error type. Double expected !|At lines " + line + " in " + fname);
                            }
                            break;
                        case Command.LUNA_SETEQU:
                            int[] tmpered = new int[10];
                            try
                            {
                                string[] equzer_s = mt.Value.Split(',');
                                if (equzer_s.Length < 10)
                                    throw new Exception("Lunapx Arguments Error|" + s + "|Invalid Arguments. Ten arguments need to be presented. !|At lines " + line + " in " + fname);
                                for(int i=0;i<equzer_s.Length;i++)
                                {
                                    int j = Convert.ToInt32(equzer_s[i]);
                                    if(j>100||j<-100)
                                        throw new Exception("Lunapx Argument Error|" + s + "|Invalid Arguments. Must between 100 and -100 !|At lines " + line + " in " + fname);
                                    tmpered[i] = j;
                                }
                                lnx_exp.equerz = tmpered;
                            }
                            catch(FormatException)
                            {
                                throw new Exception("Lunapx Type Error|" + s + "|Error type. Integer expected !|At lines " + line + " in " + fname);
                            }
                            break;
                        case Command.LUNA_SHUTDOWN_COM:
                            lnx_exp.isShutdownREQ = true;
                            break;
                        

                    }
                    lnx_exp.exe_CMD = CurrentCmd;
                }
            }
            if (NonUsual) lnx_exp.exe_CMD = TranslateCMD(s.Replace("()",""));
            return lnx_exp;
        }

        private int TranslateCMD(string cmd_)
        {
            switch(cmd_)
            {
                case "luna.play":
                    return Command.LUNA_PLAYS;
                case "luna.setVol":
                    return Command.LUNA_SETVOL;
                case "luna.setEquer":
                    return Command.LUNA_SETEQU;
                case "PASS":
                    return Command.LUNA_PASS;
                case "luna.playN":
                    return Command.LUNA_PLAYN;
                case "LLOOP":
                    return Command.LUNA_LLOOP;
                case "NightmareMoon":
                    return Command.LUNA_SHUTDOWN_COM;
                default:
                    return EXTEND_FUNC(cmd_);
            }
        }

        private int EXTEND_FUNC(string c)
        {
            foreach(string s in PH.LUNAPXEXTEND)
            {
                int a;
                if ((a = PH.ENTITIES[s].LPX.LunalipxMethod(c)) != 0x0000)
                {
                    return a;
                }
                else continue;
            }
            return 0x0000;
        }


        public void Excute(ListBox LB)
        {
            bool HAS_SELF_INCR = false;
            LUNALIPS_Expression exp = commands[PRG_COUNTER] as LUNALIPS_Expression;
            if(exp.exe_CMD==Command.LUNA_LLOOP)
            {
                //重置指针
                PRG_COUNTER = 0;
                //重新应用指针
                exp = commands[PRG_COUNTER] as LUNALIPS_Expression;
            }
            if(PRG_COUNTER>=commands.Count)
            {
                global.PLAY_MODE = 0;
                global.SELECTED_MUSIC = 1;
                return;
            }
            switch(exp.exe_CMD)
            {
                case Command.LUNA_PLAYS:
                    int test = LB.Items.IndexOf(exp.songsName);
                    if(test==-1)
                    {
                        LogFile.WriteLog("ERROR", "LUNAPX - Music " + exp.songsName + " not found. Check the spelling or file existing.");
                        LogFile.WriteLog("INFO", "LUNAPX - Move to next instruction");
                        PRG_COUNTER++;
                        Excute(LB);
                    }
                    else
                    {
                        global.SELECTED_MUSIC = test;
                    }
                    break;
                case Command.LUNA_SETVOL:
                    global.MUSIC_VOLUME = (float)exp.Vol;
                    PRG_COUNTER++;
                    Excute(LB);
                    break;
                case Command.LUNA_PASS:
                    if(global.SELECTED_MUSIC==LB.Items.Count-1)
                    {
                        global.SELECTED_MUSIC = 0;
                    }
                    else
                    {
                        global.SELECTED_MUSIC++;
                    }
                    break;
                case Command.LUNA_PLAYN:
                    global.SELECTED_MUSIC = exp.SongID;
                    break;
                case Command.LUNA_SETEQU:
                    for(int i=0;i<10;i++)
                    {
                        appliedEquzer(i, exp.equerz[i]);
                    }
                    global.EQUALIZER_SAVE = exp.equerz;
                    PRG_COUNTER++;
                    Excute(LB);
                    break;
                case Command.LUNA_SHUTDOWN_COM:
                    callNightmareMoon();
                    break;
                default:
                    foreach(string s in PH.LUNAPXEXTEND)
                    {
                        HAS_SELF_INCR = PH.ENTITIES[s].LPX.LunalipxBehavior(exp.exe_CMD, ref PRG_COUNTER, ref global.SELECTED_MUSIC);
                    }
                    if(!HAS_SELF_INCR)
                    {
                        PRG_COUNTER++;
                        Excute(LB);
                    }
                    break;
            }
            if (exp.hasREP)
            {
                REP_COUNTER++;
                if (REP_COUNTER >= exp.REP_Times)
                {
                    REP_COUNTER = 0;
                    PRG_COUNTER++;
                }
            }
            else
            {
                if(PRG_COUNTER==0)
                {
                    PRG_COUNTER++;
                }
                else if((commands[PRG_COUNTER-1] as LUNALIPS_Expression).exe_CMD != Command.LUNA_SETVOL ||
                    (commands[PRG_COUNTER - 1] as LUNALIPS_Expression).exe_CMD != Command.LUNA_SETEQU)
                {
                    PRG_COUNTER++;
                }
            }
        }

        private void callNightmareMoon()
        {
            Process.Start("shutdown.exe", "-s -t 00");
        }

        private void appliedEquzer(int inx,int val)
        {
            if (PlaySound.equzer != null)
            {
                double perc = ((double)val / 100d);
                var value = (float)(perc * 20);
                EqualizerFilter ef = PlaySound.equzer.SampleFilters[inx];
                ef.AverageGainDB = value;
            }
        }

        public int PROGRAME_COUNTER
        {
            get
            {
                return PRG_COUNTER;
            }
            set
            {
                if (value < 1) return;
                else PRG_COUNTER = value - 1;
            }
        }

        ~SyntaxParser()
        { }
    }
}

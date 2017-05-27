using LunalipseAPI.Generic;
using LunalipseAPI.LunalipxPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestPlugin
{
    [LunalipxExtend(SupportVersion = "1.1.2.0")]
    public class LunapxMethodExt : ILunalipx
    {
        public void LPxInitialize()
        {
            LunalipxEx.RegsitCommand("SAYHello");
        }

        public bool LunalipxBehavior(int command, ref int PGCounter, ref int SELMusic)
        {
            switch (command)
            {
                case 0xE000:
                    MessageBox.Show("HELLO!");
                    return false;
            }
            return true;
        }

        public int LunalipxMethod(string Method)
        {
            switch(Method)
            {
                case "SAYHello":
                    return 0xE000;
            }
            return LunalipxInst.NOT_DEFINE;
        }
    }
}

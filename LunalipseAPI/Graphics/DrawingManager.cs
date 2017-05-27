using LunalipseAPI.Graphics.Generic;
using System;
using System.Windows.Controls;

namespace LunalipseAPI.Graphics
{
    public delegate void DrawButton(LunalipseTarget lt,LButton lb);
    public delegate void DrawLabel(LunalipseTarget lt, LLabel lb);
    public delegate void DrawControls(LunalipseTarget lt, Control cc);
    public class DrawingManager
    {
        public static event DrawButton OnDrawButton;
        public static event DrawLabel OnDrawLabel;
        public static event DrawControls OnDrawControl;
        
        
        public static void Luna_DrawButton(LunalipseTarget LT,LButton LB)
        {
            OnDrawButton(LT, LB);
            
        }
        public static void Luna_DrawLabel(LunalipseTarget LT, LLabel LB)
        {
            OnDrawLabel(LT, LB);
        }

        public static void Luna_DrawMisc(LunalipseTarget lt,Control ctrl)
        {
            OnDrawControl(lt, ctrl);
        }
    }
}

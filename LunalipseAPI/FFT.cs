using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LunalipseAPI
{
    public interface FFT
    {
        void Spectrum(ImageSource iss);
        void SoundData(float left, float right);
    }
}

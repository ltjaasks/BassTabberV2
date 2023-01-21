using NAudio.Wave;
using NWaves.Audio;
using NWaves.FeatureExtractors.Options;
using NWaves.FeatureExtractors;
using NWaves.Features;
using NWaves.Signals;
using NWaves.Utils;
using NWaves.FeatureExtractors.Multi;
using System;
using System.Text;
using System.Globalization;

namespace BassTabberV2
{
    public partial class BassTabber : Form
    {
        WaveFileWriter writer = null;
        string kansio;
        string tiedosto;
        NAudio.Wave.WaveInEvent aaniIn;

        DiscreteSignal signal;
        double[] taajuudet;
        double[] tiivistetytTaajuudet;

        StringBuilder[] tabSb = new StringBuilder[4];

        readonly int[][] oteLauta = new int[][]
        {
            new int[] {98, 104, 110, 117, 123, 131, 139, 147, 156, 165, 175, 185, 196, 208, 220, 233, 247, 262, 277, 294, 311},
            new int[] {73, 78, 82, 87, 92, 98, 104, 110, 117, 123, 131, 139, 147, 156, 165, 175, 185, 196, 208, 220, 233, 247},
            new int[] {55, 58, 62, 65, 69, 73, 78, 82, 87, 92, 98, 104, 110, 117, 123, 131, 139, 147, 156, 165, 175, 185, 196},
            new int[] {41, 44, 46, 49, 52, 55, 58, 62, 65, 69, 73, 78, 82, 87, 92, 98, 104, 110, 117, 123, 131, 139, 147, 156}
        };


        public BassTabber()
        {
            InitializeComponent();

            /// Alustetaan kieli‰ kuvaava tabSb[]
            for (int i = 0; i < tabSb.Length; i++)
            {
                tabSb[i] = new StringBuilder("");
            }
                
            /// Luodaan tallennustiedosto
            kansio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            Directory.CreateDirectory(kansio);
            tiedosto = Path.Combine(kansio, "recorded.wav");

            /// Luodaan ‰‰nilaite
            aaniIn = new WaveInEvent();

            bool closing = false;

            /// Tapahtumank‰sittelij‰ ‰‰nilaitteelle
            aaniIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
                if (writer.Position > aaniIn.WaveFormat.AverageBytesPerSecond * 30)
                {
                    aaniIn.StopRecording();
                }
            };

            aaniIn.RecordingStopped += (s, a) =>
            {
                writer?.Dispose();
                writer = null;
                if (closing)
                {
                    aaniIn.Dispose();
                }
            };

            FormClosing += (s, a) =>
            {
                closing = true;
                aaniIn.StopRecording();
            };
        }


        /// <summary>
        /// Laskee taajuudet luodun PitchExtractorin avulla ja asettaa ne taulukkoon taajuudet
        /// </summary>
        private void laskeTaajuudet()
        {
            WaveFileReader wf = new WaveFileReader(tiedosto);
            int tiedostonPituus = wf.TotalTime.Seconds;

            var opts = new PitchOptions
            {
                SamplingRate = signal.SamplingRate,
                FrameDuration = 0.04,
                LowFrequency = 40,
                HighFrequency = 300
            };
            var extractor = new PitchExtractor(opts);
            var pitches = extractor.ComputeFrom(signal);
            float[][] taajuudetVali = pitches.ToArray();
            taajuudet = new double[taajuudetVali.Length];
            for (int i = 0; i < taajuudet.Length; i++)
            {
                taajuudet[i] = taajuudetVali[i][0];
                taajuudet[i] = Math.Round(taajuudet[i]);
            }
            tiivistaTaajuudet();
        }


        /// <summary>
        /// Kirjoittaa tabin n‰kyville tab-kentt‰‰n tabSb StringBuilder taulukosta
        /// </summary>
        private void kirjoitaTab()
        {
            for (int i = 0; i < tiivistetytTaajuudet.Length; i++)
            {
                //tab.AppendText(tiivistetytTaajuudet[i].ToString() + " ");
                kirjoitaIsku(tiivistetytTaajuudet[i]);
            }
            
            for (int i = 0; i < tabSb[0].Length - 40; i += 40)
            {
                for (int j = 0; j < tabSb.Length; j++)
                {
                    tab.AppendText(tabSb[j].ToString(i, 40) + "\n");
                }
                tab.AppendText("\n\n");
            }
        }


        /// <summary>
        /// Placeholder tiivist‰j‰, jotta ohjelman toimintaa voi n‰ytt‰‰
        /// Tiivist‰‰ taajuudet, ottamalla taulukosta, joka 10. luvun, jotta tuotettu tab on luettavampi
        /// TO:DO parempi toteutus. Otetaan taajuudet taulukosta ‰‰nien keskiarvotaajuus ja korvataan pitk‰kestoiset ‰‰net yhdell‰ luvulla.
        /// Tiivistet‰‰n nollien m‰‰r‰‰, kuitenkin s‰ilytt‰en taukojen oikeat pituudet suhteessa. Tarkoituksena saada tabist‰ lyhyempi ja poistaa toistuvat luvut
        /// </summary>
        private void tiivistaTaajuudet()
        {
            tiivistetytTaajuudet = new double[taajuudet.Length / 10];
            for (int i = 0; i < tiivistetytTaajuudet.Length; i++)
            {
                tiivistetytTaajuudet[i] = taajuudet[i * 10];
            }
        }


        /// <summary>
        /// Kirjoittaa tabia tabSb[] StringBuilder -taulukkoon
        /// </summary>
        /// <param name="taajuus">taajuus, jota vastaava v‰li etsit‰‰n oteLauta -taulukosta</param>
        private void kirjoitaIsku(double taajuus)
        {
            if (taajuus == 0)
            {
                lisaaTyhjat(-1);
                return;
            }

            /// Etsit‰‰n taajuutta vastaava v‰li otelaudalta
            for (int i = 0; i < oteLauta[0].Length; i++)
            {
                for (int j = 0; j < oteLauta.Length; j++)
                {
                    if (taajuus == oteLauta[j][i])
                    {
                        tabSb[j].Append(i);
                        lisaaTyhjat(j);
                        return;
                    }
                    if (oteLauta[j][i] < taajuus && taajuus < oteLauta[j][i + 1]) /// Lis‰t‰‰n oikea v‰li oikealle kielelle
                    {
                        if (etsiLahempi(oteLauta[j][i + 1], oteLauta[j][i], taajuus))
                            tabSb[j].Append(i);
                        else
                            tabSb[j].Append(i + 1);
                        lisaaTyhjat(j); /// Lis‰t‰‰n muihin kieliin "ó". Aliohjelmana liian nestingin v‰ltt‰miseksi
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Lis‰‰ "ó" tabSb StringBuildereihin, kuin tabSb[eiLis‰t‰]. Voidaan antaa parametriksi esim -1 jos halutaan lis‰t‰ kaikkiin
        /// </summary>
        /// <param name="eiLisata">StringBuilder, johon ei lis‰t‰ "ó"</param>
        private void lisaaTyhjat(int eiLisata)
        {
            for (int i = 0; i < tabSb.Length; i++)
            {
                if (i != eiLisata)
                {
                    tabSb[i].Append("ó");
                }
            }
        }


        /// <summary>
        /// Etsii taajuutta l‰hemm‰n kahdesta oteLauta-taulukon luvusta
        /// </summary>
        /// <param name="korkea">oteLauta-taulukon suurempi luku</param>
        /// <param name="matala">oteLauta-taulukon pienempi luku</param>
        /// <param name="taajuus">taajuus, johon verrataan</param>
        /// <returns>true jos matala on l‰hemp‰n‰ taajuutta, false jos korkea on l‰hemp‰n‰ taajuutta</returns>
        private static bool etsiLahempi(int korkea, int matala, double taajuus)
        {
            return Math.Abs(taajuus - matala) < Math.Abs(taajuus - korkea);
        }


        /// <summary>
        /// Tapahtumank‰sittelij‰ start-napin painamiselle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startRecording(object sender, EventArgs e)
        {
            tab.Clear();
            tab.AppendText("Recording... Click stop for tab");
            writer = new WaveFileWriter(tiedosto, aaniIn.WaveFormat);
            aaniIn.StartRecording();
        }


        /// <summary>
        /// Tapahtumank‰sittelij‰ stop-napin painamiselle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopRecording(object sender, EventArgs e)
        {
            tab.Clear();
            aaniIn.StopRecording();
            writer.Dispose();
            writer = null;

            using (var stream = new FileStream(tiedosto, FileMode.Open))
            {
                var waveFile = new WaveFile(stream);
                signal = waveFile[Channels.Left];
            }
            laskeTaajuudet();
            kirjoitaTab();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Talkative
{
    [TestFixture]
    public class Talker
    {
        [Test]
        public void Talk() {
            // Initialize a new instance of the SpeechSynthesizer.
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {

                // Output information about all of the installed voices. 
                Console.WriteLine("Installed voices -");
                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    VoiceInfo info = voice.VoiceInfo;
                    string AudioFormats = "";
                    foreach (SpeechAudioFormatInfo fmt in info.SupportedAudioFormats)
                    {
                        AudioFormats += String.Format("{0}\n",
                            fmt.EncodingFormat.ToString());
                    }

                    Console.WriteLine(" Name:          " + info.Name);
                    Console.WriteLine(" Culture:       " + info.Culture);
                    Console.WriteLine(" Age:           " + info.Age);
                    Console.WriteLine(" Gender:        " + info.Gender);
                    Console.WriteLine(" Description:   " + info.Description);
                    Console.WriteLine(" ID:            " + info.Id);
                    Console.WriteLine(" Enabled:       " + voice.Enabled);
                    if (info.SupportedAudioFormats.Count != 0)
                    {
                        Console.WriteLine(" Audio formats: " + AudioFormats);
                    }
                    else
                    {
                        Console.WriteLine(" No supported audio formats found");
                    }

                    string AdditionalInfo = "";
                    foreach (string key in info.AdditionalInfo.Keys)
                    {
                        AdditionalInfo += String.Format("  {0}: {1}\n", key, info.AdditionalInfo[key]);
                    }

                    Console.WriteLine(" Additional Info - " + AdditionalInfo);
                    Console.WriteLine();

                    string filename = "C:\\Temp\\Sample" + new Random().Next(1, 100) + ".wav";
                    if (File.Exists(filename)) {
                        Thread.Sleep(500);
                        File.Delete(filename);
                    }
                    synth.SetOutputToWaveFile(filename);

                    synth.SelectVoice(info.Name);
                    string sml = "<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US' >";
                    synth.SpeakSsml(sml.Replace('\'', '"') + @"This is some <emphasis>VERY</emphasis> silly text</speak>");
                }
            }

        }

        [Test]
        public void Talk2() {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SelectVoice("Microsoft Zira Desktop");
            string input = File.ReadAllText(@"C:\Temp\Sample2.xml");
            ss.SpeakSsml(input);
        }
    }
}

using System;
using System.Threading;
using LSL;

namespace LSLExamples
{
    static class SendData
    {
        public static void Main(string[] args)
        {
            Random rnd = new Random();

            // create stream info and outlet
            using StreamInfo info = new StreamInfo("ecl", "EEG", 8, 100, channel_format_t.cf_float32, "sddsfsdf");
            using StreamOutlet outlet = new StreamOutlet(info);
            float[] data = new float[8];
            while (!Console.KeyAvailable)
            {
                //generate random data and send it
                //for (int k = 0; k < data.Length; k++)
                //    data[k] = rnd.Next(0, 100);
                data[0] = 67f;
                data[1] = 0.78f;
                data[2] = 0.58f;
                data[3] = 0.18f;
                data[4] = 0.88f;
                data[5] = 0.98f;
                outlet.push_sample(data);
                Thread.Sleep(10);
            }
        }
    }
}

﻿using System;
using System.Threading;
using LSL;

namespace LSLExamples
{
    static class SendDataInChunks
    {
        public static void Main(string[] args)
        {
            Random rnd = new Random();
            // create stream info and outlet
            using StreamInfo info = new StreamInfo("TestCSharp", "GSR", 8, 100, channel_format_t.cf_float32, "sddsfsdf");
            using StreamOutlet outlet = new StreamOutlet(info);

            // send data in chunks of 10 samples and 8 channels
            float[,] data = new float[10, 8];
            while (!Console.KeyAvailable)
            {
                for (int s = 0; s < 10; s++)
                    for (int c = 0; c < 8; c++)
                        data[s, c] = rnd.Next(-100, 100);
                outlet.push_chunk(data);
                Thread.Sleep(100);
            }
        }
    }
}

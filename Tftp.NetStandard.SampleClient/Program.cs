﻿using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Tftp.NetStandard.SampleClient
{
    class Program
    {
        private static AutoResetEvent TransferFinishedEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            //Setup a TftpClient instance
            var client = new TftpClient("localhost");

            //Prepare a simple transfer (GET test.dat)
            var transfer = client.Upload("data.txt", TimeSpan.FromSeconds(10));

            //Capture the events that may happen during the transfer
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinshed);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);

            string text = File.ReadAllText(Directory.GetCurrentDirectory() + "\\data.txt", Encoding.UTF8);

            //Start the transfer and write the data that we're downloading into a memory stream
            Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(text));
            transfer.Start(stream);

            //Wait for the transfer to finish
            TransferFinishedEvent.WaitOne();
            Console.ReadKey();
        }

        static void transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            Console.WriteLine("Transfer running. Progress: " + progress);
        }

        static void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            Console.WriteLine("Transfer failed: " + error);
            TransferFinishedEvent.Set();
        }

        static void transfer_OnFinshed(ITftpTransfer transfer)
        {
            Console.WriteLine("Transfer succeeded.");
            TransferFinishedEvent.Set();
        }
    }
}

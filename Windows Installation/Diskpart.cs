﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Windows_Installation
{
    class Diskpart
    {
        TextBox output;
        bool clearOutput;

        public Diskpart(TextBox newOutput)
        {
            output = newOutput;
        }

        public void formatOnePartition()
        {
            clearOutput = true;
            diskPart(new List<String>(new String[] {
                "sel disk 0",
                "clean",
                "create part prim",
                "format fs=ntfs quick",
                "act",
                "ass letter=k"
            }));
        }

        public void formatTwoPartitions(int Gb)
        {
            clearOutput = true;
            diskPart(new List<String>(new String[] {
                "sel disk 0",
                "clean",
                "create part prim size=" + Gb * 1024,
                "sel part 1",
                "format fs=ntfs quick",
                "act",
                "ass letter=k",
                "create part prim",
                "format fs=ntfs quick",
                "act",
                "ass"
            }));
        }

        public void diskInfo()
        {
            clearOutput = false;
            diskPart(new List<String>(new String[] {
                "list disk",
                "list vol"
            }));
        }

        private void diskPart(List<String> commands)
        {
            ProcessStartInfo cmdStartInfo = new ProcessStartInfo();
            cmdStartInfo.FileName = @"X:\Windows\System32\diskpart.exe";

            cmdStartInfo.RedirectStandardOutput = true;
            cmdStartInfo.RedirectStandardInput = true;
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.CreateNoWindow = true;

            Process cmdProcess = new Process();
            cmdProcess.StartInfo = cmdStartInfo;
            cmdProcess.OutputDataReceived += cmd_DataReceived;
            cmdProcess.EnableRaisingEvents = true;

            try
            {
                cmdProcess.Start();
                cmdProcess.BeginOutputReadLine();

                foreach (String command in commands) {
                    cmdProcess.StandardInput.WriteLine(command);
                }

                cmdProcess.StandardInput.WriteLine("exit");     
            }

            catch (Exception ex)
            {
                output.Text = ex.ToString();
            }
        }

        private void cmd_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                output.Dispatcher.BeginInvoke(new Action(() => {
                    if (clearOutput) output.Text = "";
                    output.Text += e.Data.Trim() + Environment.NewLine; 
                }));
            }     

        }


    }
}
using SerialManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CodeManager
{
    public class CodeCompiler
    {
        IProgress<string> internalProgress = null;
        public async Task<bool> CompileAndUploadCode(List<IPAddress> addresses, List<PacketRule> rules, string ssid, string password, IProgress<string> progress = null)
        {
            internalProgress = progress;
            string Code = CodeGenerator.FireWallCode(addresses,rules, ssid, password);
            if(Code.StartsWith("NOCODE"))
                return false;
            else
            {
                string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"FirewallOut");
                try
                {
                    var dirInfo = Directory.CreateDirectory(directoryPath);
                    if (!dirInfo.Exists)
                    {
                        internalProgress?.Report("Cannot create directory");
                        return false;
                    }
                    string filePath = Path.Combine(directoryPath, "FirewallOut.ino");
                    var file = File.Create(filePath);
                    if(!File.Exists(filePath))
                    {
                        internalProgress?.Report("Cannot create file");
                        return false;
                    }
                    file.Close();
                    File.WriteAllText(filePath, Code);
                    bool compile = await CompileCode(filePath);
                    if(!compile)
                        return false;
                    bool upload = await UploadCode(filePath);
                    return upload;


                }
                catch(Exception e)
                {
                    internalProgress?.Report("ERROR");
                    internalProgress?.Report(e.ToString());
                    return false;
                }
            }
        }

        private async Task<bool> CompileCode(string path)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = @"C:\arduino-cli\arduino-cli.exe",
                Arguments = $"compile -b esp8266:esp8266:nodemcuv2 {path}"
            };
            Process compileProcess = new Process()
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };

            compileProcess.ErrorDataReceived += UploadProcess_ErrorDataReceived;
            compileProcess.OutputDataReceived += UploadProcess_OutputDataReceived;

            await Task.Run(() =>
            {
                compileProcess.Start();
                compileProcess.BeginOutputReadLine();
                compileProcess.BeginErrorReadLine();
            }
            );

            await compileProcess.WaitForExitAsync();
            return compileProcess.ExitCode == 0;
        }
        
        private async Task<bool> UploadCode(string path)
        {
            string[] allPorts = SerialMonitor.GetAllPorts();
            ProcessStartInfo info = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = @"C:\arduino-cli\arduino-cli.exe"
            };
            Process uploadProcess = new Process()
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };

            uploadProcess.ErrorDataReceived += UploadProcess_ErrorDataReceived;
            uploadProcess.OutputDataReceived += UploadProcess_OutputDataReceived;

            foreach (string port in allPorts)
            {
                info.Arguments = $"upload -b esp8266:esp8266:nodemcuv2 -p {port} {path}";
                await Task.Run(() => 
                { 
                    uploadProcess.Start(); 
                    uploadProcess.BeginOutputReadLine(); 
                    uploadProcess.BeginErrorReadLine(); 
                });
                await uploadProcess.WaitForExitAsync();
                if (uploadProcess.ExitCode == 0)
                    return true;
            }
            return false;
        }

        private void UploadProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            internalProgress?.Report($"stdout : {e.Data}");
        }

        private void UploadProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            internalProgress?.Report($"stderr : {e.Data}");
        }
    }
}

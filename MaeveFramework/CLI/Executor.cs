using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MaeveFramework.CLI
{
    public class Executor
    {
        private Action<string> _logAction;

        public Executor(Action<string> logAction)
        {
            _logAction = logAction;
        }

        public Executor()
        {

        }

        private void Log(string text)
        {
            if (_logAction != null)
                _logAction(text);
        }

        public string Run(ProcessStartInfo startInfo, TimeSpan timeout)
        {
            using (Process process = new Process())
            {
                process.StartInfo = startInfo;

                Log($"{startInfo.FileName} {startInfo.Arguments}");

                process.Start();
                process.WaitForExit(Convert.ToInt32(timeout.TotalMilliseconds));

                using (var stream = process.StandardOutput)
                {
                    string result = stream.ReadToEnd();
                    Log(result);
                    return result;
                }
            }
        }

        public string Run(string fileName, string arguments, TimeSpan? timeout = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };

            if (!timeout.HasValue)
                timeout = TimeSpan.FromMinutes(10);

            return Run(startInfo, timeout.Value);
        }
    }
}

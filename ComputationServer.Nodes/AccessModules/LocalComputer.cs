using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Diagnostics;
using ComputationServer.Data.Interfaces;

namespace ComputationServer.Nodes.AccessModules
{
    public class LocalComputer : DataProcessingComputer
    {
        private string _workareaPath;
        private string _executableName;
        private Dictionary<string, Process> _running;

        public LocalComputer(int maxConcurrent, 
            string workareaPath,
            IMethodRepository methodRepository,
            string executableName) : base(maxConcurrent, methodRepository)
        {
            _workareaPath = workareaPath;
            _executableName = executableName;
            _running = new Dictionary<string, Process>();
        }

        protected override bool StartJob(Job job)
        {
            try
            {
                var jobDir = Path.Combine(_workareaPath, job.Guid);
                Directory.CreateDirectory(jobDir);
                var execPath = FetchExecutables(job.Name, jobDir);
                var started = Process.Start(execPath);
                _running.Add(job.Guid, started);
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        protected override bool StopJob(Job job)
        {
            var jobId = job.Guid;

            if (!_running.ContainsKey(jobId))
                return false;

            var process = _running[jobId];

            if(!process.HasExited)
                process.Kill();

            return true;
        }

        protected override Dictionary<string, Status> PollJobs(List<Job> jobs)
        {
            var result = new Dictionary<string, Status>();

            foreach (var job in jobs)
            {
                var jobId = job.Guid;

                if (!_running.ContainsKey(jobId))
                    continue;

                var process = _running[jobId];

                if (process.HasExited)
                {
                    if (process.ExitCode == 0)
                        result[jobId] = Status.COMPLETED;
                    else
                        result[jobId] = Status.FAILED;
                }
                else
                    result[jobId] = Status.RUNNING;
            }

            return result;
        }

        protected override List<MnemonicValue> FetchResults(List<Job> completed)
        {
            Console.WriteLine("FetchResults stub");
            return null;
        }

        private string FetchExecutables(string name, string path)
        {
            var method = _methodRepository.Find(name);
            var execPath = Path.Combine(path, _executableName);

            using (var execFile = File.Open(execPath, FileMode.Create))
            {
                using (var writer = new StreamWriter(execFile))
                {
                    writer.Write(method.Binary);
                }
            }

            return execPath;
        }
    }
}

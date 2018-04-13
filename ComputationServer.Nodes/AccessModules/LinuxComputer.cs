using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationServer.Data.Enums;
using System.IO;

namespace ComputationServer.Nodes.AccessModules
{
    public class LinuxComputer : ProcessingComputer
    {
        private string _domainName;
        private int _port;
        private string _username;
        private Chilkat.SshKey _privateKey;
        private string _rootPath;
        private string _charset;

        private Dictionary<string, int> _jobPids = new Dictionary<string, int>();

        public LinuxComputer(int maxConcurrent) : base(maxConcurrent, null)
        {
            _domainName = "dummy";
            _port = 22;
            _username = "dummy";
            _privateKey.FromOpenSshPrivateKey("dummy, read key from somewhere");
            _rootPath = "/home/dummy/superprocessor";
            _charset = "urf-8";
        }

        protected override bool StartJob(Job operation)
        {
            var ssh = new Chilkat.Ssh();

            if (!ssh.Connect(_domainName, _port))
                return false;

            if (!ssh.AuthenticatePk(_username, _privateKey))
                return false;

            var execPath = $"{_rootPath}/{operation.Method.Name}";
            var args = new StringBuilder($"{_rootPath}/data");

            foreach(var input in operation.Input)
                args.Append($" {input.Id}");
            
            var command = $"./{execPath} {args}";
            var pid = ssh.QuickCmdSend(command);

            if (pid < 0)
                return false;
            else
                _jobPids.Add(operation.Guid, pid);

            return true;
        }

        protected override bool StopJob(Job operation)
        {
            if (!_jobPids.ContainsKey(operation.Guid))
                return false;

            var ssh = new Chilkat.Ssh();

            if (!ssh.Connect(_domainName, _port))
                return false;

            if (!ssh.AuthenticatePk(_username, _privateKey))
                return false;

            var result = ssh.ChannelSendClose(_jobPids[operation.Guid]);

            _jobPids.Remove(operation.Guid);

            return result;
        }

        protected override Dictionary<string, ExecutionStatus> PollJobs(List<Job> jobs)
        {
            throw new NotImplementedException();
        }

        protected override List<MnemonicValue> FetchResults(List<Job> completed)
        {
            throw new NotImplementedException();
        }

        protected override bool Ping()
        {
            throw new NotImplementedException();
        }
    }
}

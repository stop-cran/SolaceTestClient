using CommandLine;
using SolaceSystems.Solclient.Messaging;

namespace SolaceTestClient
{
    public class Options
    {
        [Option('h', "host", Required = true, HelpText = "Solace host name, can be multiple.")]
        public string Host { get; set; }

        [Option("vpn", Required = true, HelpText = "Solace VPN name (case-sensitive).")]
        public string VPN { get; set; }

        [Option('u', "username", Required = true, HelpText = "Solace user name.")]
        public string userName { get; set; }

        [Option('p', "password", Required = true, HelpText = "Solace password.")]
        public string Password { get; set; }

        [Option('t', "topic", HelpText = "A topic to subscribe.")]
        public string Topic { get; set; }

        [Option("verbosity", HelpText = "Logging verbosity.", Default = SolLogLevel.Info)]
        public SolLogLevel Verbosity { get; set; }
    }
}

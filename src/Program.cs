using CommandLine;
using SolaceSystems.Solclient.Messaging;
using System;
using System.Security.Cryptography.X509Certificates;

namespace SolaceTestClient
{
    class Program
    {
        static void Main(string[] args) =>
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(DoTest)
                .WithNotParsed(e => Environment.ExitCode = 1);

        private static void DoTest(Options arguments)
        {
            ContextFactory.Instance.Init(new ContextFactoryProperties
            {
                SolClientLogLevel = arguments.Verbosity,
                LogDelegate = logInfo => Console.WriteLine(logInfo.ToString())
            });

            using (var trustStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine))
            using (var context = ContextFactory.Instance.CreateContext(
                new ContextProperties(),
                (sender, e) => Console.WriteLine(e.ToString())))
            {
                trustStore.Open(OpenFlags.ReadOnly);

                using (var session = context.CreateSession(new SessionProperties
                {
                    Host = arguments.Host,
                    VPNName = arguments.VPN,
                    UserName = arguments.userName,
                    Password = arguments.Password,
                    ReconnectRetries = 10,
                    SSLTrustStore = trustStore.Certificates
                },
                (s, e) => Console.WriteLine($"Received a message: ApplicationMessageType={e.Message.ApplicationMessageType}, CorrelationId={e.Message.CorrelationId}, binary attachment (in base64): {Convert.ToBase64String(e.Message.BinaryAttachment)}."),
                (s, e) => Console.WriteLine(e.ToString())))
                {
                    Console.WriteLine("Connection result code: " + session.Connect());

                    if (!string.IsNullOrEmpty(arguments.Topic))
                    {
                        Console.WriteLine($"Subscribing to {arguments.Topic}...");
                        session.Subscribe(ContextFactory.Instance.CreateTopic(arguments.Topic), true);
                    }

                    Console.WriteLine("Press ENTER to stop...");
                    Console.ReadLine();
                    Console.WriteLine("Disconnection result code: " + session.Disconnect());
                }
            }

            ContextFactory.Instance.Cleanup();
        }
    }
}

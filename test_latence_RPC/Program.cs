using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Stormancer;
using Stormancer.Core;

namespace test_latence_RPC
{
    class Program
    {
        static private Client _client;
        static private Scene _scene;
        static Stopwatch test = new Stopwatch();
 

        static void Main()
        {
            Console.WriteLine("connecting");
            connect().Wait();
            Console.WriteLine("connected");
            if (_scene.Connected)
                TestLoop().Wait();
        }

        private static  void sendRPC()
        {
           _scene.Rpc("test_rpc", s => { }).Subscribe(resp => receiveResponse(resp));
        }

        private static void receiveResponse(Packet<IScenePeer> packet)
        {
            test.Stop();
            Console.WriteLine("temps du RPC = " + test.ElapsedMilliseconds.ToString());
            test.Reset();
        }

        private static async Task connect()
        {
            var config = ClientConfiguration.ForAccount("7794da14-4d7d-b5b5-a717-47df09ca8492", "testlatencerpc");
            config.ServerEndpoint = "http://api.stormancer.com";
            _client = new Client(config);

            _scene = await _client.GetPublicScene("test", "");
            await _scene.Connect();
        }

        private static async Task TestLoop()
        {
            while (true)
            {
                test.Start();
                sendRPC();
                await Task.Delay(1500);
            }
        }
    }
}

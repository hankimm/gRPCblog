using System;
using System.IO;
using Grpc.Core;
using Blog;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;

namespace server
{
    class Program
    {
        const int Port = 50052;
        static void Main(string[] args)
        {
            Server server = null;
            try
            {

                var reflectionServiceImpl = new ReflectionServiceImpl(BlogService.Descriptor, ServerReflection.Descriptor);
                server = new Server()
                {
                    Services = { 
                        BlogService.BindService(new BlogServiceImpl()),
                        ServerReflection.BindService(reflectionServiceImpl)
                    },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("The Server is listening on the port: " + Port);
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine("The server failed to start: " + e.Message);
            }
            finally
            {
                if (server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
        }
    }
}

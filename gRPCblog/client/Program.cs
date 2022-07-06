using System;
using System.Threading.Tasks;
using Grpc.Core;
using Blog;


namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Channel channel = new Channel("localhost", 50052, ChannelCredentials.Insecure);
            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                    Console.WriteLine("The Client connected successfully");
            });

            var client = new BlogService.BlogServiceClient(channel);

            // var newBlog = DoCreate(client);
            // DoReadBlog(client);
            // UpdateBlog(client, newBlog);
            // DeleteBlog(client, newBlog);
            await ListBlog(client);

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }

        private static Blog.Blog DoCreate(BlogService.BlogServiceClient client)
        {
            var response = client.CreateBlog(new CreateBlogRequest()
            {
                Blog = new Blog.Blog()
                {
                    AuthorId = "Han",
                    Title = "First Blog!",
                    Content = "Hello World, this is my first blog."
                }
            });

            Console.WriteLine("The blog " + response.Blog.Id + " was created!");

            return response.Blog;
        }

        private static void DoReadBlog(BlogService.BlogServiceClient client)
        {
            try
            {
                var response = client.ReadBlog(new ReadBlogRequest()
                {
                    BlogId = "62c5f2a1f672dd90aee077a8"
                });
                Console.Write(response.Blog.ToString());
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.Status.Detail);
            }
        }

        private static void UpdateBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
        {
            try
            {
                blog.AuthorId = "Updated Test Author";
                blog.Title = "Update Test title";
                blog.Content = "Updated Test Content";
                var response = client.UpdateBlog(new UpdateBlogRequest()
                {
                    Blog = blog
                });
                Console.WriteLine(response.Blog.ToString());
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.Status.Detail);
            }
        }

        private static void DeleteBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
        {
            try
            {
                var response = client.DeleteBlog(new DeleteBlogRequest() { BlogId = blog.Id });

                Console.WriteLine("The Blog with id " + response.BlogId + " was deleted");
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.Status.Detail);
            }
        }

        private static async Task ListBlog(BlogService.BlogServiceClient client)
        {
            var response = client.ListBlog(new ListBlogRequest() { });

            while(await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Blog.ToString());
            }
        }
    }
}

﻿using System.Collections.Generic;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;

namespace Apphbify.Resources
{
    public static class StaticResources
    {
        public static byte[] FavIcon;
        public static byte[] Robots;

        static StaticResources()
        {
            FavIcon = ReadFile("favicon.ico");
            Robots = ReadFile("robots.txt");
        }

        private static byte[] ReadFile(string name)
        {
            byte[] data;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Apphbify.Resources." + name))
            {
                data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
            }
            return data;
        }
    }

    public class StaticResourceStartup : IStartup
    {
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get { return null; } }

        public IEnumerable<InstanceRegistration> InstanceRegistrations { get { return null; } }

        public IEnumerable<TypeRegistration> TypeRegistrations { get { return null; } }

        public void Initialize(IPipelines pipelines)
        {
            RegisterFile("/robots.txt", StaticResources.Robots, pipelines);
        }

        private void RegisterFile(string name, byte[] data, IPipelines pipelines)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
            {
                if (ctx.Request == null || string.IsNullOrEmpty(ctx.Request.Path))
                    return null;

                if (ctx.Request.Path.Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    var response = new Response
                    {
                        ContentType = "text/plain",
                        StatusCode = HttpStatusCode.OK,
                        Contents = s => s.Write(data, 0, data.Length)
                    };
                    response.Headers["Cache-Control"] = "public, max-age=604800, must-revalidate";
                    return response;
                }
                return null;
            });
        }
    }
}
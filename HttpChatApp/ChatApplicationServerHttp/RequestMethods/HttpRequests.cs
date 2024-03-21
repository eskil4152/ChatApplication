﻿using System;
using System.Net;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
	public class HttpRequests
	{
        public static async Task ProcessRestRequest(HttpListenerContext context, DatabaseService databaseService)
        {
            Console.WriteLine("was rest");
            try
            {
                string method = context.Request.HttpMethod;
                string path = context.Request.Url.LocalPath;

                switch (method)
                {
                    case "GET":
                        Console.WriteLine("was get");
                        await HandleGetRequest(databaseService, context, path);
                        break;
                    case "POST":
                        Console.WriteLine("was post");
                        await HandlePostRequest(databaseService, context, path);
                        break;
                    default:
                        context.Response.StatusCode = 405;
                        context.Response.Close();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                context.Response.Close();
            }
        }

        private static async Task HandleGetRequest(DatabaseService databaseService, HttpListenerContext context, string path)
        {
            switch (path)
            {
                case "/api/rooms":
                    await WriteResponse.WriteJsonResponse(context, new List<string>(), 200, null);
                    break;

                default:
                    if (path.StartsWith("/api/room/"))
                    {
                        string roomId = path["/api/room/".Length..];
                        await WriteResponse.WriteJsonResponse(context, "got room " + roomId, 200, null);
                        Console.WriteLine("Room id: {0}", roomId);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                    }
                    break;
            }
        }

        private static async Task HandlePostRequest(DatabaseService databaseService, HttpListenerContext context, string path)
        {
            string requestBody;
            LoginMessage? requestData;

            switch (path)
            {
                case "/api/login":
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }

                    try
                    {
                        requestData = JsonSerializer.Deserialize<LoginMessage>(requestBody);
                        if (requestData == null)
                        {
                            context.Response.StatusCode = 400;
                            context.Response.Close();
                            break;
                        }

                        User? user = UserActions.Login(databaseService, requestData);

                        if (user != null)
                        {
                            Cookie cookie = new("Username", user.Username);

                            await WriteResponse.WriteJsonResponse(context, user.Rooms, 200, cookie);
                        }
                        else
                        {
                            context.Response.StatusCode = 401;
                            context.Response.Close();
                        }
                    } catch
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        break;
                    }

                    break;

                case "/api/register":
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }

                    try
                    {
                        requestData = JsonSerializer.Deserialize<LoginMessage>(requestBody);
                        if (requestData == null)
                        {
                            context.Response.StatusCode = 400;
                            context.Response.Close();
                            break;
                        }

                        User? user = UserActions.Register(databaseService, requestData);

                        if (user != null)
                        {
                            await WriteResponse.WriteJsonResponse(context, user.Rooms, 200, new Cookie("Username", user.Username));
                        }
                        else
                        {
                            context.Response.StatusCode = 409;
                            context.Response.Close();
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        break;
                    }

                    break;
                default:
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    break;
            }
        }
    }
}

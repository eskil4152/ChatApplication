using System;
using System.Net;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
	public class HttpRequests
	{
        public static async Task ProcessRestRequest(HttpListenerContext context, DatabaseService databaseService)
        {
            try
            {
                string method = context.Request.HttpMethod;
                string path = context.Request.Url.LocalPath;

                switch (method)
                {
                    case "GET":
                        await HandleGetRequest(databaseService, context, path);
                        break;
                    case "POST":
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
                    Cookie? cookie = context.Request.Cookies["Username"];
                    
                    if (cookie != null)
                    {
                        await WriteResponse.WriteJsonResponse(context, databaseService.GetRooms(Cookies.DecryptCookie(cookie)), 200, null);
                    } else
                    {
                        await WriteResponse.WriteEmptyResponse(context, 401);
                    }

                    break;

                default:
                    await WriteResponse.WriteEmptyResponse(context, 404);
                    break;
            }
        }

        private static async Task HandlePostRequest(DatabaseService databaseService, HttpListenerContext context, string path)
        {
            string requestBody;
            LoginMessage? loginData;
            RoomMessage? roomData;

            switch (path)
            {
                case "/api/login":
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }

                    try
                    {
                        loginData = JsonSerializer.Deserialize<LoginMessage>(requestBody);
                        if (loginData == null)
                        {
                            await WriteResponse.WriteEmptyResponse(context, 400);
                            break;
                        }

                        User? user = UserActions.Login(databaseService, loginData);

                        if (user != null)
                        {
                            await WriteResponse.WriteJsonResponse(context, databaseService.GetRooms(user.Username), 200, Cookies.CreateCookie("Username", user.Username));
                        }
                        else
                        {
                            await WriteResponse.WriteEmptyResponse(context, 401);
                        }
                    } catch (Exception e)
                    {
                        Console.WriteLine("caught " + e.Message);
                        await WriteResponse.WriteEmptyResponse(context, 500);
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
                        loginData = JsonSerializer.Deserialize<LoginMessage>(requestBody);
                        if (loginData == null)
                        {
                            await WriteResponse.WriteEmptyResponse(context, 400);
                            break;
                        }

                        User? user = UserActions.Register(databaseService, loginData);

                        if (user != null)
                        {
                            await WriteResponse.WriteJsonResponse(context, databaseService.GetRooms(user.Username), 200, Cookies.CreateCookie("Username", user.Username));
                        }
                        else
                        {
                            await WriteResponse.WriteEmptyResponse(context, 409);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        await WriteResponse.WriteEmptyResponse(context, 400);
                        break;
                    }

                    break;

                case "/api/room/join":
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }

                    try
                    {
                        roomData = JsonSerializer.Deserialize<RoomMessage>(requestBody);
                        Cookie? cookie = context.Request.Cookies["Username"];

                        if (roomData == null || cookie == null)
                        {
                            await WriteResponse.WriteEmptyResponse(context, 400);
                            break;
                        }

                        User? user = databaseService.GetUser(Cookies.DecryptCookie(cookie));

                        if (user == null)
                        {
                            await WriteResponse.WriteEmptyResponse(context, 401);
                            break;
                        }

                        if (databaseService.JoinRoom(roomData, user))
                        {
                            await WriteResponse.WriteJsonResponse(context, "", 200, null);
                        } else
                        {
                            await WriteResponse.WriteEmptyResponse(context, 401);
                        }
                    }
                    catch
                    {
                        await WriteResponse.WriteEmptyResponse(context, 400);
                        break;
                    }
                    break;

                case "/api/room/create":
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }

                    try
                    {
                        roomData = JsonSerializer.Deserialize<RoomMessage>(requestBody);
                        Cookie? cookie = context.Request.Cookies["Username"];

                        if (roomData == null || cookie == null)
                        {
                            await WriteResponse.WriteEmptyResponse(context, 400);
                            break;
                        }

                        User? user = databaseService.GetUser(Cookies.DecryptCookie(cookie));

                        if (user == null)
                        {
                            await WriteResponse.WriteEmptyResponse(context, 401);
                            break;
                        }

                        if (databaseService.CreateRoom(roomData, user))
                        {
                            await WriteResponse.WriteJsonResponse(context, "", 200, null);
                        } else
                        {
                            await WriteResponse.WriteEmptyResponse(context, 409);
                        }
                    } catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        await WriteResponse.WriteEmptyResponse(context, 400);
                    }

                    break;
                case "/api/room/enter":
                    break;
                default:
                    await WriteResponse.WriteEmptyResponse(context, 404);
                    break;
            }
        }
    }
}


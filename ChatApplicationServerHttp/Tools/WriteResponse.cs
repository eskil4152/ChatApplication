using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ChatApplicationServerHttp
{
	public class WriteResponse
	{
        public static async Task WriteJsonResponse(HttpListenerContext context, object responseObject, int statusCode, Cookie? cookie)
        {
            string jsonResponse = JsonSerializer.Serialize(responseObject);
            byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentLength64 = responseBytes.Length;

            context = AddHeaders(context);

            if (cookie != null)
            {
                context.Response.Cookies.Add(cookie);
            }

            context.Response.StatusCode = statusCode;

            await context.Response.OutputStream.WriteAsync(responseBytes);
            context.Response.Close();
        }

        public static Task WriteEmptyResponse(HttpListenerContext context, int statusCode)
        {
            context = AddHeaders(context);

            context.Response.StatusCode = statusCode;
            context.Response.ContentLength64 = 0;
            context.Response.Close();

            return Task.CompletedTask;
        }

        public static Task WriteOptionsResponse(HttpListenerContext context)
        {
            context = AddHeaders(context);

            context.Response.StatusCode = 200;
            context.Response.Close();

            return Task.CompletedTask;
        }

        private static HttpListenerContext AddHeaders(HttpListenerContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:3000");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

            return context;
        }
    }
}


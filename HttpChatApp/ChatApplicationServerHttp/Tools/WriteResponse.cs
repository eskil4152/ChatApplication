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

            if (cookie != null)
                context.Response.Cookies.Add(cookie);

            context.Response.StatusCode = statusCode;

            await context.Response.OutputStream.WriteAsync(responseBytes);
            context.Response.Close();
        }
    }
}


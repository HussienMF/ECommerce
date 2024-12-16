using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalExcpetion(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //Declare variable
            string messege = "sorry, internal server error occured, try again";
            int statuscode = (int) HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);
                //check if exception is too many request //429 status code.
                if(context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    messege = "Too Many Request";
                    statuscode = (int) StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, messege, statuscode);
                }

                //check if exception is not authorized //401 status code.
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    messege = "you are not authorized to access";
                    statuscode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, messege, statuscode);
                }

                //check if exception is Frobidden //403 status code.
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    messege = "you are noe allowed";
                    statuscode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, messege, statuscode);
                }
            }
            catch (Exception ex)
            {
                //Log Original Exception /File, Debugger, Console
                LogException.LogExceptions(ex);

                //check if Exception is Timeout //408 request Timeout
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    messege = "Request timeout... try again";
                    statuscode = StatusCodes.Status408RequestTimeout;
                }

                //if Exception is caught.
                // if none of the Exception then do the default
                await ModifyHeader(context, title, messege, statuscode);
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string messege, int statuscode)
        {
            // display scary-free messege to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = messege,
                Status = statuscode,
                Title = title
            }), CancellationToken.None);
            return;
        }
    }
}

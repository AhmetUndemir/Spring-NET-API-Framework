using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SimpleSprint.Common;

namespace SimpleSprint.Middleware {
    public class ApiResponseMiddleware {
        private readonly RequestDelegate _next;

        public ApiResponseMiddleware (RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke (HttpContext context) {
            if (IsSwagger (context))
                await this._next (context);
            else {
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream ()) {
                    context.Response.Body = responseBody;

                    try {
                        await _next.Invoke (context);

                        if (context.Response.StatusCode == (int) HttpStatusCode.OK) {
                            var body = await FormatResponse (context.Response);
                            await HandleSuccessRequestAsync (context, body, context.Response.StatusCode);

                        } else {
                            await HandleNotSuccessRequestAsync (context, context.Response.StatusCode);
                        }
                    } catch (System.Exception ex) {
                        await HandleExceptionAsync (context, ex);
                    } finally {
                        responseBody.Seek (0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync (originalBodyStream);
                    }
                }
            }

        }

        private static Task HandleExceptionAsync (HttpContext context, System.Exception exception) {
            ApiError apiError = null;
            ApiResponse ApiResponse = null;
            int code = 0;

            if (exception is ApiException) {
                var ex = exception as ApiException;
                apiError = new ApiError (ex.Message);
                apiError.ValidationErrors = ex.Errors;
                apiError.ReferenceErrorCode = ex.ReferenceErrorCode;
                apiError.ReferenceDocumentLink = ex.ReferenceDocumentLink;
                code = ex.StatusCode;
                context.Response.StatusCode = code;

            } else if (exception is UnauthorizedAccessException) {
                apiError = new ApiError ("Unauthorized Access");
                code = (int) HttpStatusCode.Unauthorized;
                context.Response.StatusCode = code;
            } else {
#if !DEBUG
                var msg = "An unhandled error occurred.";
                string stack = null;
#else
                var msg = exception.GetBaseException ().Message;
                string stack = exception.StackTrace;
#endif

                apiError = new ApiError (msg);
                apiError.Details = stack;
                code = (int) HttpStatusCode.InternalServerError;
                context.Response.StatusCode = code;
            }

            context.Response.ContentType = "application/json";

            ApiResponse = new ApiResponse (code, ResponseMessageEnum.Exception.DescriptionAttr<Enum> (), null, apiError);

            var json = JsonConvert.SerializeObject (ApiResponse);

            return context.Response.WriteAsync (json);
        }

        private static Task HandleNotSuccessRequestAsync (HttpContext context, int code) {
            context.Response.ContentType = "application/json";

            ApiError apiError = null;
            ApiResponse ApiResponse = null;

            if (code == (int) HttpStatusCode.NotFound)
                apiError = new ApiError ("The specified URI does not exist. Please verify and try again.");
            else if (code == (int) HttpStatusCode.NoContent)
                apiError = new ApiError ("The specified URI does not contain any content.");
            else if (code == (int) HttpStatusCode.Unauthorized)
                apiError = new ApiError ("Unauthorized");
            else
                apiError = new ApiError ("Your request cannot be processed. Please contact a support.");

            ApiResponse = new ApiResponse (code, ResponseMessageEnum.Failure.DescriptionAttr<Enum> (), null, apiError);
            context.Response.StatusCode = code;

            var json = JsonConvert.SerializeObject (ApiResponse);

            return context.Response.WriteAsync (json);
        }

        private static Task HandleSuccessRequestAsync (HttpContext context, object body, int code) {
            context.Response.ContentType = "application/json";
            string jsonString, bodyText = string.Empty;
            ApiResponse ApiResponse = null;
            string value = body.ToString ();
            if (!value.IsValidJson ())
                bodyText = JsonConvert.SerializeObject (body);
            else
                bodyText = body.ToString ();

            dynamic bodyContent = JsonConvert.DeserializeObject<dynamic> (bodyText);
            Type type;

            type = bodyContent?.GetType ();

            if (type.Equals (typeof (Newtonsoft.Json.Linq.JObject))) {
                ApiResponse = JsonConvert.DeserializeObject<ApiResponse> (bodyText);
                if (ApiResponse.StatusCode != code)
                    jsonString = JsonConvert.SerializeObject (ApiResponse);
                else if (ApiResponse.Result != null)
                    jsonString = JsonConvert.SerializeObject (ApiResponse);
                else {
                    ApiResponse = new ApiResponse (code, ResponseMessageEnum.Success.DescriptionAttr<Enum> (), bodyContent, null);
                    jsonString = JsonConvert.SerializeObject (ApiResponse);
                }
            } else {
                ApiResponse = new ApiResponse (code, ResponseMessageEnum.Success.DescriptionAttr<Enum> (), bodyContent, null);
                jsonString = JsonConvert.SerializeObject (ApiResponse);
            }

            return context.Response.WriteAsync (jsonString);
        }

        private async Task<string> FormatResponse (HttpResponse response) {
            response.Body.Seek (0, SeekOrigin.Begin);
            var plainBodyText = await new StreamReader (response.Body).ReadToEndAsync ();
            response.Body.Seek (0, SeekOrigin.Begin);

            return plainBodyText;
        }

        private bool IsSwagger (HttpContext context) {
            return context.Request.Path.StartsWithSegments ("/swagger");
        }
    }
}
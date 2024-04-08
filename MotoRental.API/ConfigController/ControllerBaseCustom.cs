using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MotoRental.Core.ResponseDefault;
using System.Net;

namespace MotoRental.API.ConfigController
{
    public class ControllerBaseCustom : ControllerBase
    {
        public ActionResult<T> StatusCode<T>(T returnAPI) where T : ReturnAPI
        {
            return new ObjectResult(returnAPI) { StatusCode = (int)returnAPI.StatusCode };
        }

        public IActionResult IStatusCode<T>(T returnAPI) where T : ReturnAPI
        {
            return new ObjectResult(returnAPI) { StatusCode = (int)returnAPI.StatusCode };
        }
    }
}

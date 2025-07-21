using EventPlanningAndManagementSystem.Controllers;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : BaseController
{
    [Route("Error/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        return statusCode switch
        {
            404 => View("Error404"),
            500 => View("Error500"),
            403 => View("Error403"),
            _ => View("Error")
        };
    }
}

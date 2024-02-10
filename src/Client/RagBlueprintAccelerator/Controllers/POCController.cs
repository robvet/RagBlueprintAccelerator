using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Net.Http.Json;

namespace RagBlueprintAccelerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class POCController : ControllerBase
    {
        // GET: POCController/Index

        [HttpGet]
        public IActionResult Get()
        {
            var json = System.Text.Json.JsonSerializer.Serialize("Get");
            return Content(json, "application/json");
        }

        //POST: POCController/Create
        [HttpPost]
        public ActionResult Create(int id, [FromBody] Customer customer)
        {

            //var request = System.Text.Json.JsonSerializer.Deserialize(customer);

            var json = System.Text.Json.JsonSerializer.Serialize("Post");
            return Content(json, "application/json");
        }

        //// GET: POCController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: POCController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: POCController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: POCController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}

using Azure;
using Azure.AI.OpenAI;
using RagBlueprintAccelerator.Client.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Contracts;


namespace RagBlueprintAccelerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ChatController : ControllerBase
    {

        private readonly IChatCompletion _chatCompletion;
        public ChatController(IChatCompletion chatCompletion)
        {
            _chatCompletion = chatCompletion;
        }


        // GET: POCController/Index

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    var json = System.Text.Json.JsonSerializer.Serialize("Get");
        //    return Content(json, "application/json");
        //}

        //POST: POCController/Create
        [HttpPost]
        public async Task<ActionResult> PostCompletion([FromBody] CompletionOverrides completionOptions)
        {

            //(ChatCompletions response, ChatCompletions followup, int promptTokens, int responseTokens, int suggestionTokens) = await _chatCompletion.ChatCompletionAsync(completionOptions);
            var completion = await _chatCompletion.ChatCompletionAsync(completionOptions);

            return Ok(completion);



            //return null;

            //return Ok(new Completion
            //{
            //    Response = response.Choices.FirstOrDefault()?.Message?.Content,
            //    Suggestions = followup.Choices.FirstOrDefault()?.Message?.Content,
            //    PromptTokens = promptTokens,
            //    ResponseTokens = responseTokens,
            //    SuggestionTokens = suggestionTokens
            //});


            //var request = System.Text.Json.JsonSerializer.Deserialize(customer);
            //await ChatCompletion.ChatCompletionAsync(ezCompletionOptions);

            //var json = System.Text.Json.JsonSerializer.Serialize("Post");
            //return Content(json, "application/json");

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

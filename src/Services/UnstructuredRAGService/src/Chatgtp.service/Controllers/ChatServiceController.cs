using Microsoft.AspNetCore.Mvc;
using Shared.Utilties;
using UnstructuredRAG.Service.Contracts;
using UnstructuredRAG.Service.Models;
using UnstructuredRAG.Service.Options;

namespace UnstructuredRAG.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatServiceController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatServiceController> _logger;
        private const string dummyCorrelationToken = "76657114-cbb3-45cc-9255-b7fd3c4b14a9";

        public ChatServiceController(IChatService chatService,
                                     ILogger<ChatServiceController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Returns summary (list) of active chat sessions with option of returning associated chat messages
        /// </summary>
        /// <param name="correlationToken">Unique tracking value for the request</param>
        /// <param name="returnMessages">Boolean flag to return session messages - default is false</param>
        /// <returns>Collection of Chat Sessions</returns>
        [ProducesResponseType(typeof(List<Session>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [HttpGet("ChatSessions", Name = "GetAllChatSessionsAsync")]
        public async Task<IActionResult> GetAllChatSessionsAsync([FromHeader(Name = "x-correlationToken")] string correlationToken = dummyCorrelationToken,
                                                                 bool returnMessages = false)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);

            var sessions = await _chatService.GetAllChatSessionsAsync(correlationToken, returnMessages);

            //if (sessions == null || sessions.Count < 1)
            //{
            //    return StatusCode(204, "No chat session were found");
            //}

            return new ObjectResult(Mapper.MapToSessionsToDto(sessions));
        }

        /// <summary>
        /// Create new (empty) chat session.
        /// </summary>
        /// <param name="correlationToken">Unique tracking value for the request</param>
        /// <returns>Chat session</returns>
        [ProducesResponseType(typeof(List<Message>), 204)]
        [ProducesResponseType(400)]
        [HttpPost("CreateSession", Name = "CreateNewChatSessionAsync")]
        public async Task<IActionResult> CreateNewChatSessionAsync([FromHeader(Name = "x-correlationToken")] string correlationToken = dummyCorrelationToken)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);

            var session = await _chatService.CreateNewChatSessionAsync(correlationToken);

            if (session == null)
            {
                return StatusCode(500, "A Chat Session was not created!");
            }
            else
            {
                return new ObjectResult(session);
            }
        }

        /// <summary>
        /// Returns specific chat session with option to return associated chat messages
        /// </summary>
        /// <param name="sessionId">Session Id for active chat session</param>
        /// <param name="correlationToken">Unique tracking value for the request</param>
        /// <param name="returnMessages">Boolean flag to return session messages - default is false</param>
        /// <returns>Chat Session</returns>
        [ProducesResponseType(typeof(Session), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpGet("ChatSessionById", Name = "GetChatSessionByIdAsync")]
        public async Task<IActionResult> GetChatSessionByIdAsync(string sessionId,
                                         [FromHeader(Name = "x-correlationToken")] string correlationToken = dummyCorrelationToken,
                                         bool returnMessages = false)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);
            Guard.ForInvalidGuid(sessionId);

            var session = await _chatService.GetChatSessionByIdAsync(sessionId, returnMessages, correlationToken);

            if (session == null)
            {
                return NotFound($"Session '{sessionId}' not found!");
            }

            return new ObjectResult(Mapper.MapToSessionToDto(session));
        }

        /// <summary>
        /// Adds conversation (a prompt and response) to an active chat session
        /// </summary>
        /// <param name="sessionId">Session Id for active chat session</param>
        /// <param name="prompt">Question to ask the model</param>
        /// <param name="completionOptions">Sensitivity settings for the prompt</param>
        /// <param name="correlationToken">Unique tracking value for the request</param>
        /// <returns>A Completion model class that contains response</returns>
        [ProducesResponseType(typeof(Completion), 200)]
        [ProducesResponseType(204)]

        [HttpPost("PostCompletion", Name = "PostChatCompletionAsync")]
        public async Task<IActionResult> PostChatCompletionAsync(string sessionId,
                                                                 string prompt,
                                                                 [FromBody] CompletionOptions completionOptions,
                                                                 [FromHeader(Name = "x-correlationToken")] string? correlationToken = dummyCorrelationToken)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);
            Guard.ForNullObject(completionOptions, "PromptSensitivity");
            Guard.ForNullOrEmpty(prompt, "Prompt");
            Guard.ForInvalidGuid(sessionId);

            var completion = await _chatService.PostChatCompletionAsync(sessionId, prompt, completionOptions, correlationToken);

            if (completion == null)
            {
                return NotFound($"Session '{sessionId}' not found!");
            }

            return Ok(completion);
        }

        /// <summary>
        /// Returns chat messages for a given chat session
        /// </summary>
        /// <param name="sessionId">Session Id for active chat session</param>
        /// <param name="correlationToken">Unique tracking value for the request</param>
        /// <returns>Collection of Chat Messages</returns>
        [ProducesResponseType(typeof(List<Message>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpGet("ChatSessionMessages", Name = "GetChatSessionMessagesAsync")]
        public async Task<IActionResult> GetChatSessionMessagesAsync(string sessionId,
                                         [FromHeader(Name = "x-correlationToken")] string correlationToken = dummyCorrelationToken)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);
            Guard.ForInvalidGuid(sessionId);

            var messages = await _chatService.GetChatSessionMessagesAsync(sessionId, correlationToken);

            if (messages == null)
            {
                return NotFound($"Session '{sessionId}' not found!");
            }

            //if (messages.Count == 0)
            //{
            //    return StatusCode(StatusCodes.Status204NoContent);
            //}

            return new ObjectResult(Mapper.MapToMessagesToDto(messages));
        }

        /// <summary>
        /// Updates the attributes of a chat session
        /// </summary>6
        /// <param name="sessionId">Session Id for active chat session</param>
        /// <param name="correlationToken">Unique tracking value for the request</param
        /// <returns>Http 204 Status Code</returns>
        [ProducesResponseType(typeof(List<Message>), 204)]
        [ProducesResponseType(404)]
        [HttpPut("UpdateSession", Name = "UpdateChatSessionAsync")]
        public async Task<IActionResult> UpdateChatSessionAsync([FromBody] Session session,
                                                                [FromHeader(Name = "x-correlationToken")] string correlationToken = dummyCorrelationToken)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);
            Guard.ForNullObject(session, "Session");

            var updatedSession = await _chatService.UpdateChatSessionAsync(session, correlationToken);

            if (updatedSession == null)
            {
                return NotFound($"Session '{session.SessionId}' not found!");
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }

        /// <summary>
        /// Updates session name by summmarizing prompt
        /// </summary>
        /// <param name="sessionId">Session Id for active chat session</param>
        /// <param name="prompt">Question to ask the model</param>
        /// <param name="correlationToken">Unique tracking value for the request</param>
        /// <returns>A Summary model class with the summarized response</returns>
        [ProducesResponseType(typeof(Summary), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut("RenameChatSession", Name = "RenameChatSessionAsync")]
        public async Task<IActionResult> RenameChatSessionAsync(string sessionId,
                                                                       string prompt,
                                                                       [FromHeader(Name = "x-correlationToken")] string correlationToken = dummyCorrelationToken)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);
            Guard.ForInvalidGuid(sessionId);
            Guard.ForNullOrEmpty(prompt, "Prompt");

            var summary = await _chatService.RenameChatSessionAsync(sessionId, prompt, correlationToken);

            if (summary == null)
            {
                return NotFound($"Session '{sessionId}' not found!");
            }

            return Ok(summary);
        }

        /// <summary>
        /// Deletes chat session
        /// </summary>
        /// <param name="sessionId">Session Id for active chat session</param>
        /// <param name="correlationToken">Unique tracking value for the request</param>
        /// <returns>Http 204 Status Code</returns>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("DeleteSession", Name = "DeleteChatSessionAsync")]
        public async Task<IActionResult> DeleteChatSessionAsync(string sessionId,
                                                               [FromHeader(Name = "x-correlationToken")] string correlationToken = dummyCorrelationToken)
        {
            Guard.CorrelationTokenValidation(ref correlationToken);
            Guard.ForInvalidGuid(sessionId);

            var response = await _chatService.DeleteChatSessionAsync(sessionId, correlationToken);

            if (!response)
            {
                return NotFound($"Session '{sessionId}' not found!");
            }

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}



///// <summary>
///// Rename the Chat Ssssion from "New Chat" to the summary provided
///// </summary>
///// <param name="sessionId">Session Id for active chat session</param>
///// <param name="newChatSessionName"></param>
///// <param name="correlationToken">Unique tracking value for the request</param>
///// <returns>Http 204 Status Code</returns>
//[ProducesResponseType(typeof(List<Message>), 204)]
//[ProducesResponseType(400)]
//[HttpPut("RenameSession", Name = "RenameChatSessionAsync")]
//public async Task<IActionResult> RenameChatSessionAsync(string sessionId,
//                                                        string newChatSessionName, 
//                                                        [FromHeader(Name = "x-correlationToken")] string correlationToken = "123")
//{
//    Guard.ForNullOrEmpty(correlationToken, "CorrelationToken");
//    Guard.ForNullOrEmpty(sessionId, "SessionId");
//    Guard.ForNullOrEmpty(newChatSessionName, "NewChatSessionName");

//    var session = await _chatService.RenameChatSessionAsync(sessionId, newChatSessionName, correlationToken);


//    if (session == null)
//    {
//        return NotFound($"Chat session {sessionId} not found!");
//    }
//    else
//    {
//        return new ObjectResult(session);
//    }
//}


//        /// <summary>
///// Adds conversation (a prompt and response) to an active chat session
///// </summary>
///// <param name="sessionId">Session Id for active chat session</param>
///// <param name="prompt">Question to ask the model</param>
///// <param name="correlationToken">Unique tracking value for the request</param>
///// <returns>A Completion model class that contains response</returns>
//[ProducesResponseType(typeof(string), 200)]
//[ProducesResponseType(204)]
//[ProducesResponseType(400)]
//[HttpGet("ChatCompletion", Name = "GetChatCompletionAsync")]
//public async Task<IActionResult> GetChatCompletionAsync(string sessionId,
//                                                        string prompt,
//                                                        [FromHeader(Name = "x-correlationToken")] string? correlationToken = "123")
//{
//    Guard.ForNullOrEmpty(correlationToken, "CorrelationToken");
//    //Guard.CorrelationTokenValidation(ref correlationToken);
//    Guard.ForNullOrEmpty(sessionId, "SessionId");
//    Guard.ForNullOrEmpty(prompt, "Prompt");

//    var completion = await _chatService.GetChatCompletionAsync(sessionId, prompt, correlationToken);

//    if (completion == null)
//    {
//        return StatusCode(StatusCodes.Status204NoContent);
//    }

//    return Ok(completion);
//}

using CRM.Chat.Api.Controllers.Base;
using CRM.Chat.Application.Common.Abstractions.Mediators;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Chat.Api.Controllers;

public class MessagesController : BaseController
{
    public MessagesController(IMediator mediator) : base(mediator) { }


}
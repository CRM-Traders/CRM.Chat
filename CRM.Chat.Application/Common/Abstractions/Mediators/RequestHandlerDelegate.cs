using CRM.Chat.Domain.Common.Models;

namespace CRM.Chat.Application.Common.Abstractions.Mediators;

public delegate ValueTask<Result<TResponse>> RequestHandlerDelegate<TResponse>();
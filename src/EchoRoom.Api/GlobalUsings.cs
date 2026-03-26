global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.OpenApi;

global using Swashbuckle.AspNetCore.Annotations;

global using System.Text.Json.Serialization;
global using System.Runtime.CompilerServices;

global using MediatR;

global using EchoRoom.Api.Extensions;
global using EchoRoom.Api.Providers;
global using EchoRoom.Api.SwaggerResponse;

global using EchoRoom.Application.Features.Authentication.RefreshToken;
global using EchoRoom.Application.Features.Authentication.Login;
global using EchoRoom.Application.Features.Users.CreateUser;
global using EchoRoom.Application.Features.Users.GetPersonalInfo;
global using EchoRoom.Application.Features.Users.GetUsersManaging;

global using EchoRoom.Application.Extensions;

global using EchoRoom.Shared.Constants.Core;
global using EchoRoom.Shared.Constants.Constants;
global using EchoRoom.Shared.Constants.Enums;
global using EchoRoom.Shared.Constants.Common;
global using EchoRoom.Shared.Constants.Options;

[assembly: InternalsVisibleTo("EchoRoom.Api.Tests.Unit")]
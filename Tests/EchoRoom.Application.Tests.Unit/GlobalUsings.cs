global using Microsoft.Extensions.Logging;

global using System.Linq.Expressions;

global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using Shouldly;

global using Isopoh.Cryptography.Argon2;

global using EchoRoom.Application.Features.Users.CreateUser;
global using EchoRoom.Application.Features.Authentication.Login;
global using EchoRoom.Application.Features.Users.GetPersonalInfo;

global using EchoRoom.Shared.Services.Services.Abstractions;

global using EchoRoom.Shared.Constants.Constants;
global using EchoRoom.Shared.Constants.Core;
global using EchoRoom.Shared.Constants.Enums;

global using EchoRoom.Database.Infrastructure.UoW;

global using EchoRoom.Database.Context.Models;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.Extensions.Configuration;
global using Microsoft.IdentityModel.Tokens;

global using System.ComponentModel.DataAnnotations;
global using System.Text;
global using System.Reflection;
global using System.Linq.Expressions;

global using MediatR;

global using Isopoh.Cryptography.Argon2;

global using EchoRoom.Shared.Services.Services.Abstractions;
global using EchoRoom.Shared.Services.Extensions;

global using EchoRoom.Shared.Constants.Core;
global using EchoRoom.Shared.Constants.Common;
global using EchoRoom.Shared.Constants.Constants;
global using EchoRoom.Shared.Constants.Enums;
global using EchoRoom.Shared.Constants.Options;

global using EchoRoom.Database.Infrastructure.Extensions;
global using EchoRoom.Database.Infrastructure.UoW;

global using EchoRoom.Database.Context.Models;
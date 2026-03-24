global using Microsoft.Extensions.Caching.Distributed;
global using System.Text.Json;

global using EchoRoom.Shared.Constants.Common;
global using EchoRoom.Shared.Constants.Constants;
global using EchoRoom.Shared.Constants.Options;

global using EchoRoom.Shared.Services.Services.Abstractions;
global using EchoRoom.Shared.Services.Services.Implementations;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EchoRoom.Shared.Services.Tests.Unit")]
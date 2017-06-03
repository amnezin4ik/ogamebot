﻿using System.Collections.Generic;
using System.Threading.Tasks;
using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.APIClients
{
    public interface IUserPlanetsClient
    {
        Task<IEnumerable<UserPlanet>> GetUserPlanetsAsync(SessionData sessionData);
        Task SetActivePlanetAsync(SessionData sessionData, string planetId);
    }
}
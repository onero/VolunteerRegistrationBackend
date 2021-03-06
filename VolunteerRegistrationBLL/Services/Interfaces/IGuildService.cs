﻿using System;
using System.Collections.Generic;
using System.Text;
using VolunteerRegistrationBLL.BusinessObjects;
using VolunteerRegistrationDAL.Entities;

namespace VolunteerRegistrationBLL.Services.Interfaces
{
    public interface IGuildService : IService<GuildBO>
    {
        bool AddGuildWork(GuildWorkBO guildWork);
        IEnumerable<GuildWorkBO> GetGuidWorksFromGuild(int idOfGuild);
    }
}

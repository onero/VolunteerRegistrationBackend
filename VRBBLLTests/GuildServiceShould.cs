﻿using System.Collections.Generic;
using Moq;
using VolunteerRegistrationBLL.BusinessObjects;
using VolunteerRegistrationBLL.Services;
using VolunteerRegistrationBLL.Services.Interfaces;
using VolunteerRegistrationDAL.Entities;
using VolunteerRegistrationDAL.Repositories;
using Xunit;

namespace VRBBLLTests
{
    public class GuildServiceShould : AServiceTest
    {
        private readonly Mock<IGuildRepository> _mockGuildRepo;
        private readonly IGuildService _service;

        public GuildServiceShould()
        {
            _mockGuildRepo = new Mock<IGuildRepository>();
            MockUOW.SetupGet(uow => uow.GuildRepository).Returns(_mockGuildRepo.Object);
            _service = new GuildService(MockDALFacade.Object);
        }

        [Fact]
        public override void CreateOne()
        {
            _mockGuildRepo.Setup(r => r.Create(It.IsAny<Guild>())).Returns(new Guild());
            var entity = _service.Create(new GuildBO());
            Assert.NotNull(entity);
        }

        [Fact]
        public override void DeleteByExistingId()
        {
            var entity = new GuildBO {Id = 1};
            _mockGuildRepo.Setup(r => r.Delete(entity.Id)).Returns(new Guild {Id = entity.Id});
            var deleted = _service.Delete(entity.Id);
            Assert.True(deleted);
        }

        [Fact]
        public override void GetAll()
        {
            _mockGuildRepo.Setup(r => r.GetAll()).Returns(new List<Guild> {new Guild()});
            var entities = _service.GetAll();
            Assert.NotEmpty(entities);
        }

        [Fact]
        public override void GetAllByExistingIds()
        {
            var guild = new GuildBO {Id = 1};
            _mockGuildRepo.Setup(r => r.GetAll(It.IsAny<List<int>>())).Returns(new List<Guild> {new Guild {Id = guild.Id}});
            var entities = _service.GetAll(new List<int> {guild.Id});
            Assert.NotEmpty(entities);
        }

        [Fact]
        public override void GetOneByExistingId()
        {
            var guild = new GuildBO {Id = 1};
            _mockGuildRepo.Setup(r => r.Get(guild.Id)).Returns(new Guild {Id = guild.Id});
            var entity = _service.Get(guild.Id);
            Assert.NotNull(entity);
        }

        [Fact]
        public void GetOneByExistingIdWithVolunteers()
        {
            var volunteer = new VolunteerBO {Id = 1};
            _mockGuildRepo.Setup(r => r.Get(volunteer.Id)).Returns(new Guild
            {
                Id = 1,
                GuildWork = new List<GuildWork>
                {
                    new GuildWork
                    {
                        GuildId = 1,
                        VolunteerId = 1
                    }
                }
            });
            var mockVolunteerRepo = new Mock<IVolunteerRepository>();
            MockUOW.SetupGet(uow => uow.VolunteerRepository).Returns(mockVolunteerRepo.Object);
            mockVolunteerRepo.Setup(r => r.GetVolunteersWithIds(It.IsAny<List<int>>())).Returns(new List<Volunteer>
            {
                new Volunteer {Id = 1}
            });


            var entity = _service.Get(volunteer.Id);

            Assert.NotNull(entity);
            Assert.NotEmpty(entity.Volunteers);
        }

        [Fact]
        public void GetOneByIdWithGuildWork()
        {
            var guildFromDB = new Guild
            {
                Id = 1,
                Name = "D4FF",
                GuildWork = new List<GuildWork>
                {
                    new GuildWork {GuildId = 1, VolunteerId = 1}
                }
            };

            var volunteers = new List<Volunteer> {new Volunteer {Id = 1}};

            _mockGuildRepo.Setup(r => r.Get(It.IsAny<int>())).Returns(guildFromDB);
            var volunteerRepo = new Mock<IVolunteerRepository>();
            MockUOW.SetupGet(uow => uow.VolunteerRepository).Returns(volunteerRepo.Object);
            volunteerRepo.Setup(r => r.GetVolunteersWithIds(It.IsAny<List<int>>())).Returns(volunteers);

            var result = _service.Get(1);
            Assert.NotEmpty(result.GuildWork);
        }

        [Fact]
        public override void NotConvertNullEntity()
        {
            var entity = _service.Create(null);
            Assert.Null(entity);
        }

        [Fact]
        public override void NotDeleteByNonExistingId()
        {
            _mockGuildRepo.Setup(r => r.Delete(It.IsAny<int>())).Returns(() => null);
            var nonExistingId = 0;
            var deleted = _service.Delete(nonExistingId);
            Assert.False(deleted);
        }

        [Fact]
        public override void NotGetAllByNonExistingIds()
        {
            _mockGuildRepo.Setup(r => r.GetAll(It.IsAny<List<int>>())).Returns(new List<Guild>());
            var entities = _service.GetAll(new List<int>());
            Assert.Empty(entities);
        }

        [Fact]
        public override void NotGetOneByNonExistingId()
        {
            _mockGuildRepo.Setup(r => r.Get(It.IsAny<int>())).Returns(() => null);
            var nonExistingId = 0;
            var entity = _service.Get(nonExistingId);
            Assert.Null(entity);
        }

        [Fact]
        public override void NotUpdateByNonExistingId()
        {
            _mockGuildRepo.Setup(r => r.Get(It.IsAny<int>())).Returns(() => null);
            var nonExistingGuild = new GuildBO {Id = 0};
            var entity = _service.Update(nonExistingGuild);
            Assert.Null(entity);
        }

        [Fact]
        public override void UpdateByExistingId()
        {
            var guild = new Guild {Id = 1, Name = "One"};
            _mockGuildRepo.Setup(r => r.Get(guild.Id)).Returns(new Guild {Id = guild.Id, Name = guild.Name});
            var guildToUpdate = _service.Get(guild.Id);
            var newName = "D4FF";
            guildToUpdate.Name = newName;
            var updatedGuild = _service.Update(guildToUpdate);
            Assert.Contains(updatedGuild.Name, newName);
        }

        [Fact]
        public void GetGuildWorksFromGuildWithId()
        {
            const int id = 1;

            _mockGuildRepo.Setup(r => r.Get(It.IsAny<int>()))
                .Returns(new Guild
                {
                    GuildWork = new List<GuildWork>
                {
                    new GuildWork(), new GuildWork()
                }
                });
            var result = _service.GetGuidWorksFromGuild(id);
            Assert.NotNull(result);
        }

        [Fact]
        public void NotGetGuildWorksFromGuildWithNoId()
        {
            var id = 0;
            _mockGuildRepo.Setup(r => r.Get(id)).Returns(() => null);
            var result = _service.GetGuidWorksFromGuild(id);
            Assert.Null(result);
        }

        [Fact]
        public void NotGetGuildWorksFromGuildWithNoGuildWorks()
        {
            var id = 1;
            _mockGuildRepo.Setup(r => r.Get(It.IsAny<int>())).Returns(new Guild());
            var result = _service.GetGuidWorksFromGuild(id);
            Assert.Null(result);
        }
    }
}
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoMapper;
using MagicEvents.Api.Service.Domain.Repositories;
using System;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Application.Exceptions;
using FluentAssertions;
using MagicEvents.Api.Service.Domain.Enums;
using System.Collections.Generic;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Domain.ValueObjects;
using System.Linq;

namespace MagicEvents.Api.Service.UnitTests.Application.Services
{
    public class UserActivityServiceTests
    {
        private readonly Mock<IMapper> _mapperStub = new();
        private readonly Mock<IUserRepository> _userRepositoryStub = new();
        private readonly Mock<IEventRepository> _eventRepositoryStub = new();
        
        [Fact]
        public void GetActivitiesAsync_WithInvalidUserId_ThrowsServiceException()
        {
            // Arrange
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User) null);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            var invalidId = Guid.NewGuid();
            // Act
            Func<Task> getActitiesFunc = async () => await activityService.GetActivitiesAsync(invalidId);
            // Assert
            getActitiesFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public async Task GetActivitiesAsync_WithValidUserId_ReturnsListOfActivities()
        {
            // Arrange
            var existingUser = new User(Guid.NewGuid(),null, null);
            existingUser.AddToActivities(
                Guid.NewGuid(), 
                UserEventRole.Organizer,
                EventActivityStatus.Active);
            existingUser.AddToActivities(
                Guid.NewGuid(), 
                UserEventRole.StandardParticipant,
                EventActivityStatus.Left);

            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _mapperStub
                .Setup(x => x.Map<IEnumerable<UserEventActivityDto>>(It.IsAny<List<UserEventActivity>>()))
                .Returns(existingUser.EventActivities.Select(x => 
                    {
                        return new UserEventActivityDto
                        {
                            EventId = x.EventId,
                            Role = x.Role,
                            Status = x.Status
                        };
                    })
                    .AsEnumerable());
            
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            // Act
            var result = await activityService.GetActivitiesAsync(existingUser.Id);
            // Assert
            result.Should()
                .BeEquivalentTo(existingUser.EventActivities);
        }

        [Fact]
        public void RegisterOnEventAsync_WithInvalidUserId_ThrowsServiceException()
        {
            // Arrange
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User) null);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            var userId = Guid.NewGuid(); 
            var eventId = Guid.NewGuid(); 
            // Act
            Func<Task> registerOnEventFunc = async () => 
                await activityService.RegisterOnEventAsync(userId, eventId);
            // Assert
            registerOnEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void RegisterOnEventAsync_WithInvalidEventId_ThrowsServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null); 
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Event) null);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            var eventId = Guid.NewGuid(); 
            // Act
            Func<Task> registerOnEventFunc = async () => 
                await activityService.RegisterOnEventAsync(userId, eventId);
            // Assert
            registerOnEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }

        [Fact]
        public void RegisterOnEventAsync_WhenEventIsNotOpenForRegistration_ThrowsServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            var existingEvent = CreateRandomEvent();
            existingEvent.Status = EventStatus.Canceled;
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            // Act
            Func<Task> registerOnEventFunc = async () => 
                await activityService.RegisterOnEventAsync(userId, existingEvent.Id);
            // Assert
            registerOnEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.CantRegisterForEvent);
        }

        [Fact]
        public void RegisterOnEventAsync_WhenUserIsAlreadyRegistered_ThrowsServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            var existingEvent = CreateRandomEvent();
            existingEvent.Participants.AddParticipant(userId, UserEventRole.StandardParticipant);
            existingUser.AddToActivities(existingEvent.Id, UserEventRole.StandardParticipant, 
                EventActivityStatus.Active);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            // Act
            Func<Task> registerOnEventFunc = async () => 
                await activityService.RegisterOnEventAsync(userId, existingEvent.Id);
            // Assert
            registerOnEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.UserAlreadyRegisteredForEvent);
        }

        [Fact]
        public void LeaveEventAsync_WithInvalidUserId_ThrowsServiceException()
        {
            // Arrange
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User) null);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            var userId = Guid.NewGuid(); 
            var eventId = Guid.NewGuid(); 
            // Act
            Func<Task> leaveEventFunc = async () => 
                await activityService.LeaveEventAsync(userId, eventId);
            // Assert
            leaveEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void LeaveEventAsync_WithInvalidEventId_ThrowsServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null); 
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Event) null);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            var eventId = Guid.NewGuid(); 
            // Act
            Func<Task> leaveEventFunc = async () => 
                await activityService.LeaveEventAsync(userId, eventId);
            // Assert
            leaveEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }

        [Fact]
        public void LeaveEventAsync_WhenUserIsOrganizer_ThrowsServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            var existingEvent = Event.CreateEvent(
                Guid.NewGuid(),
                userId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1));
            existingUser.AddToActivities(existingEvent.Id, UserEventRole.Organizer, 
                EventActivityStatus.Active);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            // Act
            Func<Task> registerOnEventFunc = async () => 
                await activityService.LeaveEventAsync(userId, existingEvent.Id);
            // Assert
            registerOnEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.OrgCantLeaveEvent);
        }

        [Fact]
        public void LeaveEventAsync_WhenUserIsNotRegistered_ThrowsServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            var existingEvent = CreateRandomEvent();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            // Act
            Func<Task> registerOnEventFunc = async () => 
                await activityService.LeaveEventAsync(userId, existingEvent.Id);
            // Assert
            registerOnEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.UserNotRegisteredForEvent);
        }

        [Fact]
        public void LeaveEventAsync_WhenEventHasFinished_ThrowsServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            var existingEvent = CreateRandomEvent();
            existingEvent.Participants.AddParticipant(userId, UserEventRole.StandardParticipant);
            existingUser.AddToActivities(existingEvent.Id, UserEventRole.StandardParticipant,
                EventActivityStatus.Active); 
            existingEvent.Status = EventStatus.Finished;
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var activityService = new UserActivityService(
                _mapperStub.Object, 
                _userRepositoryStub.Object,
                _eventRepositoryStub.Object);
            // Act
            Func<Task> registerOnEventFunc = async () => 
                await activityService.LeaveEventAsync(userId, existingEvent.Id);
            // Assert
            registerOnEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventHasFinished);
        }

        private Event CreateRandomEvent()
        {
            return Event.CreateEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1));
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Enums;
using MagicEvents.Api.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace MagicEvents.Api.Service.UnitTests.Application.Services
{
    public class EventParticipantsOrganizerServiceTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryStub = new();
        private readonly Mock<IUserRepository> _userRepositoryStub = new();

        [Fact]
        public void AddCoOrganizerAsync_WhenOrganizerDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var coOrganizerId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> addCoOrganizerFunc = () => service.AddCoOrganizerAsync(eventId, coOrganizerId, organizerId);
            // Assert
            addCoOrganizerFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void AddCoOrganizerAsync_WhenCoOrganizerDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var organizer = GenerateUser();
            var coOrganizerId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(organizer.Id))
                .ReturnsAsync(organizer);
            _userRepositoryStub.Setup(x => x.GetAsync(coOrganizerId))
                .ReturnsAsync((User)null);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> addCoOrganizerFunc = () => service.AddCoOrganizerAsync(eventId, coOrganizerId, organizer.Id);
            // Assert
            addCoOrganizerFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }
        
        [Fact]
        public void AddCoOrganizerAsync_WhenEventDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var organizer = GenerateUser();
            var coOrganizer = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(organizer.Id))
                .ReturnsAsync(organizer);
            _userRepositoryStub.Setup(x => x.GetAsync(coOrganizer.Id))
                .ReturnsAsync(coOrganizer);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> addCoOrganizerFunc = () => service.AddCoOrganizerAsync(eventId, coOrganizer.Id, organizer.Id);
            // Assert
            addCoOrganizerFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }
        
        [Fact]
        public void AddCoOrganizerAsync_WhenUserIsNotOrganizer_ShouldThrowServiceException()
        {
            // Arrange
            var @event = GenerateEvent();
            var organizer = GenerateUser();
            var coOrganizer = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(organizer.Id))
                .ReturnsAsync(organizer);
            _userRepositoryStub.Setup(x => x.GetAsync(coOrganizer.Id))
                .ReturnsAsync(coOrganizer);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> addCoOrganizerFunc = () => service.AddCoOrganizerAsync(@event.Id, coOrganizer.Id, organizer.Id);
            // Assert
            addCoOrganizerFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }
        
        [Fact]
        public void AddCoOrganizerAsync_WhenUserIsAlreadyCrewMember_ShouldThrowServiceException()
        {
            // Arrange
            var organizer = GenerateUser();
            var coOrganizer = GenerateUser();
            var @event = GenerateEvent(organizer.Id);
            @event.AddParticipant(coOrganizer.Id, UserEventRole.CoOrganizer);
            _userRepositoryStub.Setup(x => x.GetAsync(organizer.Id))
                .ReturnsAsync(organizer);
            _userRepositoryStub.Setup(x => x.GetAsync(coOrganizer.Id))
                .ReturnsAsync(coOrganizer);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> addCoOrganizerFunc = () => service.AddCoOrganizerAsync(@event.Id, coOrganizer.Id, organizer.Id);
            // Assert
            addCoOrganizerFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.UserAlreadyRegisteredForEvent);
        }

        [Fact]
        public async Task AddCoOrganizerAsync_WhenUserCanBeAdded_ShouldAddAsCoOrganizer()
        {
            // Arrange
            var organizer = GenerateUser();
            var coOrganizer = GenerateUser();
            var @event = GenerateEvent(organizer.Id);
            _userRepositoryStub.Setup(x => x.GetAsync(organizer.Id))
                .ReturnsAsync(organizer);
            _userRepositoryStub.Setup(x => x.GetAsync(coOrganizer.Id))
                .ReturnsAsync(coOrganizer);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            await service.AddCoOrganizerAsync(@event.Id, coOrganizer.Id, organizer.Id);
            // Assert
            @event.IsCoOrganizer(coOrganizer.Id)
                .Should()
                .BeTrue();
            _eventRepositoryStub.Verify(x => x.UpdateAsync(@event));
            _userRepositoryStub.Verify(x => x.UpdateAsync(coOrganizer));
        }

        [Fact]
        public void RemoveUserFromEventAsync_WhenUserDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var crewMemberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeUserFunc = () => service.RemoveUserFromEventAsync(eventId, userId, crewMemberId);
            // Assert
            removeUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void RemoveUserFromEventAsync_WhenEventDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeUserFunc = () => service.RemoveUserFromEventAsync(eventId, user.Id, crewMemberId);
            // Assert
            removeUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }

        [Fact]
        public void RemoveUserFromEventAsync_WhenUserDoesntHavePermission_ShouldThrowServiceException()
        {
            // Arrange
            var @event = GenerateEvent();
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeUserFunc = () => service.RemoveUserFromEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            removeUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }

        [Fact]
        public void RemoveUserFromEventAsync_WhenUserTryToRemoevOrganizer_ShouldThrowServiceException()
        {
            // Arrange
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            var @event = GenerateEvent(user.Id);
            @event.AddParticipant(crewMemberId, UserEventRole.CoOrganizer);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeUserFunc = () => service.RemoveUserFromEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            removeUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.OrgCantLeaveEvent);
        }

        [Fact]
        public void RemoveUserFromEventAsync_WhenUserNotRegisteredOnEvent_ShouldThrowServiceException()
        {
            // Arrange
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            var @event = GenerateEvent();
            @event.AddParticipant(crewMemberId, UserEventRole.CoOrganizer);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeUserFunc = () => service.RemoveUserFromEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            removeUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.UserNotRegisteredForEvent);
        }

        [Fact]
        public async Task RemoveUserFromEventAsync_WhenUserRegisteredOnEvent_ShouldRemoveUser()
        {
            // Arrange
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            var @event = GenerateEvent();
            @event.AddParticipant(crewMemberId, UserEventRole.CoOrganizer);
            @event.AddParticipant(user.Id, UserEventRole.StandardParticipant);
            user.AddToActivities(@event.Id, UserEventRole.StandardParticipant, EventActivityStatus.Active);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            await service.RemoveUserFromEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            @event.Participants
                .IsStandardParticipant(user.Id)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void RemoveCoOrganizerAsync_WhenUserDoesntExist_ShouldThrowException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var crewMemberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeCoOrgFunc = () => service.RemoveCoOrganizerAsync(eventId, userId, crewMemberId);
            // Assert
            removeCoOrgFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void RemoveCoOrganizerAsync_WhenEventDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeCoOrgFunc = () => service.RemoveCoOrganizerAsync(eventId, user.Id, crewMemberId);
            // Assert
            removeCoOrgFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }

        [Fact]
        public void RemoveCoOrganizerAsync_WhenUserIsNotOrganizer_ShouldThrowServiceException()
        {
            // Arrange
            var user = GenerateUser();
            var organizerId = Guid.NewGuid();
            var @event = GenerateEvent();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeCoOrgFunc = () => service.RemoveCoOrganizerAsync(@event.Id, user.Id, organizerId);
            // Assert
            removeCoOrgFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }

        [Fact]
        public void RemoveCoOrganizerAsync_WhenUserIsNotRegisteredOnEvent_ShouldThrowServiceException()
        {
            // Arrange
            var user = GenerateUser();
            var organizerId = Guid.NewGuid();
            var @event = GenerateEvent(organizerId);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeCoOrgFunc = () => service.RemoveCoOrganizerAsync(@event.Id, user.Id, organizerId);
            // Assert
            removeCoOrgFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.UserNotRegisteredForEvent);
        }

        [Fact]
        public void RemoveCoOrganizerAsync_WhenUserIsNotCoOrganizer_ShouldThrowServiceException()
        {
            // Arrange
            var user = GenerateUser();
            var organizerId = Guid.NewGuid();
            var @event = GenerateEvent(organizerId);
            @event.AddParticipant(user.Id, UserEventRole.StandardParticipant);
            @user.AddToActivities(@event.Id, UserEventRole.StandardParticipant, EventActivityStatus.Active);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> removeCoOrgFunc = () => service.RemoveCoOrganizerAsync(@event.Id, user.Id, organizerId);
            // Assert
            removeCoOrgFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.InvalidEventRole);
        }

        [Fact]
        public async Task RemoveCoOrganizerAsync_WhenUserIsCoOrganizer_ShouldRemoveUser()
        {
            // Arrange
            var user = GenerateUser();
            var organizerId = Guid.NewGuid();
            var @event = GenerateEvent(organizerId);
            @event.AddParticipant(user.Id, UserEventRole.CoOrganizer);
            @user.AddToActivities(@event.Id, UserEventRole.CoOrganizer, EventActivityStatus.Active);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            await service.RemoveCoOrganizerAsync(@event.Id, user.Id, organizerId);
            // Assert
            @event.Participants.IsStandardParticipant(user.Id)
                .Should()
                .BeTrue();
            _userRepositoryStub.Verify(x => x.UpdateAsync(user));
            _eventRepositoryStub.Verify(x => x.UpdateAsync(@event));
        }

        [Fact]
        public void BanUserOnEventAsync_WhenUserDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var crewMemberId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> banUserFunc = () => service.BanUserOnEventAsync(eventId, userId, crewMemberId);
            // Assert
            banUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void BanUserOnEventAsync_WhenEventDoesntExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> banUserFunc = () => service.BanUserOnEventAsync(eventId, user.Id, crewMemberId);
            // Assert
            banUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }

        [Fact]
        public void BanUserOnEventAsync_WhenUserDoesntHavePermission_ShouldThrowServiceException()
        {
            // Arrange
            var @event = GenerateEvent();
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> banUserFunc = () => service.BanUserOnEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            banUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }

        [Fact]
        public void BanUserOnEventAsync_WhenUserTriesToBanOrganizer_ShouldThrowServiceException()
        {
            // Arrange
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            var @event = GenerateEvent();
            @event.AddParticipant(crewMemberId, UserEventRole.CoOrganizer);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> banUserFunc = () => service.BanUserOnEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            banUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.UserNotRegisteredForEvent);
        }

        [Fact]
        public async Task BanUserOnEventAsync_WhenUserCanBeBanned_ShouldBanUser()
        {
            // Arrange
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            var @event = GenerateEvent();
            @event.AddParticipant(crewMemberId, UserEventRole.CoOrganizer);
            @event.AddParticipant(user.Id, UserEventRole.StandardParticipant);
            user.AddToActivities(@event.Id, UserEventRole.StandardParticipant, EventActivityStatus.Active);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            await service.BanUserOnEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            user.EventActivities
                .SingleOrDefault(x => x.EventId == @event.Id)
                .Should()
                .NotBeNull();
            user.EventActivities
                .SingleOrDefault(x => x.EventId == @event.Id)
                .Status
                .Should()
                .BeEquivalentTo(EventActivityStatus.Banned);
            _userRepositoryStub.Verify(x => x.UpdateAsync(user));
            _eventRepositoryStub.Verify(x => x.UpdateAsync(@event));
        }

        [Fact]
        public void BanUserOnEventAsync_WhenUserIsNotRegistered_ShouldThrowServiceException()
        {
            // Arrange
            var crewMemberId = Guid.NewGuid();
            var user = GenerateUser();
            var @event = GenerateEvent(user.Id);
            @event.AddParticipant(crewMemberId, UserEventRole.CoOrganizer);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var service = new EventParticipantsOrganizerService(
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object
            );
            // Act
            Func<Task> banUserFunc = () => service.BanUserOnEventAsync(@event.Id, user.Id, crewMemberId);
            // Assert
            banUserFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.OrgCantLeaveEvent);
        }

        private Event GenerateEvent(Guid? organizerId = null)
        {
            return Event.CreateEvent(
                Guid.NewGuid(), 
                (organizerId is null) ? Guid.NewGuid() : (Guid) organizerId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(5)
                );
        }

        private User GenerateUser()
        {
            return new User(Guid.NewGuid());
        }
    }
}
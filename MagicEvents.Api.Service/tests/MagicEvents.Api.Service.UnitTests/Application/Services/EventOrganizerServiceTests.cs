using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.Api.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Enums;
using MagicEvents.Api.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace MagicEvents.Api.Service.UnitTests.Application.Services
{
    public class EventOrganizerServiceTests
    {
        private readonly Mock<IMapper> _mapperStub = new();
        private readonly Mock<IEventRepository> _eventRepositoryStub = new();
        private readonly Mock<IUserRepository> _userRepositoryStub = new();
        private readonly Mock<IImageProcessor> _imageProcessorStub = new();

        [Fact]
        public void CreateEventAsync_WhenUserNotExist_ShouldRaiseServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var createEventDto = new CreateEventDto
            {
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(5)
            };
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as User);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> createEventFunc = () => organizerService.CreateEventAsync(eventId, userId,createEventDto);
            // Assert
            createEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void CreateEventAsync_WhenEventTitleReserved_ShouldRaiseServiceException()
        {
            // Arrange
            var createEventDto = new CreateEventDto
            {
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(5)
            };
            var existingEvent = GenerateEvent();
            var existingUser = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> createEventFunc = () => organizerService.CreateEventAsync(
                Guid.NewGuid(), 
                existingUser.Id,
                createEventDto
                );
            // Assert
            createEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventAlreadyExist);
        }

        [Fact]
        public async Task CreateEventAsync_WhenCommandValid_ShouldCreateEvent()
        {
            // Arrange
            var createEventDto = new CreateEventDto
            {
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(5)
            };
            var existingUser = GenerateUser();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((Event) null);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            await organizerService.CreateEventAsync(Guid.NewGuid(),existingUser.Id,createEventDto);
            // Assert
            _userRepositoryStub.Verify(x => x.UpdateAsync(existingUser));
            _eventRepositoryStub.Verify(x => x.CreateAsync(It.IsAny<Event>()));
        }

        [Fact]
        public void DeleteEventAsync_WhenUserNotExist_ShouldThrowServiceException()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> deleteEventFunc = () => organizerService.DeleteEventAsync(eventId, userId);
            // Assert
            deleteEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }

        [Fact]
        public void DeleteEventAsync_WhenUserIsNotOrganizer_ShouldThrowServiceException()
        {
            // Arrange
            var existingUser = GenerateUser();
            var existingEvent = GenerateEvent();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> deleteEventFunc = () => 
                organizerService.DeleteEventAsync(existingEvent.Id, existingUser.Id);
            // Assert
            deleteEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }

        [Fact]
        public async Task DeleteEventAsync_WhenUserIsOrganizer_ShouldDeleteEvent()
        {
            // Arrange
            var existingUser = GenerateUser();
            var existingEvent = Event.CreateEvent(
                Guid.NewGuid(),
                existingUser.Id,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(5)
            );
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            await organizerService.DeleteEventAsync(existingEvent.Id, existingUser.Id);
            // Assert
            _eventRepositoryStub.Verify(x => x.DeleteAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public void CancelEventAsync_WhenEventNotFound_ShouldThrowServiceException()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> cancelEventFunc = () => organizerService.CancelEventAsync(eventId, userId);
            // Assert
            cancelEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }
        
        [Fact]
        public void CancelEventAsync_WhenUserDoesnthavePermission_ShouldThrowServiceException()
        {
            var userId = Guid.NewGuid();
            var existingEvent = GenerateEvent();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> cancelEventFunc = () => organizerService.CancelEventAsync(existingEvent.Id, userId);
            // Assert
            cancelEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }

        [Fact]
        public async Task CancelEventAsync_WhenRequestIsValid_ShouldCancelEvent()
        {
            var userId = Guid.NewGuid();
            var existingEvent = Event.CreateEvent(
                Guid.NewGuid(), 
                userId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(5)
            );
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            await organizerService.CancelEventAsync(existingEvent.Id, userId);
            // Assert
            existingEvent.Status
                .Should()
                .BeEquivalentTo(EventStatus.Canceled);
        }

        [Fact]
        public void SetThumbnailAsync_WhenEventNotFound_ShouldThrowServiceException()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var thumbnailBinaryData = Guid.NewGuid().ToByteArray();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> cancelEventFunc = () => 
                organizerService.SetThumbnailAsync(eventId, userId, thumbnailBinaryData);
            // Assert
            cancelEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }

        [Fact]
        public void SetThumbnailAsync_WhenUserDoesntHavePermission_ShouldThrowServiceException()
        {
            var existingEvent = GenerateEvent();
            var userId = Guid.NewGuid();
            var thumbnailBinaryData = Guid.NewGuid().ToByteArray();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> cancelEventFunc = () => 
                organizerService.SetThumbnailAsync(existingEvent.Id, userId, thumbnailBinaryData);
            // Assert
            cancelEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }
        
        [Fact]
        public void SetThumbnailAsync_WhenThDataIsNotValid_ShouldThrowServiceException()
        {
            var userId = Guid.NewGuid();
            var existingEvent = Event.CreateEvent(
                Guid.NewGuid(), 
                userId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(5)
            );
            var thumbnailBinaryData = Guid.NewGuid().ToByteArray();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            _imageProcessorStub.Setup(x => x.IsValidImage(It.IsAny<byte[]>()))
                .Returns(false);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> cancelEventFunc = () => 
                organizerService.SetThumbnailAsync(existingEvent.Id, userId, thumbnailBinaryData);
            // Assert
            cancelEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.File.InvalidInputFile);
        }

        [Fact]
        public async Task SetThumbnailAsync_WhenThDataIsValid_ShouldUpdateThumbnail()
        {
            var userId = Guid.NewGuid();
            var existingEvent = Event.CreateEvent(
                Guid.NewGuid(), 
                userId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(5)
            );
            var imageBinaryData = Guid.NewGuid().ToByteArray();
            var thumbnailBinaryData = Guid.NewGuid().ToByteArray();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            _imageProcessorStub.Setup(x => x.IsValidImage(It.IsAny<byte[]>()))
                .Returns(true);
            _imageProcessorStub.Setup(x => x.CreateThumbnailAsync(imageBinaryData))
                .ReturnsAsync(thumbnailBinaryData);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act 
            await organizerService.SetThumbnailAsync(existingEvent.Id, userId, imageBinaryData);
            // Assert
            existingEvent.Thumbnail.BinaryData
                .Should()
                .BeEquivalentTo(thumbnailBinaryData);
            _eventRepositoryStub.Verify(x => x.UpdateAsync(existingEvent));   
        }

        [Fact]
        public void UpdateEventAsync_WhenEventNotFound_ShouldThrowServiceException()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var updateEventDto = new UpdateEventDto
            {
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(5),
            };
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null as Event);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> updateEventFunc = () => 
                organizerService.UpdateEventAsync(eventId, userId, updateEventDto);
            // Assert
            updateEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Event.EventNotFound);
        }
        
        [Fact]
        public void UpdateEventAsync_WhenUserDoesntHavePermission_ShouldThrowServiceException()
        {
            var existingEvent = GenerateEvent();
            var userId = Guid.NewGuid();
            var updateEventDto = new UpdateEventDto
            {
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(5),
            };
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> updateEventFunc = () => 
                organizerService.UpdateEventAsync(existingEvent.Id, userId, updateEventDto);
            // Assert
            updateEventFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.NoPermissionForOp);
        }

        [Fact]
        public async Task UpdateEventAsync_WhenUserHasPermission_ShouldUpdateEvent()
        {
            var userId = Guid.NewGuid();
            var existingEvent = Event.CreateEvent(
                Guid.NewGuid(), 
                userId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(5)
            );
            var updateEventDto = new UpdateEventDto
            {
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(5),
            };
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingEvent);
            var organizerService = new EventOrganizerService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            await organizerService.UpdateEventAsync(existingEvent.Id, userId, updateEventDto);
            // Assert
            existingEvent.Should()
                .BeEquivalentTo(updateEventDto, options => options.ExcludingMissingMembers());
            _eventRepositoryStub.Verify(x => x.UpdateAsync(It.IsAny<Event>()));
        }

        private Event GenerateEvent()
        {
            return Event.CreateEvent(
                Guid.NewGuid(), 
                Guid.NewGuid(),
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
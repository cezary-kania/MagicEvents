using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginatedResponse;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginationQuery;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace MagicEvents.Api.Service.UnitTests.Application.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IMapper> _mapperStub = new();
        private readonly Mock<IUserRepository> _userRepositoryStub = new();
        private readonly Mock<IEventRepository> _eventRepositoryStub = new();
        
        [Fact]
        public async Task GetEventAsync_WhenEventNotFound_ShouldReturnNull()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Event) null);
            _mapperStub.Setup(x => x.Map<EventDto>(null))
                .Returns((EventDto) null);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            var result = await eventService.GetEventAsync(eventId);
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetEventAsync_WhenEventExist_ShouldReturnEventDto()
        {
            // Arrange
            var @event = GenerateNewEvent();
            var eventDto = EventToDto(@event);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            _mapperStub.Setup(x => x.Map<EventDto>(@event))
                .Returns(eventDto);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            var result = await eventService.GetEventAsync(@event.Id);
            // Assert
            result.Should()
                .BeEquivalentTo(eventDto);
        }

        [Fact]
        public async Task GetEventAsync_WhenEventWithSpecifiedTitleNotFound_ShouldReturnNull()
        {
            // Arrange
            var eventTitle = Guid.NewGuid().ToString();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((Event) null);
            _mapperStub.Setup(x => x.Map<EventDto>(null))
                .Returns((EventDto) null);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            var result = await eventService.GetEventAsync(eventTitle);
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetEventAsync_WhenEventWithSpecifiedTitleFound_ShouldReturnEventDto()
        {
            // Arrange
            var @event = GenerateNewEvent();
            var eventDto = EventToDto(@event);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(@event);
            _mapperStub.Setup(x => x.Map<EventDto>(@event))
                .Returns(eventDto);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            var result = await eventService.GetEventAsync(@event.Title);
            // Assert
            result.Should()
                .BeEquivalentTo(eventDto);
        }

        [Fact]
        public async Task GetEventThumbnailAsync_WhenEvenotFound_ShouldReturnNull()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Event) null);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            var result = await eventService.GetEventThumbnailAsync(eventId);
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetEventThumbnailAsync_WhenEvenotFound_ShouldReturnBinaryData()
        {
            // Arrange
            var binaryData = Guid.NewGuid().ToByteArray();
            var @event = GenerateNewEvent();
            @event.SetThumbnail(binaryData);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(@event);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            var result = await eventService.GetEventThumbnailAsync(@event.Id);
            // Assert
            result.Should()
                .BeEquivalentTo(binaryData);
        }

        [Fact]
        public void GetEventsAsync_WhenPageSizeIsNotValid_ShouldRaiseServiceException()
        {
            // Arrange
            var pageNumber = 0;
            var pageSize = 0;
            var queryDto = new PaginationQueryDto(pageNumber,pageSize);
            _eventRepositoryStub.Setup(x => x.CountAsync())
                .ReturnsAsync(10);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            Func<Task> getEventsFunc = () => eventService.GetEventsAsync(queryDto);
            // Assert
            getEventsFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Org.InvalidPaginationParams);
        }

        [Fact]
        public void GetEventsAsync_WhenPageNoIsNotValid_ShouldRaiseServiceException()
        {
            // Arrange
            var pageNumber = 15;
            var pageSize = 5;
            var queryDto = new PaginationQueryDto(pageNumber,pageSize);
            _eventRepositoryStub.Setup(x => x.CountAsync())
                .ReturnsAsync(10);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            Func<Task> getEventsFunc = () => eventService.GetEventsAsync(queryDto);
            // Assert
            getEventsFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.Org.InvalidPaginationParams);
        }

        [Fact]
        public async Task GetEventsAsync_WhenQueryIsValid_ShouldReturnPaginatedResponse()
        {
            // Arrange
            var pageNumber = 0;
            var pageSize = 5;
            var totalEventNo = 100;
            var queryDto = new PaginationQueryDto(pageNumber,pageSize);
            var existingEvents = new Event[] 
            {
                GenerateNewEvent(),
                GenerateNewEvent(),
                GenerateNewEvent(),
                GenerateNewEvent(),
                GenerateNewEvent(),
            };
            var eventDtos = existingEvents.Select(x => EventToDto(x));
            var pagitatedResponseDto = new PaginatedResponse<EventDto>(
                eventDtos,
                pageNumber,
                totalEventNo/pageSize,
                totalEventNo
            );
            _eventRepositoryStub.Setup(x => x.CountAsync())
                .ReturnsAsync(totalEventNo);
            _eventRepositoryStub.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(existingEvents);
            _mapperStub.Setup(x => x.Map<IEnumerable<EventDto>>(It.IsAny<List<Event>>()))
                .Returns(eventDtos);
            var eventService = new EventService(
                _mapperStub.Object,
                _eventRepositoryStub.Object,
                _userRepositoryStub.Object);
            // Act
            var result = await eventService.GetEventsAsync(queryDto);
            // Assert
            result.Should()
                .BeEquivalentTo(pagitatedResponseDto);
            
        }

        private Event GenerateNewEvent()
        {
            return Event.CreateEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(4)
            );
        }

        private EventDto EventToDto(Event @event)
        {
           return  new EventDto 
            {
                Id = @event.Id,
                OrganizerId = @event.OrganizerId,
                Participants = null,
                Title = @event.Title,
                Description = @event.Description,
                StartsAt = @event.StartsAt,
                EndsAt = @event.EndsAt,
                Status = @event.Status
            };
        }
    }
}
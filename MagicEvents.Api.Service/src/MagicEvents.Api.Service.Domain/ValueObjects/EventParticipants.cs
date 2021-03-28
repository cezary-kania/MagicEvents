using System;
using System.Collections.Generic;
using MagicEvents.Api.Service.Domain.Enums;

namespace MagicEvents.Api.Service.Domain.ValueObjects
{
    public class EventParticipants
    {
        private ISet<Guid> _standardParticipants = new HashSet<Guid>();
        private ISet<Guid> _coOrganizers = new HashSet<Guid>();
        public IEnumerable<Guid> StandardParticipants => _standardParticipants;
        public IEnumerable<Guid> CoOrganizers => _coOrganizers;

        public bool IsCoOrganizer(Guid userId)
        {
            return _coOrganizers.Contains(userId);
        }

        public bool IsStandardParticipant(Guid userId)
        {
            return _standardParticipants.Contains(userId);
        }

        public void AddParticipant(Guid userId, string role)
        {
            if(role == UserEventRole.StandardParticipant)
            {
                _standardParticipants.Add(userId);
            }
            else if(role == UserEventRole.CoOrganizer)
            {
                _coOrganizers.Add(userId);
            }
        }

        public void RemoveParticipant(Guid userId) 
        {
            var deletedUser = _coOrganizers.Remove(userId);
            if(!deletedUser)
            {
                _standardParticipants.Remove(userId);
            } 
        }
    }
}
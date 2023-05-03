using System;
using System.Collections.Generic;
using WPFClient.Matches.Service.Dto;

namespace WPFClient.Matches.Models
{
    public class PlayerMatchesModel
    {
        public readonly List<PlayerMatchModel> FoundMatches;
        public readonly List<PlayerMatchModel> ReceivedRequests;
        public readonly List<PlayerMatchModel> SentRequests;
        public readonly List<PlayerMatchModel> AcceptedRequests;

        public PlayerMatchesModel()
        {
            FoundMatches = new();
            ReceivedRequests = new();
            SentRequests = new();
            AcceptedRequests = new();
        }

        public PlayerMatchesModel(
            List<PlayerMatchModel> foundMatches,
            List<PlayerMatchModel> receivedRequests,
            List<PlayerMatchModel> sentRequests,
            List<PlayerMatchModel> acceptedRequests)
        {
            FoundMatches = foundMatches;
            ReceivedRequests = receivedRequests;
            SentRequests = sentRequests;
            AcceptedRequests = acceptedRequests;
        }
    }
}

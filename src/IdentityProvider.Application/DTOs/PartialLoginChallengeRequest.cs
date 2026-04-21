namespace IdentityProvider.Application.DTOs;

public record PartialLoginChallengeRequest(string Username);

public record PartialLoginChallengeResponse(int[][] PositionsToRequest);
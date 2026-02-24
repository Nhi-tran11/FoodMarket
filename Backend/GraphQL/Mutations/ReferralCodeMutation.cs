using Backend.Service;

namespace Backend.GraphQL.Mutations
{
    [ExtendObjectType("Mutation")]
    public class ReferralCodeMutation
    {
        public Task<String> GenerateReferralCodeAsync(
            int userId,
            [Service] IReferralService referralService)
        {
            return referralService.GenerateReferralCodeAsync(userId);
        }
    }
}
using Backend.Service;

namespace Backend.GraphQL.Mutations
{
    [ExtendObjectType("Mutation")]
    public class ReferralCodeMutation
    {
        public Task<String> GenerateReferralCodeAsync(
            int userId,
            string invitedUserEmail,
            [Service] IReferralService referralService)
        {
            return referralService.GenerateReferralCodeAsync(userId, invitedUserEmail);
        }

        public async Task<string> GenerateQRCodeBase64Async(
            string payload,
            [Service] IReferralService referralService)
        {
            var pngBytes = await referralService.GenerateQRCodeAsync(payload);
            return Convert.ToBase64String(pngBytes);
        }
        public Task<InvitationResult> SendInvitationEmailAsync(
            int inviterId,
            string inviteeEmail,
            [Service] IReferralService referralService)
        {
            return referralService.SendInvitationEmailAsync(inviterId, inviteeEmail);
        }
    }
}
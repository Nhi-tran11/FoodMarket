using Backend.Model;
using Backend.Service;

namespace Backend.GraphQL.Queries
{
    [ExtendObjectType("Query")]
    public class ReferralCodeQuery
    {
        public async Task<List<ReferalCode>> GetReferalCodeAsync(
            int userId, 
            [Service] IReferralService referralService)
        {
            return await referralService.GetReferalCodeByUserIdAsync(userId);
        }
        public async Task<ReferalCode?> GetReferalCodeByCodeAsync(
            int userId,
            string code,
            [Service] IReferralService referralService)
        {
            return await referralService.GetReferalCodeByCodeAndUserIdAsync(code, userId);
        }



    }
}
using Backend.Data;
using Backend.Model;

namespace Backend.Service
{
    public interface IShippingDetailService
    {
        Task<ShippingDetail> CreateShippingDetailAsync(int customerId, string fullName, string phoneNumber, string address, string city, string state, string zipCode, string country, bool isDefault);
        Task<List<ShippingDetail>> GetShippingDetailsByCustomerIdAsync(int customerId);
        Task<ShippingDetail?> GetDefaultShippingDetailByCustomerIdAsync(int customerId);
        Task<ShippingDetail?> GetShippingDetailByIdAsync(int id);
        Task<ShippingDetail> UpdateShippingDetailAsync(int id, int customerId, string fullName, string phoneNumber, string address, string city, string state, string zipCode, string country, bool isDefault);
        Task<bool> DeleteShippingDetailAsync(int id);
    }
    public class ShippingDetailService: IShippingDetailService
    {
        private readonly ApplicationDbContext _dbContext;
        public ShippingDetailService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ShippingDetail> CreateShippingDetailAsync(int customerId, string fullName, string phoneNumber, string address, string city, string state, string zipCode, string country, bool isDefault)
        {
            ShippingDetail? existingDefault = _dbContext.ShippingDetails.FirstOrDefault(s =>s.CustomerId == customerId && s.Address == address );
            if(existingDefault !=null)
            {
                if (existingDefault.IsDefault != isDefault)
                {
                    existingDefault.IsDefault = isDefault;
                }
                
                _dbContext.ShippingDetails.Update(existingDefault);
                return existingDefault;
            }
            var newShippingDetail = new ShippingDetail
                {
                    CustomerId = customerId,
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    City = city,
                    State = state,
                    ZipCode = zipCode,
                    Country = country,
                    IsDefault = isDefault
                };


                _dbContext.ShippingDetails.Add(newShippingDetail);
                await _dbContext.SaveChangesAsync();
                return newShippingDetail;
            

        }

        public Task<bool> DeleteShippingDetailAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ShippingDetail?> GetDefaultShippingDetailByCustomerIdAsync(int customerId)
        {
            ShippingDetail? defaultShippingDetail = _dbContext.ShippingDetails.FirstOrDefault(s =>s.CustomerId==customerId && s.IsDefault);
            return Task.FromResult(defaultShippingDetail);
        }

        public Task<ShippingDetail?> GetShippingDetailByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShippingDetail>> GetShippingDetailsByCustomerIdAsync(int customerId)
        {
            ShippingDetail[] shippingDetails = _dbContext.ShippingDetails.Where(s=>s.CustomerId == customerId).ToArray();
            return Task.FromResult(shippingDetails.ToList());
        }

        public Task<ShippingDetail> UpdateShippingDetailAsync(int id, int customerId, string fullName, string phoneNumber, string address, string city, string state, string zipCode, string country, bool isDefault)
        {
            throw new NotImplementedException();
        }
    }
}
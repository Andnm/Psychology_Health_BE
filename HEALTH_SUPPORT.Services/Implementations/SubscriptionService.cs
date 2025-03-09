using HEALTH_SUPPORT.Repositories.Entities;
using HEALTH_SUPPORT.Repositories;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEALTH_SUPPORT.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IBaseRepository<SubscriptionData, Guid> _subscriptionRepository;
        private readonly IBaseRepository<Category, Guid> _categoryRepository;
        private readonly IBaseRepository<Order, Guid> _orderRepository;
        private readonly IBaseRepository<SubscriptionProgress, Guid> _subscriptionProgressRepository;
        private readonly IBaseRepository<Psychologist, Guid> _psychologistRepository;
        private readonly IBaseRepository<Account, Guid> _accountRepository;

        public SubscriptionService(
            IBaseRepository<SubscriptionData, Guid> subscriptionRepository,
            IBaseRepository<Category, Guid> categoryRepository,
            IBaseRepository<Order, Guid> orderRepository,
            IBaseRepository<SubscriptionProgress, Guid> subscriptionProgressRepository,
            IBaseRepository<Psychologist, Guid> psychologistRepository,
            IBaseRepository<Account, Guid> accountRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
            _subscriptionProgressRepository = subscriptionProgressRepository;
            _psychologistRepository = psychologistRepository;
            _accountRepository = accountRepository;
        }

        public async Task AddSubscription(SubscriptionRequest.CreateSubscriptionModel model)
        {
            //Kiểm tra xem subscription đã tồn tại chưa
            var existingSubscription = await _subscriptionRepository.GetAll().AnyAsync(s => s.SubscriptionName == model.SubscriptionName);
            if (existingSubscription)
            {
                throw new Exception("Tên chương trình đã tồn tại!");
            }
            //Kiểm tra xem category có tồn tại không
            var category = await _categoryRepository.GetAll().FirstOrDefaultAsync(c => c.CategoryName.ToLower() == model.CategoryName.ToLower());
            if (category == null)
            {
                throw new Exception("Vui lòng chọn loại chương trình!");
            }
            var psychologist = await _psychologistRepository.GetAll().FirstOrDefaultAsync(p => p.Name == model.PsychologistName);
            if (psychologist == null)
            {
                throw new Exception("Vui lòng chọn tên chuyên gia");
            }
            try
            {
                var subscription = new SubscriptionData()
                {
                    Id = Guid.NewGuid(),
                    SubscriptionName = model.SubscriptionName,
                    Description = model.Description,
                    Price = model.Price,
                    Duration = model.Duration,
                    CategoryId = category.Id,
                    PsychologistId = psychologist.Id,
                    CreateAt = DateTimeOffset.UtcNow
                };
                await _subscriptionRepository.Add(subscription);
                await _subscriptionRepository.SaveChangesAsync();
            }
            
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<SubscriptionResponse.GetSubscriptionsModel?> GetSubscriptionById(Guid id)
        {
            var subscription = await _subscriptionRepository.GetAll().Include(s => s.Category).Include(s => s.Psychologists).FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null || subscription.IsDeleted)
            {
                return null;
            }

            return new SubscriptionResponse.GetSubscriptionsModel(
                subscription.Id,
                subscription.SubscriptionName,
                subscription.Description,
                (float)subscription.Price,
                subscription.Duration,
                subscription.Category?.CategoryName ?? "Unknown",
                subscription.Psychologists?.Name ?? "Unknown"
            );
        }

        public async Task<List<SubscriptionResponse.GetSubscriptionsModel>> GetSubscriptions()
        {
            return await _subscriptionRepository.GetAll()
            .Where(s => !s.IsDeleted) // Exclude deleted subscriptions
            .AsNoTracking()
            .Select(s => new SubscriptionResponse.GetSubscriptionsModel(
                s.Id,
                s.SubscriptionName,
                s.Description,
                (float)s.Price,
                s.Duration,
                s.Category != null ? s.Category.CategoryName : "Unknown",
                s.Psychologists != null ? s.Psychologists.Name : "Unknown"
            ))
            .ToListAsync();
        }

        public async Task RemoveSubscription(Guid id)
        {
            var subscription = await _subscriptionRepository.GetById(id);
            if (subscription == null)
            {
                throw new InvalidOperationException("Subscription not found.");
            }

            subscription.IsDeleted = true;
            subscription.ModifiedAt = DateTimeOffset.UtcNow; // Track modification

            await _subscriptionRepository.Update(subscription);
            await _subscriptionRepository.SaveChangesAsync();
        }

        public async Task UpdateSubscription(Guid id, SubscriptionRequest.UpdateSubscriptionModel model)
        {
            try
            {
                // Find the subscription by Id
                var existedSubscription = await _subscriptionRepository.GetById(id);

                if (existedSubscription == null)
                {
                    throw new Exception("Subscription not found.");
                }

                // Update only if new values are provided
                existedSubscription.Description = string.IsNullOrWhiteSpace(model.Description) ? existedSubscription.Description : model.Description;
                existedSubscription.Price = model.Price > 0 ? model.Price : existedSubscription.Price;
                existedSubscription.Duration = model.Duration > 0 ? model.Duration : existedSubscription.Duration;
                existedSubscription.ModifiedAt = DateTimeOffset.UtcNow;

                // Save changes to database
                await _subscriptionRepository.Update(existedSubscription);
                await _subscriptionRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Dùng để tạo order khi user đăng ký subscription
        public async Task<OrderResponse.GetOrderDetailsModel?> CreateOrder(SubscriptionRequest.RegisterSubscriptionModel model)
        {
            // Check if the subscription exists
            var subscription = await _subscriptionRepository.GetById(model.SubscriptionId);
            if (subscription == null || subscription.IsDeleted)
            {
                return null; // Subscription does not exist or is deleted
            }

            // Check if the account exists
            var account = await _accountRepository.GetById(model.AccountId);
            if (account == null || account.IsDeleted)
            {
                return null; // Account does not exist or is deleted
            }

            // Create a new order
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                SubscriptionDataId = model.SubscriptionId,
                AccountId = model.AccountId,
                Quantity = model.Quantity,
                CreateAt = DateTimeOffset.UtcNow,
                IsActive = true
            };

            await _orderRepository.Add(newOrder);
            await _orderRepository.SaveChangesAsync();

            // Return order details
            return await GetOrderDetails(newOrder.Id);
        }

        public async Task<Guid> CreateOrder(Guid subscriptionDataId, Guid accountId, int quantity)
        {
            var subscription = await _subscriptionRepository.GetById(subscriptionDataId);
            var account = await _accountRepository.GetById(accountId);

            if (subscription == null || account == null)
            {
                return Guid.Empty; // Subscription or Account does not exist
            }

            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                SubscriptionDataId = subscriptionDataId,
                AccountId = accountId,
                Quantity = quantity,
                CreateAt = DateTimeOffset.UtcNow,
                IsActive = true
            };

            await _orderRepository.Add(newOrder);
            await _orderRepository.SaveChangesAsync();

            return newOrder.Id;
        }


        //Để lấy thông tin chi tiết của order
        public async Task<OrderResponse.GetOrderDetailsModel?> GetOrderDetails(Guid orderId)
        {
            var order = await _orderRepository.GetAll()
                .Include(o => o.SubscriptionData)
                .Include(o => o.Accounts)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.IsDeleted)
            {
                return null;
            }

            return new OrderResponse.GetOrderDetailsModel(
                order.Id,
                order.SubscriptionData.SubscriptionName,
                order.SubscriptionData.Description,
                (float)order.SubscriptionData.Price,
                order.Quantity,
                order.Accounts.Fullname,
                order.Accounts.Email,
                order.CreateAt
            );
        }
        public async Task<OrderResponse.GetOrderDetailsModel?> ConfirmOrder(Guid orderId)
        {
            // Retrieve order
            var order = await _orderRepository.GetById(orderId);
            if (order == null || order.IsDeleted)
            {
                return null; // Order does not exist or was deleted
            }

            // Confirm the order
            order.IsActive = true;
            order.ModifiedAt = DateTimeOffset.UtcNow;

            await _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            // Return order details after confirmation
            return await GetOrderDetails(orderId);
        }


    }
}

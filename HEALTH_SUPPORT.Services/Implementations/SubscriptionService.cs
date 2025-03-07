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

        public SubscriptionService(
            IBaseRepository<SubscriptionData, Guid> subscriptionRepository,
            IBaseRepository<Category, Guid> categoryRepository,
            IBaseRepository<Order, Guid> orderRepository)

        {
            _subscriptionRepository = subscriptionRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
        }

        public async Task AddSubscription(SubscriptionRequest.CreateSubscriptionModel model)
        {
            var category = await _categoryRepository.GetAll().FirstOrDefaultAsync(c => c.Id == model.CategoryId);
            if (category == null)
            {
                throw new Exception("Invalid Category ID");
            }

            var subscription = new SubscriptionData()
            {
                Id = Guid.NewGuid(),
                SubscriptionName = model.SubscriptionName,
                Description = model.Description,
                Price = model.Price,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                CreateAt = DateTimeOffset.UtcNow
            };

            await _subscriptionRepository.Add(subscription);
        }

        public async Task<SubscriptionResponse.GetSubscriptionsModel?> GetSubscriptionById(Guid id)
        {
            var subscription = await _subscriptionRepository.GetAll()
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subscription == null || subscription.IsDeleted)
            {
                return null;
            }

            return new SubscriptionResponse.GetSubscriptionsModel(
                subscription.Id,
                subscription.SubscriptionName,
                subscription.Description,
                subscription.Price,
                subscription.Duration,
                subscription.CategoryId,
                subscription.Category?.CategoryName ?? "Unknown"
            );
        }

        public async Task<List<SubscriptionResponse.GetSubscriptionsModel>> GetSubscriptions()
        {
            return await _subscriptionRepository.GetAll()
                .Where(s => !s.IsDeleted)
                .AsNoTracking()
                .Select(s => new SubscriptionResponse.GetSubscriptionsModel(
                    s.Id,
                    s.SubscriptionName,
                    s.Description,
                    s.Price,
                    s.Duration,
                    s.CategoryId,
                    s.Category.CategoryName
                ))
                .ToListAsync();
        }

        public async Task RemoveSubscription(Guid id)
        {
            var subscription = await _subscriptionRepository.GetById(id);
            if (subscription == null)
            {
                throw new InvalidOperationException("Subscription not found");
            }

            subscription.IsDeleted = true;
            subscription.ModifiedAt = DateTimeOffset.UtcNow;

            await _subscriptionRepository.Update(subscription);
            await _subscriptionRepository.SaveChangesAsync();
        }

        public async Task UpdateSubscription(Guid id, SubscriptionRequest.UpdateSubscriptionModel model)
        {
            var existingSubscription = await _subscriptionRepository.GetById(id);
            if (existingSubscription is null)
            {
                throw new Exception("Subscription not found!");
            }

            existingSubscription.SubscriptionName = string.IsNullOrWhiteSpace(model.SubscriptionName)
                ? existingSubscription.SubscriptionName : model.SubscriptionName;
            existingSubscription.Description = string.IsNullOrWhiteSpace(model.Description)
                ? existingSubscription.Description : model.Description;
            existingSubscription.Price = model.Price ?? existingSubscription.Price;
            existingSubscription.Duration = model.Duration ?? existingSubscription.Duration;
            existingSubscription.CategoryId = model.CategoryId ?? existingSubscription.CategoryId;

            await _subscriptionRepository.Update(existingSubscription);
            await _subscriptionRepository.SaveChangesAsync();
        }
        public async Task RegisterSubscription(Guid accountId, SubscriptionRequest.RegisterSubscriptionModel model)
        {
            var subscription = await _subscriptionRepository.GetById(model.SubscriptionId);
            if (subscription == null || subscription.IsDeleted)
            {
                throw new Exception("Invalid Subscription ID");
            }

            var order = new Order()
            {
                Id = Guid.NewGuid(),
                SubscriptionDataId = model.SubscriptionId,
                AccountId = accountId,
                Quantity = model.Quantity,
                CreateAt = DateTimeOffset.UtcNow,
                IsActive = true
            };

            await _orderRepository.Add(order);
        }
        public async Task<List<SubscriptionResponse.GetSubscriptionsModel>> GetUserSubscriptions(Guid accountId)
        {
            return await _orderRepository.GetAll()
                .Where(o => o.AccountId == accountId && o.IsActive)
                .Include(o => o.SubscriptionData)
                .Select(o => new SubscriptionResponse.GetSubscriptionsModel(
                    o.SubscriptionData.Id,
                    o.SubscriptionData.SubscriptionName,
                    o.SubscriptionData.Description,
                    o.SubscriptionData.Price,
                    o.SubscriptionData.Duration,
                    o.SubscriptionData.CategoryId,
                    o.SubscriptionData.Category.CategoryName
                ))
                .ToListAsync();
        }
        public async Task CancelSubscription(Guid orderId)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null || !order.IsActive)
            {
                throw new Exception("Subscription not found or already canceled");
            }

            order.IsActive = false;
            await _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }
    }
}

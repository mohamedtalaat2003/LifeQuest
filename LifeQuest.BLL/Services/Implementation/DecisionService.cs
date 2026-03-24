using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
using LifeQuest.DAL.Exceptions;
using LifeQuest.DAL.Models;
using LifeQuest.DAL.UOW.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeQuest.BLL.Services.Implementation
{
    public class DecisionService : IDecisionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMetricsCalcService _metricsService;

        public DecisionService(IUnitOfWork unitOfWork, IMapper mapper, IMetricsCalcService metricsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _metricsService = metricsService;
        }

        public async Task<bool> AddDecisionAsync(DecisionDTO dto)
        {
            var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(dto.UserId);
            if (user == null) throw new NotFoundException("ApplicationUser", dto.UserId);

            var decision = _mapper.Map<Decision>(dto);
            decision.CreateAt = DateTime.Now;
            decision.UpdateAt = DateTime.Now;

            // Generate initial metrics (will be adjusted for the user's history)
            // Passing 0 as decisionId is fine if we link via navigation property next
            var metrics = await _metricsService.CalculateUserMetricsAsync(dto.UserId, 0);
            
            decision.Metrics = metrics;
            await _unitOfWork.Repository<Decision>().AddAsync(decision);
            
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateDecisionResultAsync(int decisionId, bool isSuccess)
        {
            var decision = await _unitOfWork.Repository<Decision>().GetByIdAsync(decisionId);
            if (decision == null) throw new NotFoundException("Decision", decisionId);

            decision.IsSuccess = isSuccess;
            decision.UpdateAt = DateTime.Now;
            await _unitOfWork.Repository<Decision>().Update(decision);

            // Recalculate metrics for the user after this decision is resolved
            var metrics = await _metricsService.CalculateUserMetricsAsync(decision.UserId, decision.Id);
            
            var existingMetrics = await _unitOfWork.Repository<MetricsCalc>().GetByIdWithIncludeAsync(m => m.DecisionId == decisionId);
            if (existingMetrics != null)
            {
                existingMetrics.SuccessRate = metrics.SuccessRate;
                existingMetrics.ConfidenceAccuracy = metrics.ConfidenceAccuracy;
                existingMetrics.OverConfidenceIndex = metrics.OverConfidenceIndex;
                existingMetrics.RiskPattern = metrics.RiskPattern;
                existingMetrics.UpdateAt = DateTime.Now;
                await _unitOfWork.Repository<MetricsCalc>().Update(existingMetrics);
            }
            else
            {
                await _unitOfWork.Repository<MetricsCalc>().AddAsync(metrics);
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<DecisionDTO>> GetUserDecisionsAsync(int userId)
        {
            var decisions = await _unitOfWork.Repository<Decision>()
                .GetAllWithIncludesAsync(d => d.UserId == userId, "Metrics");
            
            return _mapper.Map<IEnumerable<DecisionDTO>>(decisions);
        }

        public async Task<DecisionDTO?> GetDecisionDetailsAsync(int decisionId)
        {
            var decision = await _unitOfWork.Repository<Decision>()
                .GetByIdWithIncludeAsync(d => d.Id == decisionId, "Metrics");
            
            return _mapper.Map<DecisionDTO>(decision);
        }

        public async Task<bool> DeleteDecisionAsync(int decisionId)
        {
            var metrics = await _unitOfWork.Repository<MetricsCalc>().GetByIdWithIncludeAsync(m => m.DecisionId == decisionId);
            if (metrics != null)
            {
                await _unitOfWork.Repository<MetricsCalc>().Delete(metrics.Id);
            }

            await _unitOfWork.Repository<Decision>().Delete(decisionId);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}

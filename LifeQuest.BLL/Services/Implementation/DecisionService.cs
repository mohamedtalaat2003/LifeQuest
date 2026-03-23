using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.BLL.Services.Interfaces;
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
            // هتأكد بس ان المستخدم موجود عندنا فى السيستم
            var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(dto.UserId);
            if (user == null) throw new Exception("User not found!");

            // هعمل Mapping للقرار الجديد واظبط التواريخ
            var decision = _mapper.Map<Decision>(dto);
            decision.CreateAt = DateTime.Now;
            decision.UpdateAt = DateTime.Now;

            await _unitOfWork.Repository<Decision>().AddAsync(decision);
            
            // هحتاج اسيف القرار هنا عشان اخد ال ID بتاعه للمؤشرات (لان ال ID بيطلع من الداتابيز)
            await _unitOfWork.CompleteAsync();

            // هحسب بقا ال Metrics ونحفظها
            var metrics = await _metricsService.CalculateUserMetricsAsync(dto.UserId, decision.Id);
            await _unitOfWork.Repository<MetricsCalc>().AddAsync(metrics);
            
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateDecisionResultAsync(int decisionId, bool isSuccess)
        {
            // هشوف القرار ده موجود ولا لا
            var decision = await _unitOfWork.Repository<Decision>().GetByIdAsync(decisionId);
            if (decision == null) throw new Exception("Decision not found!");

            // هحدث حالة النجاح ووقت التعديل
            decision.IsSuccess = isSuccess;
            decision.UpdateAt = DateTime.Now;
            _unitOfWork.Repository<Decision>().Update(decision);

            // لازم نحدث المؤشرات تانى بعد ما النتيجة اتغيرت
            var metrics = await _metricsService.CalculateUserMetricsAsync(decision.UserId, decision.Id);
            
            // لو فيه Metrics قديمة هنحدثها.. لو مفيش هنكرت واحدة جديدة
            var existingMetrics = await _unitOfWork.Repository<MetricsCalc>().GetByIdWithIncludeAsync(m => m.DecisionId == decisionId);
            if (existingMetrics != null)
            {
                existingMetrics.SuccessRate = metrics.SuccessRate;
                existingMetrics.ConfidenceAccuracy = metrics.ConfidenceAccuracy;
                existingMetrics.OverConfidenceIndex = metrics.OverConfidenceIndex;
                existingMetrics.RiskPattern = metrics.RiskPattern;
                existingMetrics.UpdateAt = DateTime.Now;
                _unitOfWork.Repository<MetricsCalc>().Update(existingMetrics);
            }
            else
            {
                await _unitOfWork.Repository<MetricsCalc>().AddAsync(metrics);
            }

            // هعمل سيف مرة واحدة لكل التعديلات
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<DecisionDTO>> GetUserDecisionsAsync(int userId)
        {
            // هجيب كل القرارات اللى اخدها المستخدم ده
            var decisions = await _unitOfWork.Repository<Decision>()
                .GetAllWithIncludesAsync(d => d.UserId == userId, "Metrics");
            
            return _mapper.Map<IEnumerable<DecisionDTO>>(decisions);
        }

        public async Task<DecisionDTO?> GetDecisionDetailsAsync(int decisionId)
        {
            // هجيب داتا القرار كاملة ومعاها ال Metrics بتاعتها
            var decision = await _unitOfWork.Repository<Decision>()
                .GetByIdWithIncludeAsync(d => d.Id == decisionId, "Metrics");
            
            return _mapper.Map<DecisionDTO>(decision);
        }

        public async Task<bool> DeleteDecisionAsync(int decisionId)
        {
            // هحذف القرار والمؤشرات بتاعته عشان ننظف الداتابيز
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

using AutoMapper;
using LifeQuest.BLL.DTOs;
using LifeQuest.DAL.Models;

namespace LifeQuest.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            
            
             CreateMap<DailyLog, DailyLogDTO>()
            .ForMember(dest => dest.ChallengeName, opt => opt.MapFrom(src => src.UserChallenge!.Challenge!.Title));

             CreateMap<DailyLogDTO, DailyLog>()
            .ForMember(dest => dest.Challenge, opt => opt.Ignore());

            CreateMap<UserChallenge, UserChallengeDTO>()
                .ForMember(dest => dest.ChallengeName, opt => opt.MapFrom(src => src.Challenge!.Title));

            CreateMap<UserChallengeDTO, UserChallenge>()
                .ForMember(dest => dest.Challenge , opt => opt.Ignore());

            // Category Mappings
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>()
                .ForMember(dest => dest.Challenges, opt => opt.Ignore());

            CreateMap<Challenge, ChallengeDTO>();
            CreateMap<ChallengeDTO, Challenge>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore())
                .ForMember(dest => dest.DailyLogs, opt => opt.Ignore());

            // UserBadge Mappings
            CreateMap<UserBadge, UserBadgeDTO>()
                .ForMember(dest => dest.BadgeName, opt => opt.MapFrom(src => src.Badge!.Name));
            CreateMap<UserBadgeDTO, UserBadge>()
                .ForMember(dest => dest.Badge, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Badge Mappings
            CreateMap<Badges, BadgeDTO>()
                .ForMember(dest => dest.RequiredLevelName, opt => opt.MapFrom(src => src.RequiredLevel != null ? src.RequiredLevel.LevelName : string.Empty));
            CreateMap<BadgeDTO, Badges>()
                .ForMember(dest => dest.UserBadges, opt => opt.Ignore())
                .ForMember(dest => dest.RequiredLevel, opt => opt.Ignore());

            // Level Mappings
            CreateMap<Level, LevelDTO>();
            CreateMap<LevelDTO, Level>();

            // Decision Mappings
            CreateMap<Decision, DecisionDTO>();
            CreateMap<DecisionDTO, Decision>()
                .ForMember(dest => dest.Metrics, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // UserProfile Mappings
            CreateMap<UserProfile, UserProfileDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.LevelName, opt => opt.MapFrom(src => src.Level.LevelName))
                .ForMember(dest => dest.LevelNumber, opt => opt.MapFrom(src => src.Level.LevelsCount))
                .ForMember(dest => dest.TotalBadges, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveChallenges, opt => opt.Ignore())
                .ForMember(dest => dest.RemainingPointsForNextLevel, opt => opt.Ignore());

            // Metrics Mappings
            CreateMap<MetricsCalc, MetricsCalcDTO>();
            CreateMap<MetricsCalcDTO, MetricsCalc>()
                .ForMember(dest => dest.Decision, opt => opt.Ignore());
        }
    }
}

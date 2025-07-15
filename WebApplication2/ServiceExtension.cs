using System.Runtime.CompilerServices;
using WebApplication2.Properties.Services;
using WebApplication2.Properties.Services.Interfaces;

namespace WebApplication2
{
    public static class ServiceExtension
    {
        public static void Service(this IServiceCollection services) 
        {
            services.AddScoped<IAdMadiaService, AdMadiaService>();
            services.AddScoped<IAdsService, AdsService>();
            services.AddScoped<ICommentsService,CommentsService>();
            services.AddScoped<IMessagesService, MessagesService>();
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<ISubcategoriesService, SubcategoriesService>();
            services.AddScoped<IReportedContentsService, ReportedContentsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRatingsService,RatingsService>();
            services.AddScoped<IAdModerationsService,AdModerationsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserModerationsService, UserModerationsService>();
            services.AddScoped<IUserSocialAuthService, UserSocialAuthService>();
            services.AddScoped<IAdminAuditLogsService,AdminAuditLogsService>();
            services.AddScoped<IAdminPermissionsService,AdminPermissionsService>();
            services.AddScoped<IAdminsService, AdminsService>();
            services.AddScoped<IAdminsSessionsService,AdminsSessionsService>();
        }
    }
}

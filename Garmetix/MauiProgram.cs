
using CommunityToolkit.Maui;
using Garmetix.Databases;
using Garmetix.Dependencies;
using Microsoft.Extensions.Logging;

namespace Garmetix
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() //Placeholder to ensure the toolkit is registered before any module, as some modules might depend on it.
                .UseGarmetixDependencies() // Registering Garmetix dependencies and global configurations
                .ConfigureGarmetix()       // Registering Garmetix Modules and their routes         
                .UseGarmetixDatabases();   // Registering Garmetix Databases

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            //TODO: these will be remove in final version, just for testing purpose

            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();

            builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

            return builder.Build();
        }
    }
}

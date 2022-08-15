using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Bot.Common;
using Bot.BusinessLogic.Interfaces;
using Bot.BusinessLogic.Implementations;
using Bot.Models;
using Bot.Common.Mapper;

static void ConfigurationBuild(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables();
}


var builder = new ConfigurationBuilder();
ConfigurationBuild(builder);
builder.SetBasePath(Directory.GetCurrentDirectory());
builder.AddJsonFile("appsettings.json");
var config = builder.Build();
string connection = config.GetConnectionString("DefaultConnection");

var mapperConfig = new MapperConfiguration(x => { x.AddProfile<MapperProfile>(); });
mapperConfig.AssertConfigurationIsValid();
IMapper mapper = mapperConfig.CreateMapper();

var host = Host.CreateDefaultBuilder()
.ConfigureServices((context, services) =>
{
    services.AddTransient<ISectionService, SectionService>();
    services.AddTransient<ITeacherService, TeacherService>();
    services.AddTransient<ITeacherBySectionService, TeacherBySectionService>();
    services.AddTransient<IRunningTimeService, RunningTimeService>();
    services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));
    services.AddSingleton(mapper);
})
.Build();

var sectionService = ActivatorUtilities.CreateInstance<SectionService>(host.Services);
var teacherService = ActivatorUtilities.CreateInstance<TeacherService>(host.Services);
var viewService = ActivatorUtilities.CreateInstance<TeacherBySectionService>(host.Services);
var runningService = ActivatorUtilities.CreateInstance<RunningTimeService>(host.Services);

ITelegramBotClient botClient = new TelegramBotClient("5703718127:AAEX-jRSqF05MsOaetqhzQ__CwxBAO3cWEw");
var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;
var receivverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};
botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receivverOptions,
    cancellationToken: cts.Token);
Console.WriteLine("���" + " " + botClient.GetMeAsync().Result.FirstName + " " + "�������");
Console.ReadLine();
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient telegramBot, Update update, CancellationToken cancellationToken)
{

    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await HandleMessage(botClient, update.Message);
        return;
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }

}

async Task OutputKeyboard(ITelegramBotClient telegramBot, CallbackQuery callbackQuery)
{
    List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
    foreach (var item in sectionService.Gets())
    {
        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(text: item.Name, callbackData: item.Name) });
    }
    InlineKeyboardMarkup keyboardMarkup = new(buttons.ToArray());
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "�������� ������: ", replyMarkup: keyboardMarkup);
}

async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    if (callbackQuery.Data.StartsWith("section"))
    {
        OutputKeyboard(botClient, callbackQuery);

    }
    if (callbackQuery.Data.StartsWith(callbackQuery.Data))
    {

        foreach (var item in viewService.Gets(callbackQuery.Data))
        {
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"�������� ������: {item.SectionName}" + Environment.NewLine +
         /* $"����������: {item.SectionRunningTime}" + Environment.NewLine +
          $"��������������: {item.SectionLocation}" + Environment.NewLine +*/
          $"" + Environment.NewLine +
          $"��� �������������: {item.TeacherFullName}" + Environment.NewLine +
          $"����� ��������: {item.TeacherMobilePhone}" + Environment.NewLine +
          $"{GetData(item.SectionName, callbackQuery)}");

            InlineKeyboardMarkup markup = new(new[]
            {
           new[]{ InlineKeyboardButton.WithCallbackData(text: "�������� ���������� � ����� ������:", callbackData: "newSection")}
           });

            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "������� �� ������:", replyMarkup: markup);
            return;
        }
    }
    if (callbackQuery.Data.StartsWith("newSection"))
    {
        OutputKeyboard(botClient, callbackQuery);
    }
}

//string GetData(string name, CallbackQuery callbackQuery)
//{
//    while (runningService.Gets(name).Any(x => callbackQuery.Data.StartsWith(name)))
//    {
//        foreach (var item in runningService.Gets(name))
//        {
//            return $"����� ����������: {item.DayOfWeek} {item.StartTime} - {item.EndTime}\n��������������: {item.Location}";
//        }
//    }
//    return string.Empty;
//    //    //List<string> asd = new List<string>();
//    //    //foreach(var item in runningService.Gets(name))
//    //    //{
//    //    //   asd.Add($"����� ����������: {item.DayOfWeek} {item.StartTime} - {item.EndTime}\n��������������: {item.Location}");
//    //    //}
//    //    //return asd;
//}

List<RunningTimeDTO> GetData(string name, CallbackQuery callbackQuery)
{
    RunningTimeDTO runningTimeDTO = new RunningTimeDTO();
    while (runningService.Gets(name).Any(x => callbackQuery.Data.StartsWith(name)))
    {
        foreach (var item in runningService.Gets(name))
        {
            runningTimeDTO.Logs.Add(item);
        }
    }
    return runningTimeDTO.Logs;

}


async Task HandleMessage(ITelegramBotClient botClient, Message message)
{
    if (message.Text != "")
    {
        InlineKeyboardMarkup markup = new(new[]
        {
           new[]{ InlineKeyboardButton.WithCallbackData("������ �������", "section")}
        });

        await botClient.SendTextMessageAsync(message.Chat.Id, "������� �� ������:", replyMarkup: markup);
        return;
    }

}

Task HandleErrorAsync(ITelegramBotClient telegramBot, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException => $"������ Telegram API:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static TelegramBotClient botClient;

    // Словарь категорий и идей
    private static Dictionary<string, List<string>> ideas = new Dictionary<string, List<string>>()
    {
        { "technology", new List<string> { "Создать AI-приложение для улучшения сна", "Стартап по разработке роботов-помощников" } },
        { "business", new List<string> { "Открыть кафе с виртуальной реальностью", "Создать маркетплейс для фрилансеров" } },
        { "art", new List<string> { "Платформа для создания цифрового искусства", "Создание онлайн-галереи с выставками" } }
    };

    static async Task Main ( string[] args )
    {
        
        botClient = new TelegramBotClient ("8189954499:AAEBlXu8uW_zPzIE1yWIAEoI3RFGqem8uUU");

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() 
        };

        botClient.StartReceiving (HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

        var me = await botClient.GetMeAsync();
        Console.WriteLine ($"Начал работать бот @{me.Username}");

        Console.ReadLine ();
        cts.Cancel ();
    }

    // Обработка обновлений
    static async Task HandleUpdateAsync ( ITelegramBotClient botClient, Update update, CancellationToken cancellationToken )
    {
        if ( update.Type == UpdateType.Message && update.Message.Type == MessageType.Text )
        {
            var messageText = update.Message.Text.ToLower();
            var chatId = update.Message.Chat.Id;

            if ( messageText == "/start" )
            {
                await botClient.SendTextMessageAsync (chatId, "Привет! Я бот-генератор идей. Напиши категорию: technology, business, art", cancellationToken: cancellationToken);
            }
            else if ( ideas.ContainsKey (messageText) )
            {
                var randomIdea = GetRandomIdea(messageText);
                await botClient.SendTextMessageAsync (chatId, $"Вот идея для категории {messageText}: {randomIdea}", cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync (chatId, "Я не понимаю эту категорию. Напиши: technology, business, art", cancellationToken: cancellationToken);
            }
        }
    }

    
    static Task HandleErrorAsync ( ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken )
    {
        Console.WriteLine (exception);
        return Task.CompletedTask;
    }

    
    static string GetRandomIdea ( string category )
    {
        var rnd = new Random();
        var categoryIdeas = ideas[category];
        int index = rnd.Next(categoryIdeas.Count);
        return categoryIdeas[index];
    }
}